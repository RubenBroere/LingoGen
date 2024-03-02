using LingoGen.Generator.DataTypes;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace LingoGen.Generator.Parsing;

/// <summary>
/// The result of a parser, containing the parsed LingoData and any diagnostics
/// </summary>
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