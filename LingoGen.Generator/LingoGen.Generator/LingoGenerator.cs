using LingoGen.Generator.Parsing;
using Microsoft.CodeAnalysis;

namespace LingoGen.Generator;

[Generator(LanguageNames.CSharp)]
public class LingoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Initialize the compilation with the lingo class 
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("Lingo.g.cs", Classes.Lingo);
            ctx.AddSource("Lingo.Content.g.cs", Classes.Content);
            ctx.AddSource("Lingo.Noun.g.cs", Classes.Noun);
        });

        // Cache file contents
        var lingoFiles = context.AdditionalTextsProvider
            .Where(static x => Path.GetFileName(x.Path) == "lingo.json")
            .Select(static (x, ct) => ((string Path, string Content))(x.Path, x.GetText(ct)?.ToString())!)
            .Where(static x => x.Content is not null);

        // Bool to indicate if there are no files
        var noFiles = lingoFiles.Collect().Select((x, _) => x.IsEmpty);

        var codeModel = lingoFiles.Select((x, _) =>
        {
            // TODO: Use cancellation token
            ILingoJsonParser parser = new LingoJsonParser(x.Path);

            var parserResult = parser.Parse(x.Content);

            return new GenerateCodeModel
            {
                ParserResult = parserResult
            };
        });

        // TODO: Cache lingo entries

        context.RegisterSourceOutput(noFiles.Combine(codeModel.Collect()), (ctx, tuple) =>
        {
            if (tuple.Left)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(Diagnostics.NoJson, Location.None));
                return;
            }

            foreach (var model in tuple.Right)
            {
                GenerateCode(ctx, model);
            }
        });
    }

    private static void GenerateCode(SourceProductionContext ctx, GenerateCodeModel model)
    {
        foreach (var error in model.ParserResult.Diagnostics)
        {
            ctx.ReportDiagnostic(error);
        }

        foreach (var phrase in model.ParserResult.LingoData.Phrases)
        {
            var source = LingoClass.BuildPhrase(phrase);
            ctx.AddSource($"Lingo.Phrase.{phrase.Key}.g.cs", source);
        }

        foreach (var noun in model.ParserResult.LingoData.Nouns)
        {
            var source = LingoClass.BuildNoun(noun);
            ctx.AddSource($"Lingo.Noun.{noun.Key}.g.cs", source);
        }
    }
}

public class GenerateCodeModel
{
    public ParserResult ParserResult { get; set; } = new();
}