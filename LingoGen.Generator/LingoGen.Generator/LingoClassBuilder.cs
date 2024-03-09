﻿using LingoGen.Generator.DataTypes;

namespace LingoGen.Generator;

public static class LingoClass
{
    public static string BuildPhrase(LingoPhrase phrase)
    {
        var sb = new SourceBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Globalization;");
        sb.AppendLine();
        sb.AppendLine($"namespace {Classes.Namespace}");
        using (sb.EnterBlock())
        {
            sb.AppendLine("public static partial class Lingo");
            using (sb.EnterBlock())
            {
                var hasArguments = phrase.Arguments.Count > 0;
                var parameters = hasArguments ? $"({String.Join(", ", phrase.Arguments.Select(x => $"string {x}"))})" : "";

                // Public static property
                sb.AppendLine("/// <summary>");
                sb.AppendLine($"/// {phrase.Translations["en"]}");
                sb.AppendLine("/// </summary>");
                sb.AppendLine(hasArguments
                    ? $"public static Content {phrase.Key}{parameters} => new {phrase.Key}Content({String.Join(", ", phrase.Arguments)});"
                    : $"public static Content {phrase.Key} {{ get; }} = new {phrase.Key}Content();");

                BuildContentClass(sb, phrase.Key, phrase.Translations, parameters);
            }
        }

        return sb.ToString();
    }

    public static string BuildNoun(LingoNoun noun)
    {
        var sb = new SourceBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Globalization;");
        sb.AppendLine();
       
        sb.AppendLine($"namespace {Classes.Namespace}");
        using (sb.EnterBlock())
        {
            sb.AppendLine("public static partial class Lingo");
            using (sb.EnterBlock())
            {
                // Public static property
                sb.AppendLine("/// <summary>");
                sb.AppendLine($"/// {noun.Singular["en"]} | {noun.Plural["en"]}");
                sb.AppendLine("/// </summary>");
                sb.AppendLine($"public static Noun {noun.Key} {{ get; }} = new {noun.Key}Noun();");
                sb.AppendLine();

                // Noun class
                sb.AppendLine($"private sealed class {noun.Key}Noun : Noun");
                using (sb.EnterBlock())
                {
                    // Public static singular property
                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// {noun.Singular["en"]}");
                    sb.AppendLine("/// </summary>");
                    sb.AppendLine($"public override Content Singular {{ get; }} = new {noun.Key}SingularContent();");
                    sb.AppendLine();

                    // Public static plural property
                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// {noun.Plural["en"]}");
                    sb.AppendLine("/// </summary>");
                    sb.AppendLine($"public override Content Plural {{ get; }} = new {noun.Key}PluralContent();");
                    sb.AppendLine();

                    BuildContentClass(sb, $"{noun.Key}Singular", noun.Singular, "");
                    sb.AppendLine();
                    
                    BuildContentClass(sb, $"{noun.Key}Plural", noun.Plural, "");
                }
            }
        }

        return sb.ToString();
    }

    private static void BuildContentClass(SourceBuilder sb, string key, Dictionary<string, string> translations, string parameters)
    {
        var formatter = parameters.Length > 0 ? "$" : "";

        // Content class
        sb.AppendLine($"private sealed class {key}Content{parameters} : Content");
        using (sb.EnterBlock())
        {
            // Switch statement 
            sb.AppendLine("public override string? ToString(string languageCode) => languageCode switch");
            using (sb.EnterIndentedRegion("{", "};"))
            {
                // Translations
                foreach (var translation in translations)
                {
                    sb.AppendLine($"\"{translation.Key}\" => {formatter}\"{translation.Value}\",");
                }

                // Default
                sb.AppendLine("_ => null");
            }
        }
    }
}