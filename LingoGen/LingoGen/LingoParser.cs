using System.IO;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace LingoGen;

public class LingoParser
{
    private static LingoJson? ParseLingoJson(AdditionalText text)
    {
        // Check if the file name is the specific file that we expect.
        if (Path.GetFileName(text.Path) != "lingo.json")
            return null;

        var json = text.GetText();

        if (json is null)
            return null;

        var lingoJson = JsonSerializer.Deserialize<LingoJson>(json.ToString());

        return lingoJson ?? null;
    }
}