using Microsoft.CodeAnalysis;

namespace LingoGen.Generator;

public static class Diagnostics
{
    public static readonly DiagnosticDescriptor NoJson = new("LINGO1000",
        "No lingo.json file found",
        "No lingo.json file found",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor InvalidLanguage = new("LINGO1001",
        "Invalid language",
        "Invalid language: {0}",
        "LingoGen",
        DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor KeyStartsWithDigit = new("LINGO1002",
        "Key starts with a number",
        "The key '{0}' starts with a digit, prefixing with 'd'",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor JsonException = new("LINGO1003",
        "Json exception",
        "Json exception: {0}",
        "LingoGen",
        DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NoMetaData = new("LINGO1004",
        "No metadata found",
        "No metadata found in lingo.json",
        "LingoGen",
        DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NoVersionFound = new("LINGO1005",
        "Version not specified",
        "lingo.json version is not specified",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor NoLanguagesFound = new("LINGO1006",
        "Supported languages not specified",
        "There are no supported languages specified in lingo.json",
        "LingoGen",
        DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NoPhrasesFound = new("LINGO1007",
        "No phrases found",
        "No phrases found in lingo.json",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor InvalidJsonFormat = new("LINGO1008",
        "Invalid JSON format",
        "Invalid JSON format: {0}",
        "LingoGen",
        DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor ExtraPhraseArgument = new("LINGO1009",
        "Extra argument",
        "Phrase '{0}' has an extra argument '{1}' in '{2}'",
        "LingoGen",
        DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor MissingPhraseArgument = new("LINGO1010",
        "Missing argument",
        "Phrase '{0}' is missing an argument for '{1}' in '{2}'",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor ExtraTranslation = new("LINGO1011",
        "Extra translation",
        "'{0}' has an extra translation for '{1}'",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor MissingTranslation = new("LINGO1012",
        "Missing translation",
        "'{0}' is missing a translation for '{1}'",
        "LingoGen",
        DiagnosticSeverity.Error, true);

    // TODO: Use this diagnostic
    public static readonly DiagnosticDescriptor PhraseNounDuplicate = new("LINGO1013",
        "There already exists a noun for this phrase",
        "Noun '{0}' has a value for the phrase '{1}'",
        "LingoGen",
        DiagnosticSeverity.Error, true);
    
    public static readonly DiagnosticDescriptor NounIsIncomplete = new("LINGO1014",
        "Noun is incomplete",
        "Noun '{0}' is incomplete for '{1}'",
        "LingoGen",
        DiagnosticSeverity.Error, true);
}