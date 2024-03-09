﻿namespace LingoGen.Generator;

public static class Classes
{
    public const string Namespace = "LingoGen";

    public static string Lingo =>
        $$"""
          // <auto-generated/>

          using System.Globalization;

          namespace {{Namespace}};

          /// <summary>
          /// Static class containing all lingo entries.
          /// </summary>
          public static partial class Lingo
          {
              public static string GetLanguageCode() => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
          }
          """;

    public const string Content =
        $$"""
          // <auto-generated/>

          #nullable enable

          namespace {{Namespace}}
          {
              /// <summary>
              /// Content that can be used to provide translations.
              /// </summary>
              public abstract class Content
              {
                  /// <summary>
                  /// Returns a localized string based on the current language.
                  /// </summary>
                  /// <returns>Returns a localized string based on the current language.</returns>
                  public sealed override string ToString()
                  {
                      var code = Lingo.GetLanguageCode();
                      return ToString(code) ?? $"[ No '{code}' translation for '{GetType().Name}' ]";
                  }
              
                  /// <summary>
                  /// Returns a localized string based on the provided language code.
                  /// </summary>
                  public abstract string? ToString(string languageCode);
                  
                  /// <summary>
                  /// Implicitly converts the content to a string.
                  /// </summary>
                  public static implicit operator string(Content content) => content.ToString();
              }
          }
          """;

    public const string Noun =
        $$"""
          // <auto-generated/>

          namespace {{Namespace}}
          {
              /// <summary>
              /// Noun class that can be used to provide translations for nouns.
              /// </summary>
              public abstract class Noun
              {
                  public abstract Content Singular { get; }
                  
                  public abstract Content Plural { get; }
              }
          }
          """;
}