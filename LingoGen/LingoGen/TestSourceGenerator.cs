﻿using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace LingoGen;

[Generator]
public class TestSourceGenerator : ISourceGenerator
{
    private const string Namespace = "Generators";


    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this generator.
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var lingoFile = context.AdditionalFiles
            .FirstOrDefault(static t => Path.GetFileName(t.Path) == "lingo.json");

        var lingoJson = lingoFile?.GetText();

        if (lingoJson is null)
            return;

        var lingo = JsonSerializer.Deserialize<LingoJson>(lingoJson.ToString());

        if (lingo is null)
            return;

        var entries = lingo.Entries?.Select(static (x, _) => new LingoEntry
        {
            Key = x.Key,
            Translations = x.Value,
        });
        
        var translationSourceCode = $$"""
                                      // <auto-generated/>

                                      namespace {{Namespace}}
                                      {
                                         public static partial class Lingo
                                         {
                                            /*  */
                                         }
                                      }
                                      """;

        context.AddSource("Lingo.g.cs", SourceText.From(translationSourceCode, Encoding.UTF8));
    }
}