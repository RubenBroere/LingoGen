using System.Text;
using LingoGen.Generator.DataTypes;

namespace LingoGen.Generator;

public static class LingoClass
{
    public static string Build(LingoPhrase phrase)
    {
        var cb = ClassBuilder.Create("Lingo", "LingoGen").AsStatic().AsPartial()
            .AddUsings("System", "System.Globalization");

        
        var sb = new StringBuilder();
        
        // Method declaration
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// {phrase.Translations["en"]}");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static string " + phrase.Key + " => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch");
        sb.AppendLine("    {");

        // Translations
        foreach (var translation in phrase.Translations)
        {
            sb.AppendLine($"        \"{translation.Key}\" => \"{translation.Value}\",");
        }

        // Default
        sb.AppendLine("        _ => $\"[ No '" + phrase.Key + "' lingo for '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' ]\"");
        sb.Append("    };");

        cb.AddBody(sb.ToString());

        return cb.ToString();
    }
}