namespace LingoGen.Generator.Sample;

public class Examples
{
    public Examples()
    {
        // Logs "ERROR: Users"
        LogTranslatedError(Lingo.Nouns.Person.Singular.Capitalized());

        // Logs "ERROR: Select a(n) user"
        LogTranslatedError(Lingo.Phrases.SelectAn_(Lingo.Nouns.Person.Singular));
    }

    private static void LogTranslatedError(Content content)
    {
        Console.WriteLine($"ERROR: {content}");
    }
}

public static class NounExtensions
{
    public static ContentMapped Capitalized(this Content content) => new(content, StringExtensions.Capitalize);
}

/// <summary>
/// Custom content that can be used to provide custom translations.
/// </summary>
/// <param name="translations">List of translations linked to their language code</param>
public class ContentCustom(IReadOnlyDictionary<string, string> translations) : Content
{
    public override string? ToString(string languageCode) => translations.GetValueOrDefault(languageCode);

    public static implicit operator string(ContentCustom content) => content.ToString();
}

/// <summary>
/// Custom content that can be used to map translations.
/// </summary>
public class ContentMapped(Content content, Func<string, string> map) : Content
{
    public override string? ToString(string languageCode)
    {
        var translation = content.ToString(languageCode);

        return translation is null ? null : map(translation);
    }
}

public static class StringExtensions
{
    public static string Capitalize(this string input)
    {
        if (String.IsNullOrEmpty(input))
        {
            return String.Empty;
        }

        return String.Create(input.Length, input, static (chars, str) =>
        {
            chars[0] = Char.ToUpperInvariant(str[0]);
            str.AsSpan(1).CopyTo(chars[1..]);
        });
    }
}