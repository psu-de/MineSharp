namespace MineSharp.ChatComponent;

/// <summary>
///     Represents a Minecraft Text style
/// </summary>
public class TextStyle(char code, string name)
{
    private const char Prefix = '$';

    /// <summary>
    ///     All known text styles
    /// </summary>
    public static TextStyle[] KnownStyles =
    {
        new('0', "black"), new('1', "dark_blue"), new('2', "dark_green"), new('4', "dark_red"),
        new('3', "dark_aqua") { Aliases = new[] { "dark_cyan" } },
        new('5', "dark_purple") { Aliases = new[] { "dark_magenta" } },
        new('6', "gold") { Aliases = new[] { "dark_yellow" } }, new('7', "gray"), new('8', "dark_gray"),
        new('9', "blue"), new('a', "green"), new('b', "aqua") { Aliases = new[] { "cyan" } }, new('c', "red"),
        new('d', "light_purple") { Aliases = new[] { "magenta" } }, new('e', "yellow"), new('f', "white"),
        new('k', "magic"), new('l', "bold"), new('m', "strikethrough"), new('n', "underline"), new('o', "italic"),
        new('r', "reset")
    };

    /// <summary>
    ///     The character for this style
    /// </summary>
    public char Code { get; } = code;

    /// <summary>
    ///     The name of this text style
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    ///     An array of alias names for this text style
    /// </summary>
    public string[] Aliases { get; set; } = Array.Empty<string>();


    /// <summary>
    ///     Parse text style from string
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TextStyle? GetTextStyle(string name)
    {
        return KnownStyles.FirstOrDefault(x =>
                                              string.Equals(name, x.Name, StringComparison.OrdinalIgnoreCase) ||
                                              x.Aliases.Any(
                                                  y => string.Equals(name, y, StringComparison.OrdinalIgnoreCase)));
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Prefix}{Code}";
    }
}
