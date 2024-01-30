using System.Collections.Generic;

namespace LingoGen;

public sealed class LingoEntry(string path, Dictionary<string, string> translations)
{
    public string Path { get; } = path;

    public Dictionary<string, string> Translations { get; } = translations;
}