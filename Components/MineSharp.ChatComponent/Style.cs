using fNbt;
using Newtonsoft.Json.Linq;

namespace MineSharp.ChatComponent;

/// <summary>
///     Represents the text style of a chat component
/// </summary>
public class Style
{
    /// <summary>
    ///     The default style
    /// </summary>
    public static Style DefaultStyle => new();

    /// <summary>
    ///     The text color
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    ///     The font
    /// </summary>
    public string? Font { get; set; }

    /// <summary>
    ///     Whether the text is bold
    /// </summary>
    public bool? Bold { get; set; }

    /// <summary>
    ///     Whether the text is italic
    /// </summary>
    public bool? Italic { get; set; }

    /// <summary>
    ///     Whether the text is underlined
    /// </summary>
    public bool? Underlined { get; set; }

    /// <summary>
    ///     Whether the text is struck through
    /// </summary>
    public bool? Strikethrough { get; set; }

    /// <summary>
    ///     Whether the text is obfuscated
    /// </summary>
    public bool? Obfuscated { get; set; }


    /// <summary>
    ///     Encode this style as json
    /// </summary>
    /// <returns></returns>
    public JObject? ToJson()
    {
        JObject? token = null;

        if (Color is not null)
        {
            token ??= new();
            token["color"] = Color;
        }

        if (Font is not null)
        {
            token ??= new();
            token["font"] = Font;
        }

        if (Bold is not null)
        {
            token ??= new();
            token["bold"] = Bold;
        }

        if (Italic is not null)
        {
            token ??= new();
            token["italic"] = Italic;
        }

        if (Underlined is not null)
        {
            token ??= new();
            token["underlined"] = Underlined;
        }

        if (Strikethrough is not null)
        {
            token ??= new();
            token["strikethrough"] = Strikethrough;
        }

        if (Obfuscated is not null)
        {
            token ??= new();
            token["obfuscated"] = Obfuscated;
        }

        return token;
    }

    /// <summary>
    ///     Encode this style as nbt
    /// </summary>
    /// <returns></returns>
    public NbtCompound? ToNbt()
    {
        NbtCompound? nbt = null;

        if (Color is not null)
        {
            nbt ??= new();
            nbt["color"] = new NbtString("color", Color);
        }

        if (Font is not null)
        {
            nbt ??= new();
            nbt["font"] = new NbtString("font", Font);
        }

        if (Bold is not null)
        {
            nbt ??= new();
            nbt["bold"] = new NbtByte("bold", Bold.Value ? (byte)1 : (byte)0);
        }

        if (Italic is not null)
        {
            nbt ??= new();
            nbt["italic"] = new NbtByte("italic", Italic.Value ? (byte)1 : (byte)0);
        }

        if (Underlined is not null)
        {
            nbt ??= new();
            nbt["underlined"] = new NbtByte("underlined", Underlined.Value ? (byte)1 : (byte)0);
        }

        if (Strikethrough is not null)
        {
            nbt ??= new();
            nbt["strikethrough"] = new NbtByte("strikethrough", Strikethrough.Value ? (byte)1 : (byte)0);
        }

        if (Obfuscated is not null)
        {
            nbt ??= new();
            nbt["obfuscated"] = new NbtByte("obfuscated", Obfuscated.Value ? (byte)1 : (byte)0);
        }

        return nbt;
    }

    /// <summary>
    ///     Parse style from token
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Style Parse(JToken json)
    {
        if (json.Type != JTokenType.Object)
        {
            return DefaultStyle;
        }

        var style = DefaultStyle;
        var obj = (json as JObject)!;

        if (obj.TryGetValue("color", out var color))
        {
            style.Color = (string)color!;
        }

        if (obj.TryGetValue("font", out var font))
        {
            style.Font = (string)font!;
        }

        if (obj.TryGetValue("bold", out var bold))
        {
            style.Bold = (bool)bold!;
        }

        if (obj.TryGetValue("italic", out var italic))
        {
            style.Italic = (bool)italic!;
        }

        if (obj.TryGetValue("underlined", out var underlined))
        {
            style.Underlined = (bool)underlined!;
        }

        if (obj.TryGetValue("strikethrough", out var strikethrough))
        {
            style.Strikethrough = (bool)strikethrough!;
        }

        if (obj.TryGetValue("obfuscated", out var obfuscated))
        {
            style.Obfuscated = (bool)obfuscated!;
        }

        return style;
    }

    /// <summary>
    ///     Parse style from nbt
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Style Parse(NbtTag tag)
    {
        if (tag.TagType != NbtTagType.Compound)
        {
            return DefaultStyle;
        }

        var style = DefaultStyle;
        var compound = (tag as NbtCompound)!;

        if (compound.TryGet("color", out var color))
        {
            style.Color = color.StringValue;
        }

        if (compound.TryGet("font", out var font))
        {
            style.Font = font.StringValue;
        }

        if (compound.TryGet("bold", out var bold))
        {
            style.Bold = bold.ByteValue == 1;
        }

        if (compound.TryGet("italic", out var italic))
        {
            style.Italic = italic.ByteValue == 1;
        }

        if (compound.TryGet("underlined", out var underlined))
        {
            style.Underlined = underlined.ByteValue == 1;
        }

        if (compound.TryGet("strikethrough", out var strikethrough))
        {
            style.Strikethrough = strikethrough.ByteValue == 1;
        }

        if (compound.TryGet("obfuscated", out var obfuscated))
        {
            style.Obfuscated = obfuscated.ByteValue == 1;
        }

        return style;
    }
}
