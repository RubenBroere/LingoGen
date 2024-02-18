using LingoGen.Generator.DataTypes;
using Microsoft.CodeAnalysis;

namespace LingoGen.Generator.Validation;

public static class LingoValidator
{
    public static List<Diagnostic> Validate(LingoData? lingoJson, string filePath)
    {
        var diagnostics = new List<Diagnostic>();

        return diagnostics;
    }
}