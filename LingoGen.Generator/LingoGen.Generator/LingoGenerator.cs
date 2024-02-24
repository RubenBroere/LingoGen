using LingoGen.Generator.Parsing;
using Microsoft.CodeAnalysis;

namespace LingoGen.Generator;

[Generator(LanguageNames.CSharp)]
public class LingoGenerator : IIncrementalGenerator
{
    public const string Namespace = "LingoGen";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Initialize the compilation with the lingo class 
        context.RegisterPostInitializationOutput(ctx =>
        {
            var cb = ClassBuilder.Create("Lingo", Namespace).AsStatic().AsPartial().WithSummary("Static class containing all lingo entries.");
            ctx.AddSource("Lingo.g.cs", cb.ToString());
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
            var parserResult = LingoJsonParser.Parse(x.Content, x.Path);

            return new GenerateCodeModel
            {
                ParserResult = parserResult
            };
        });

        // TODO: Cache lingo entries

        context.RegisterSourceOutput(codeModel.Combine(noFiles), GenerateCode);
    }

    private static void GenerateCode(SourceProductionContext ctx, (GenerateCodeModel model, bool noFiles) input)
    {
        var (model, noFiles) = input;

        if (noFiles)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(Diagnostics.NoJsonWarning, Location.None));
            return;
        }

        foreach (var error in model.ParserResult.Diagnostics)
        {
            ctx.ReportDiagnostic(error);
        }

        foreach (var phrase in model.ParserResult.LingoData.Phrases)
        {
            var source = LingoClass.Create(phrase);
            ctx.AddSource($"Lingo.{phrase.Key}.g.cs", source);
        }
    }
}

public class GenerateCodeModel
{
    public ParserResult ParserResult { get; set; } = new();
}