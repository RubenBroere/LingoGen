using System.Globalization;

namespace LingoGen.Console.Example;

public static partial class Lingo
{
    public static string Hello => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
    {
        "en" => "Hello",
        "de" => "Hallo",
        _ => $"[ No 'Hello' for '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' ]"
    };
}

public static partial class Lingo
{
    public static partial class Clothing
    {
        public static string Jacket => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "en" => "Jacket",
            "nl" => "Jas",
            _ => $"[ No 'Jacket' lingo for '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' ]"
        };
    }
}

public static partial class Lingo
{
    public static partial class Finance
    {
        public static string Invoice => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "en" => "Invoice",
            "de" => "Rechnung",
            _ => $"[ No 'Invoice' for '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' ]"
        };
    }
}

public static partial class Lingo
{
    public static partial class Finance
    {
        public static string Currency => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "en" => "Currency",
            "de" => "Währung",
            _ => $"[ No 'Currency' lingo for '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' ]"
        };
    }
}

public class C
{
    public void M()
    {
        var x = Lingo.Finance.Invoice;
    }
}

public class LingoClothing
{
    public string Jacket => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
    {
        "en" => "Jacket",
        "nl" => "Jas",
        _ => $"[ No 'Jacket' lingo for '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' ]"
    };
}