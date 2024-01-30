using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace LingoGen;

public static class LingoEntryParser
{
    public static IEnumerable<LingoEntry> ParseLingo(string json)
    {
        return JObject.Parse(json).Properties().SelectMany(ParseLingo);
    }
    
    private static IEnumerable<LingoEntry> ParseLingo(JProperty jProperty)
    {
        try
        {
            var entry = jProperty.Value.ToObject<Dictionary<string, string>>();

            if (entry is not null)
                return new[] { new LingoEntry(jProperty.Path, entry) };
        }
        catch (Exception)
        {
            if (jProperty.Value is JObject jObject)
            {
                return jObject.Properties().SelectMany(ParseLingo);
            }
        }

        return Enumerable.Empty<LingoEntry>();
    }
}