using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using LingoGen.Generator.DataTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LingoGen.Generator.Parsing;

public class LingoJsonParser(string filePath) : ILingoJsonParser
{
    private readonly List<Diagnostic> _diagnostics = [];
    private readonly LingoData _lingoData = new();

    private readonly Regex _argumentRegex = new("(?<={)(.*?)(?=})");

    /// <summary>
    /// All languages that are supported by the CultureInfo class 
    /// </summary>
    private static readonly ImmutableHashSet<string> AllLanguages =
        CultureInfo.GetCultures(CultureTypes.NeutralCultures).Select(x => x.TwoLetterISOLanguageName).ToImmutableHashSet();

    public ParserResult Parse(string json)
    {
        JObject jObject;
        try
        {
            jObject = JObject.Parse(json);
        }
        catch (JsonException e)
        {
            return ParserResult.FromException(e, filePath);
        }

        if (!ParseMetaData(jObject))
        {
            return new()
            {
                Diagnostics = _diagnostics
            };
        }

        ParsePhrases(jObject);

        return new()
        {
            LingoData = _lingoData,
            Diagnostics = _diagnostics
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
            if (String.IsNullOrWhiteSpace(versionString))
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
                Report(languageToken, Diagnostics.InvalidLanguage, $"'{languageToken}' is not a string");
                continue;
            }

            var language = languageToken.Value<string>();
            if (String.IsNullOrWhiteSpace(language))
            {
                Report(languageToken, Diagnostics.InvalidLanguage, "Language is empty");
                continue;
            }

            if (!AllLanguages.Contains(language!))
            {
                Report(languageToken, Diagnostics.InvalidLanguage, $"'{language}' is not a valid TwoLetterISOLanguageName");
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

        // We dont need to check for duplicate keys because JObject will filter them out
        
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

        var key = CreateKey(property);
        if (key is null)
        {
            Report(property, Diagnostics.InvalidJsonFormat, "Key is empty or invalid");
            return null;
        }

        var phrase = new LingoPhrase
        {
            Key = key,
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

    private string? CreateKey(JProperty property)
    {
        var name = property.Name;

        var sb = new StringBuilder();

        var nextIsUpper = false;
        var inBraces = false;

        // Check if the name is empty
        if (String.IsNullOrWhiteSpace(name))
            return null;

        // Add underscore if the first character is a number
        if (Char.IsDigit(name.FirstOrDefault()))
        {
            Report(property, Diagnostics.KeyStartsWithDigit, name);
            sb.Append("d");
        }

        foreach (var c in name)
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


        _diagnostics.Add(Diagnostic.Create(descriptor, Location.Create(filePath, textSpan, linePositionSpan), messageArgs));
    }
}