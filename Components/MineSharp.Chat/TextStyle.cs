namespace MineSharp.Chat;

public class TextStyle
{
    public static TextStyle[] KnownStyles = new[] {
        new TextStyle('0', "black"),
        new TextStyle('1', "dark_blue"),
        new TextStyle('2', "dark_green"),
        new TextStyle('4', "dark_red"),
        new TextStyle('3', "dark_aqua") { Aliases = new[] { "dark_cyan" } },
        new TextStyle('5', "dark_purple") { Aliases = new[] { "dark_magenta" } },
        new TextStyle('6', "gold") { Aliases = new[] { "dark_yellow" } },
        new TextStyle('7', "gray"),
        new TextStyle('8', "dark_gray"),
        new TextStyle('9', "blue"),
        new TextStyle('a', "green"),
        new TextStyle('b', "aqua") { Aliases = new[] { "cyan" } },
        new TextStyle('c', "red"),
        new TextStyle('d', "light_purple") { Aliases = new[] {"magenta" } },
        new TextStyle('e', "yellow"),
        new TextStyle('f', "white"),
        
        new TextStyle('k', "magic"),
        new TextStyle('l', "bold"),
        new TextStyle('m', "strikethrough"),
        new TextStyle('n', "underline"),
        new TextStyle('o', "italic"),
        new TextStyle('r', "reset"),
    };

    private const char PREFIX = '$';

    public char Code { get; private set; }
    public string Name { get; private set; }
    public string[] Aliases { get; set; } = Array.Empty<string>();

    public TextStyle(char code, string name)
    {
        this.Code = code;
        this.Name = name;
    }

    public override string ToString() => $"{PREFIX}{this.Code}";

    public static TextStyle? GetTextStyle(string name)
    {
        return KnownStyles.FirstOrDefault(x =>
            string.Equals(name, x.Name, StringComparison.OrdinalIgnoreCase) ||
            x.Aliases.Any(y => string.Equals(name, y, StringComparison.OrdinalIgnoreCase)));
    }
}