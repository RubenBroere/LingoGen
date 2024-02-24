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

        if (!parser.ParseMetaData(jObject))
        {
            return new()
            {
                Diagnostics = parser._diagnostics
            };
        }

        parser.ParsePhrases(jObject);

        return new()
        {
            LingoData = parser._lingoData,
            Diagnostics = parser._diagnostics
        };
    }

    private bool ParseMetaData(JObject jObject)
    {
        if (!jObject.TryGetValue<JObject>("metadata", out var metaData))
        {
            Report(jObject, Diagnostics.NoMetaData);
            return false;
        }

        if (metaData.TryGetValue<JValue>("version", out var version) && version.Type == JTokenType.String)
        {
            var versionString = version.Value<string>();
            if (String.IsNullOrEmpty(versionString))
            {
                Report(version, Diagnostics.NoVersionFound);
            }

            _lingoData.MetaData.Version = versionString;
        }
        else
        {
            Report(metaData, Diagnostics.NoVersionFound);
        }

        if (!metaData.TryGetValue<JArray>("languages", out var languages))
        {
            Report(metaData, Diagnostics.NoLanguagesFound);
            return false;
        }

        foreach (var languageToken in languages)
        {
            if (languageToken.Type != JTokenType.String)
            {
                Report(languageToken, Diagnostics.InvalidJsonFormat, $"Language '{languageToken}' is not a string");
                continue;
            }

            var language = languageToken.Value<string>();
            if (String.IsNullOrEmpty(language))
            {
                Report(languageToken, Diagnostics.InvalidJsonFormat, "Language is empty");
                continue;
            }

            _lingoData.MetaData.Languages.Add(language!);
        }

        return true;
    }

    private void ParsePhrases(JObject jObject)
    {
        if (!jObject.TryGetValue<JObject>("phrases", out var phrases))
        {
            Report(jObject, Diagnostics.NoPhrasesFound);
            return;
        }

        // We dont need to check for duplicate keys because JObject will not allow it

        foreach (var property in phrases.Properties())
        {
            var phrase = ParsePhrase(property);
            if (phrase is not null)
                _lingoData.Phrases.Add(phrase);
        }
    }

    private LingoPhrase? ParsePhrase(JProperty property)
    {
        Dictionary<string, string> translations;
        try
        {
            translations = property.Value.ToObject<Dictionary<string, string>>() ?? new();
        }
        catch (Exception e)
        {
            Report(property.Value, Diagnostics.JsonException, e.Message);
            return null;
        }

        var phrase = new LingoPhrase
        {
            Key = CreateKey(property.Name),
            Translations = translations
        };

        foreach (var missingLanguage in _lingoData.MetaData.Languages.Except(translations.Keys))
        {
            Report(property.Value, Diagnostics.MissingTranslation, phrase.Key, missingLanguage);
        }

        foreach (var extraLanguage in translations.Keys.Except(_lingoData.MetaData.Languages))
        {
            Report(property.Value[extraLanguage]!, Diagnostics.ExtraTranslation, phrase.Key, extraLanguage);
        }

        // Add english translation after checking for missing languages 
        translations.Add("en", property.Name);

        foreach (Match match in _argumentRegex.Matches(property.Name))
        {
            phrase.Arguments.Add(match.Value);
        }

        foreach (var translation in translations)
        {
            var translationArgs = _argumentRegex.Matches(translation.Value).OfType<Match>().Select(x => x.Value).ToList();

            foreach (var extraArg in translationArgs.Except(phrase.Arguments))
            {
                Report(property.Value[translation.Key]!, Diagnostics.ExtraArgument, phrase.Key, extraArg, translation);
                return null;
            }

            foreach (var missingArg in phrase.Arguments.Except(translationArgs))
            {
                // Missing argument is an error
                Report(property.Value[translation.Key]!, Diagnostics.MissingArgument, phrase.Key, missingArg, translation);
            }
        }

        return phrase;
    }

    private static string CreateKey(string input)
    {
        var sb = new StringBuilder();

        var nextIsUpper = false;
        var inBraces = false;
        
        // TODO: What if the input is empty?
        // TODO: What if the input starts with an number?
        
        foreach (var c in input)
        {
            if (Char.IsLetterOrDigit(c) && !inBraces)
            {
                sb.Append(nextIsUpper ? Char.ToUpper(c) : c);
                nextIsUpper = false;
            }

            switch (c)
            {
                case '{':
                    inBraces = true;
                    break;
                case '}':
                    sb.Append('_');
                    inBraces = false;
                    nextIsUpper = true;
                    break;
                case ' ':
                    nextIsUpper = true;
                    break;
            }
        }

        return sb.ToString();
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private void Report(JToken token, DiagnosticDescriptor descriptor, params object?[] messageArgs)
    {
        var info = (IJsonLineInfo)token;
        var length = token.ToString().Length;

        var textSpan = new TextSpan(info.LinePosition - 1, length);
        var startPosition = new LinePosition(info.LineNumber - 1, info.LinePosition - 1);
        var endPosition = new LinePosition(info.LineNumber - 1, info.LinePosition - 1);
        var linePositionSpan = new LinePositionSpan(startPosition, endPosition);


        _diagnostics.Add(Diagnostic.Create(descriptor, Location.Create(_filePath, textSpan, linePositionSpan), messageArgs));
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