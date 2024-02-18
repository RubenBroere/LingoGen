namespace LingoGen.Generator.DataTypes;

public sealed class LingoPhrase
{
    public string Key { get; set; } = "";

    public Dictionary<string, string> Translations { get; set; } = [];
    
    public List<string> Arguments { get; } = [];
}

public sealed class LingoData
{
    public MetaData MetaData { get; set; } = new();

    public List<LingoPhrase> Phrases { get; } = [];

    public Dictionary<string, Dictionary<string, string[]>> Nouns { get; } = [];
}

public sealed class MetaData
{
    public List<string> Languages { get; set; } = [];

    public string? Version { get; set; }
}