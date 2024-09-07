namespace MineSharp.Core.Common;

// TODO: use this enum in more places like Chat
// and add extension methods that convert it to and from the §0-9a-fk-r format

/// <summary>
/// Enum representing the various text colors (and stylings) available in Minecraft.
/// </summary>
public enum TextColor
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Black = 0,
    DarkBlue = 1,
    DarkGreen = 2,
    DarkAqua = 3,
    DarkRed = 4,
    DarkPurple = 5,
    Gold = 6,
    Gray = 7,
    DarkGray = 8,
    Blue = 9,
    Green = 10,
    Aqua = 11,
    Red = 12,
    LightPurple = 13,
    Yellow = 14,
    White = 15,
    Obfuscated = 16,
    Bold = 17,
    Strikethrough = 18,
    Underlined = 19,
    Italic = 20,
    Reset = 21
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
