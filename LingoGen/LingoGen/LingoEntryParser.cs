using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace LingoGen;

public static class LingoEntryParser
{
    public static IncrementalValuesProvider<LingoEntry> ParseLingo(IncrementalValuesProvider<string> jsons)
    {
        var jsonProperties = jsons
            .SelectMany(static (text, _) => JObject.Parse(text).Properties())
            .WithComparer(new JTokenEqualityComparer());

        var lingoEntries = jsonProperties
            .SelectMany(static (jProperty, _) => ParseLingo(jProperty))
            .WithComparer(new LingoEntryComparer());

        return lingoEntries;
    }
    
    private static IEnumerable<LingoEntry> ParseLingo(JProperty jProperty)
    {
        var entries = new List<LingoEntry>();
        try
        {
            var entry = jProperty.Value.ToObject<Dictionary<string, string>>();

            if (entry is not null)
                entries.Add(new(jProperty.Path, entry));
        }
        catch (Exception)
        {
            if (jProperty.Value is JObject jObject)
            {
                foreach (var property in jObject.Properties())
                {
                    entries.AddRange(ParseLingo(property));
                }
            }
        }

        return entries;
    }
}