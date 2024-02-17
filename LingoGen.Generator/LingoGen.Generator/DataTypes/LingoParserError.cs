using Newtonsoft.Json;

namespace LingoGen.Generator.DataTypes;

public readonly struct LingoParserError(LingoErrorType lingoErrorType, string? path, int lineNumber, int linePosition)
{
    public LingoErrorType LingoErrorType { get; } = lingoErrorType;
    
    public string? Path { get; } = path;
    
    public int LineNumber { get; } = lineNumber;
    
    public int LinePosition { get; } = linePosition;

    public static LingoParserError FromException(JsonException e, LingoErrorType type)
    {
        return e switch
        {
            JsonReaderException re => new(type, re.Path, re.LineNumber, re.LinePosition),
            JsonSerializationException se => new(type, se.Path, se.LineNumber, se.LinePosition),
            _ => throw new ArgumentOutOfRangeException(nameof(e), e, null)
        };
    }
}