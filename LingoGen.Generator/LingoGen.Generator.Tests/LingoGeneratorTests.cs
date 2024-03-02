namespace LingoGen.Generator.Tests;

public class LingoGeneratorTests
{
    [Fact]
    public void InvalidJson_ReturnsJsonExceptionDiagnostic()
    {
        // Arrange
        const string json = "{ invalid json }";

        // Act
        var result = RunGenerator(json);

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.JsonException.Id);
    }
    
    [Fact]
    public void EmptyLingoJson_ReturnsNoMetaDataDiagnostic()
    {
        // Arrange
        const string json = "{ }";

        // Act
        var result = RunGenerator(json);

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.NoMetaData.Id);
    }

    [Fact]
    public void MetadataWithoutVersion_ReturnsNoVersionDiagnostic()
    {
        // Arrange
        const string json =
            """
            {
              "metadata": { }
            }
            """;

        // Act
        var result = RunGenerator(json);

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.NoVersionFound.Id);
    }
    
    [Fact]
    public void MetadataWithEmptyVersion_ReturnsNoVersionDiagnostic()
    {
        // Arrange
        const string json =
            """
            {
              "metadata": {
                version: " "
              }
            }
            """;

        // Act
        var result = RunGenerator(json);

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.NoVersionFound.Id);
    }
    
    [Fact]
    public void MetadataWithoutLanguage_ReturnsNoLanguageDiagnostic()
    {
        // Arrange
        const string json =
            """
            {
              "metadata": {
                "version": "1.0"
              }
            }
            """;

        // Act
        var result = RunGenerator(json);

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.NoLanguagesFound.Id);
    }
    
    [Fact]
    public void MetadataWithAnotherObjectAsLanguage_ReturnsInvalidJsonDiagnostic()
    {
        // Arrange
        const string json =
            """
            {
              "metadata": {
                "version": "1.0",
                "languages": ["en", 123]
              }
            }
            """;

        // Act
        var result = RunGenerator(json);

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.InvalidLanguage.Id);
    }
    
    [Fact]
    public void MetadataWithEmptyLanguage_ReturnsInvalidJsonDiagnostic()
    {
        // Arrange
        const string json =
            """
            {
              "metadata": {
                "version": "1.0",
                "languages": ["en", " "]
              }
            }
            """;

        // Act
        var result = RunGenerator(json);

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.InvalidLanguage.Id);
    }
    
    [Fact]
    public void MetadataWithInvalidLanguage_ReturnsInvalidJsonDiagnostic()
    {
        // Arrange
        const string json =
            """
            {
              "metadata": {
                "version": "1.0",
                "languages": ["en", "cheese"]
              }
            }
            """;

        // Act
        var result = RunGenerator(json);

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.InvalidLanguage.Id);
    }
   
    [Fact]
    public void NoPhrases_ReturnsNoPhrasesDiagnostic()
    {
        // Arrange
        const string json =
            """
            {
              "metadata": {
                "version": "1.0",
                "languages": ["en", "nl"]
              }
            }
            """;

        // Act
        var result = RunGenerator(json);

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.NoPhrasesFound.Id);
    }
    
    [Fact]
    public void NoLingoJson_ReturnsNoJsonDiagnostic()
    {
        // Act
        var result = RunGenerator();

        // Assert
        result.Diagnostics.Should().ContainSingle(x => x.Id == Diagnostics.NoJson.Id);
    }
}