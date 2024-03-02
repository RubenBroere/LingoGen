namespace LingoGen.Generator.Parsing;

public interface ILingoJsonParser
{
    /// <summary>
    /// Parses the given json string and returns a ParserResult
    /// </summary>
    public ParserResult Parse(string json);
}