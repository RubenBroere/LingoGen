using System.Collections.Generic;

namespace LingoGen.DataTypes;

public sealed class LingoEntry(string fullPath, Dictionary<string, string> translations)
{
    /// <summary>
    /// Path from the root of the json file to the entry.
    /// </summary>
    public string FullPath { get; } = fullPath;

    /// <summary>
    /// List of translations for the entry.
    /// </summary>
    public Dictionary<string, string> Translations { get; } = translations;
}