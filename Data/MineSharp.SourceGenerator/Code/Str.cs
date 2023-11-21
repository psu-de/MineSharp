using System.Globalization;

namespace MineSharp.SourceGenerator.Code;

public static class Str
{

    private static NumberFormatInfo _nfi = new NumberFormatInfo() {
        NumberDecimalSeparator = "."
    };
    
    public static string Float(float value)
        => value.ToString(_nfi) + "f";

    public static string Bool(bool value)
        => value ? "true" : "false";

    public static string String(string value)
        => $"\"{value}\"";

    
}
