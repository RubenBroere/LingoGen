using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LingoGen.Generator.Parsing;

public static class JsonExtensions
{
    public static bool TryGetValue<T>(this JObject jObject, string key, out T value)
    {
        if (jObject.TryGetValue(key, out var token) && token is T jValue)
        {
            value = jValue;
            return true;
        }

        value = default!;
        return false;
    }

    public static Diagnostic ToDiagnostic(this JsonException e, string path)
    {
        var info = e switch
        {
            JsonReaderException re => new { re.LineNumber, re.LinePosition },
            JsonSerializationException se => new { se.LineNumber, se.LinePosition },
            _ => throw new ArgumentOutOfRangeException(nameof(e), e, null)
        };

        var location = Location.Create(path, new(),
            new(new(info.LineNumber - 1, info.LinePosition - 1), new(info.LineNumber - 1, info.LinePosition - 1)));

        return Diagnostic.Create(Diagnostics.InvalidJsonFormat, location, e.Message);
    }
}