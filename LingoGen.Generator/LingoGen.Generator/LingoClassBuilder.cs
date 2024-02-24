using System.Text;
using LingoGen.Generator.DataTypes;

namespace LingoGen.Generator;

public static class LingoClass
{
    public static string Create(LingoPhrase phrase)
    {
        var cb = ClassBuilder.Create("Lingo", LingoGenerator.Namespace).AsStatic().AsPartial()
            .AddUsings("System", "System.Globalization");

        var sb = new StringBuilder();
        
        var parameters = phrase.Arguments.Count > 0 ? $"({String.Join(", ", phrase.Arguments.Select(x => $"string {x}"))})" : "";
        var formatter = phrase.Arguments.Count > 0 ? "$" : "";
        
        // Method declaration
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// {phrase.Translations["en"]}");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    public static string {phrase.Key}{parameters} => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch");
        sb.AppendLine("    {");

        // Translations
        foreach (var translation in phrase.Translations)
        {
            sb.AppendLine($"        \"{translation.Key}\" => {formatter}\"{translation.Value}\",");
        }

        // Default
        sb.AppendLine("        _ => $\"[ No '" + phrase.Key + "' lingo for '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' ]\"");
        sb.Append("    };");

        cb.AddBody(sb.ToString());

        return cb.ToString();
    }
}