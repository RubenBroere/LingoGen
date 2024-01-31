using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LingoGen;

public static class LingoEntryParser
{
    public static IEnumerable<LingoEntry> ParseLingo2(string json, CancellationToken cancellationToken = default)
    {
        using var reader = new JsonTextReader(new StringReader(json));
        
        reader.Read();
        
        while (reader.Read())
        {
            Console.WriteLine(reader.Path);
        }

        return Enumerable.Empty<LingoEntry>();
    }
    
    
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