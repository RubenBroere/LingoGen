using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LingoGen.Generator.Tests.Utils;

public static class TestBase
{
    public static GeneratorDriverRunResult RunGenerator(IEnumerable<AdditionalText>? additionalTexts = null)
    {
        // Create an instance of the source generator.
        var generator = new LingoGenerator();

        // Source generators should be tested using 'GeneratorDriver'.
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, additionalTexts);

        // We need to create a compilation with the required source code.
        var compilation = CSharpCompilation.Create(nameof(LingoGeneratorTests), Enumerable.Empty<SyntaxTree>(), new[]
        {
            // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        });

        // Run generators and retrieve all results.
        return driver.RunGenerators(compilation).GetRunResult();
    }

    public static GeneratorDriverRunResult RunGenerator(string lingoJson)
    {
        return RunGenerator([new TestAdditionalFile("lingo.json", lingoJson)]);
    }
}