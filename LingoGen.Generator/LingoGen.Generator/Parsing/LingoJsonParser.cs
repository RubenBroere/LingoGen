using System.Text;
using System.Text.RegularExpressions;
using LingoGen.Generator.DataTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LingoGen.Generator.Parsing;

public class LingoJsonParser
{
    private readonly string _filePath;

    private readonly List<Diagnostic> _diagnostics = [];
    private readonly LingoData _lingoData = new();

    private readonly Regex _argumentRegex = new("(?<={)(.*?)(?=})");

    private LingoJsonParser(string filePath)
    {
        _filePath = filePath;
    }

    public static ParserResult Parse(string json, string filePath)
    {
        var parser = new LingoJsonParser(filePath);

        JObject jObject;
        try
        {
            jObject = JObject.Parse(json);
        }
        catch (JsonException e)
        {
            return ParserResult.FromException(e, filePath);
        }

        parser.ParseMetaData(jObject);
        parser.ParsePhrases(jObject);

        return new()
        {
            LingoData = parser._lingoData,
            Diagnostics = parser._diagnostics
        };
    }

    private void ParseMetaData(JObject jObject)
    {
        if (!jObject.TryGetValue<JObject>("metadata", out var metaData))
        {
            Report(jObject, "No metadata found");
            return;
        }

        if (metaData.TryGetValue<JValue>("version", out var version))
        {
            if (version.Type == JTokenType.String)
            {
                var versionString = version.Value<string>();
                if (String.IsNullOrEmpty(versionString))
                {
                    Report(version, "Version is empty");
                }

                _lingoData.MetaData.Version = versionString;
            }
        }
        else
        {
            Report(metaData, "No version found");
        }

        if (!metaData.TryGetValue<JArray>("languages", out var languages))
        {
            Report(metaData, "No languages found");
            return;
        }

        foreach (var languageToken in languages)
        {
            if (languageToken.Type != JTokenType.String)
            {
                Report(languageToken, "Language is not a string");
                continue;
            }

            var language = languageToken.Value<string>();
            if (String.IsNullOrEmpty(language))
            {
                Report(languageToken, "Language is empty");
                continue;
            }

            _lingoData.MetaData.Languages.Add(language!);
        }
    }

    private void ParsePhrases(JObject jObject)
    {
        if (!jObject.TryGetValue<JObject>("phrases", out var phrases))
        {
            Report(jObject, "No phrases found");
            return;
        }

        // We dont need to check for duplicate keys because JObject will not allow it

        foreach (var property in phrases.Properties())
        {
            Dictionary<string, string> translations;
            try
            {
                translations = property.Value.ToObject<Dictionary<string, string>>() ?? new();
            }
            catch (Exception e)
            {
                Report(property.Value, e.Message);
                return;
            }

            var phrase = new LingoPhrase
            {
                Key = CreateKey(property.Name),
                Translations = translations
            };

            foreach (var missingLanguage in _lingoData.MetaData.Languages.Except(translations.Keys))
            {
                Report(property.Value, $"Phrase '{phrase.Key}' does not have a required '{missingLanguage}' translation");
            }

            foreach (var extraLanguage in translations.Keys.Except(_lingoData.MetaData.Languages))
            {
                Report(property.Value[extraLanguage]!, $"Phrase '{phrase.Key}' has a '{extraLanguage}' translation which is not supported");
            }

            foreach (Match match in _argumentRegex.Matches(property.Name))
            {
                phrase.Arguments.Add(match.Value);
            }

            foreach (var translation in translations)
            {
                var translationArgs = _argumentRegex.Matches(translation.Value).OfType<Match>().Select(x => x.Value).ToList();

                foreach (var extraArg in translationArgs.Except(phrase.Arguments))
                {
                    Report(property.Value[translation.Key]!, $"Phrase '{phrase.Key}' has an extra argument '{extraArg}'");
                }

                foreach (var missingArg in phrase.Arguments.Except(translationArgs))
                {
                    Report(property.Value[translation.Key]!, $"Phrase '{phrase.Key}' is missing the argument '{missingArg}'");
                }
            }

            _lingoData.Phrases.Add(phrase);
        }
    }


    private static string CreateKey(string input)
    {
        var sb = new StringBuilder();

        var nextIsUpper = false;
        foreach (var c in input)
        {
            if (Char.IsLetterOrDigit(c))
            {
                sb.Append(nextIsUpper ? Char.ToUpper(c) : c);
                nextIsUpper = false;
            }

            if (c == ' ')
                nextIsUpper = true;
        }

        return sb.ToString();
    }

    private void Report(JToken token, string message)
    {
        var info = (IJsonLineInfo)token;
        var length = token.ToString().Length;

        var textSpan = new TextSpan(info.LinePosition - 1, length);
        var startPosition = new LinePosition(info.LineNumber - 1, info.LinePosition - 1);
        var endPosition = new LinePosition(info.LineNumber - 1, info.LinePosition - 1);
        var linePositionSpan = new LinePositionSpan(startPosition, endPosition);


        _diagnostics.Add(Diagnostic.Create(Diagnostics.InvalidJsonFormat, Location.Create(_filePath, textSpan, linePositionSpan), message));
    }
}

public sealed class ParserResult
{
    public LingoData LingoData { get; set; } = new();

    public List<Diagnostic> Diagnostics { get; set; } = [];

    public static ParserResult FromException(JsonException e, string filePath)
    {
        var parserResult = new ParserResult();
        parserResult.Diagnostics.Add(e.ToDiagnostic(filePath));
        return parserResult;
    }
}