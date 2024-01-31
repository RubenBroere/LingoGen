using System.Linq;
using LingoGen.Tests.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace LingoGen.Tests;

public class LingoGeneratorTests
{
    private const string LingoJson =
        """
        {
            "Hello": {
                "en" : "Hello",
                "nl" : "Hallo"
            },
            "World": {
                "en" : "World",
                "nl" : "Wereld"
            }
        }
        """;

    [Fact]
    public void GenerateLingoClasses()
    {
        // Create an instance of the source generator.
        var generator = new LingoGenerator();

        // Source generators should be tested using 'GeneratorDriver'.
        var driver = CSharpGeneratorDriver.Create(
            new[] { generator.AsSourceGenerator() },
            new[] { new TestAdditionalFile("lingo.json", LingoJson) });

        // We need to create a compilation with the required source code.
        var compilation = CSharpCompilation.Create(nameof(LingoGeneratorTests),
            Enumerable.Empty<SyntaxTree>(),
            new[]
            {
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            });

        // Run generators and retrieve all results.
        var runResult = driver.RunGenerators(compilation).GetRunResult();

        // All generated files can be found in 'RunResults.GeneratedTrees'.
        var generatedFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("Lingo.Hello.g.cs"));
    }
}