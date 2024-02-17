using LingoGen.DataTypes;
using Microsoft.CodeAnalysis;

namespace LingoGen;

public static class Diagnostics
{
    public static readonly DiagnosticDescriptor NoJsonWarning = new("LG1000",
        "No lingo.json file found",
        "No lingo.json file found",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor NoEntriesWarning = new("LG1001",
        "No entries in lingo.json",
        "No entries in {0}",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor InvalidJsonFormat = new("LG1003",
        "Invalid json format",
        "Invalid json format: {0}",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor InvalidObject = new("LG1004",
        "Invalid Lingo item",
        "'{0}' is neither a Lingo entry nor a Lingo category",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static Diagnostic FromParserError(LingoParserError error, string filePath)
    {
        var location = Location.Create(filePath, new(),
            new(new(error.LineNumber - 1, error.LinePosition - 1), new(error.LineNumber - 1, error.LinePosition - 1)));

        return error.LingoErrorType switch
        {
            LingoErrorType.InvalidJsonFormat => Diagnostic.Create(InvalidJsonFormat, location, filePath),
            LingoErrorType.InvalidObject => Diagnostic.Create(InvalidObject, location, error.Path),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}