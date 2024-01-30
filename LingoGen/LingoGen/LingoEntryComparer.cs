using System.Collections.Generic;

namespace LingoGen;

public sealed class LingoEntryComparer : IEqualityComparer<LingoEntry>
{
    public bool Equals(LingoEntry x, LingoEntry y)
    {
        if (x.Path != y.Path)
            return false;

        if (x.Translations.Count != y.Translations.Count)
            return false;

        foreach (var translation in y.Translations)
        {
            if (!x.Translations.TryGetValue(translation.Key, out var value))
                return false;

            if (value != translation.Value)
                return false;
        }

        return true;
    }

    // TODO: Create a better hashcode
    public int GetHashCode(LingoEntry obj) => obj.Path.GetHashCode();
}