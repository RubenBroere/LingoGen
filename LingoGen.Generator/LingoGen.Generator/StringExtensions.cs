namespace LingoGen.Generator;

public static class StringExtensions
{
    /// <summary>
    /// Returns the input string with the first character converted to uppercase, or mutates any nulls passed into string.Empty
    /// </summary>
    public static string Capitalize(this string s)
    {
        if (String.IsNullOrEmpty(s))
            return String.Empty;

        var a = s.ToCharArray();
        a[0] = Char.ToUpper(a[0]);
        return new(a);
    }
}