using LingoGen.DataTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LingoGen;

public static class LingoDataParser
{
    private static readonly JsonLoadSettings Settings = new()
    {
        DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
    };

    public static LingoData ParseLingo(string json, CancellationToken ct = default)
    {
        var lingoData = new LingoData();

        try
        {
            var jObject = JObject.Parse(json, Settings);
            ParseProperties(jObject, lingoData, ct);
        }
        catch (JsonException e)
        {
            lingoData.Errors.Add(LingoParserError.FromException(e, LingoErrorType.InvalidJsonFormat));
        }

        return lingoData;
    }


    private static LingoData ParseLingoRecursive(JObject jObject, string fullPath, CancellationToken ct = default)
    {
        var lingoData = new LingoData();

        if (ct.IsCancellationRequested)
            return lingoData;

        try
        {
            var entry = jObject.ToObject<Dictionary<string, string>>();
            if (entry != null)
            {
                lingoData.LingoEntries.Add(new(fullPath, entry));
                return lingoData;
            }
        }
        catch (JsonException) // Not a lingo entry
        {
            try
            {
                ParseProperties(jObject, lingoData, ct);
            }
            catch (JsonException e)
            {
                lingoData.Errors.Add(LingoParserError.FromException(e, LingoErrorType.InvalidJsonFormat));
            }
        }

        return lingoData;
    }

    private static void ParseProperties(JObject jObject, LingoData lingoData, CancellationToken ct)
    {
        foreach (var jProperty in jObject.Properties())
        {
            var lineInfo = (IJsonLineInfo)jProperty.Value;
            if (jProperty.Value.Type != JTokenType.Object)
            {
                lingoData.Errors.Add(new(LingoErrorType.InvalidObject, jProperty.Path, lineInfo.LineNumber, lineInfo.LinePosition));
                continue;
            }

            var propObj = (JObject)jProperty.Value;

            var newEntries = ParseLingoRecursive(propObj, jProperty.Value.Path, ct);

            lingoData.Errors.AddRange(newEntries.Errors);
            lingoData.LingoEntries.AddRange(newEntries.LingoEntries);
        }
    }
}