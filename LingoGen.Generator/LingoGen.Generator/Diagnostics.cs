using Microsoft.CodeAnalysis;

namespace LingoGen.Generator;

public static class Diagnostics
{
    public static readonly DiagnosticDescriptor NoJsonWarning = new("LINGO1000",
        "No lingo.json file found",
        "No lingo.json file found",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor NoEntriesWarning = new("LINGO1001",
        "No entries in lingo.json",
        "No entries in {0}",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor InvalidJsonFormat = new("LINGO1003",
        "Invalid json format",
        "Invalid json format: {0}",
        "LingoGen",
        DiagnosticSeverity.Warning, true);

    private static readonly DiagnosticDescriptor InvalidObject = new("LINGO1004",
        "Invalid Lingo item",
        "'{0}' is neither a Lingo entry nor a Lingo category",
        "LingoGen",
        DiagnosticSeverity.Warning, true);
}