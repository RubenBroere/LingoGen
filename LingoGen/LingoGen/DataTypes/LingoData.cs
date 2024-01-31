using System.Collections.Generic;

namespace LingoGen.DataTypes;

public sealed class LingoData
{
    public List<LingoEntry> LingoEntries { get; } = [];

    public List<LingoParserError> Errors { get; } = [];
    
    public void Deconstruct(out List<LingoEntry> lingoEntries, out List<LingoParserError> errors)
    {
        lingoEntries = LingoEntries;
        errors = Errors;
    }
}