namespace LingoGen.Generator.DataTypes;

public sealed class LingoPhrase
{
    public string Key { get; set; } = "";

    public Dictionary<string, string> Translations { get; set; } = [];

    public List<string> Arguments { get; } = [];
}

public sealed class LingoNoun
{
    public string Key { get; set; } = "";

    public Dictionary<string, string> Singular { get; set; } = [];

    public Dictionary<string, string> Plural { get; set; } = [];
}

public sealed class LingoData
{
    public MetaData MetaData { get; set; } = new();

    public List<LingoPhrase> Phrases { get; } = [];

    public List<LingoNoun> Nouns { get; } = [];
}

public sealed class MetaData
{
    public List<string> Languages { get; set; } = [];

    public string? Version { get; set; }
}