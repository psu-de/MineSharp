using fNbt;
using MineSharp.Data;
using Newtonsoft.Json.Linq;

namespace MineSharp.ChatComponent.Components;

/// <summary>
///     Represents a text component
/// </summary>
public class TextComponent : Chat
{
    /// <summary>
    ///     Create a new TextComponent
    /// </summary>
    /// <param name="text"></param>
    public TextComponent(string text)
    {
        Text = text;
    }

    /// <summary>
    ///     Create a new TextComponent
    /// </summary>
    /// <param name="text"></param>
    /// <param name="style"></param>
    public TextComponent(string text, Style style) : base(style)
    {
        Text = text;
    }

    /// <summary>
    ///     Create a new TextComponent
    /// </summary>
    /// <param name="text"></param>
    /// <param name="style"></param>
    /// <param name="children"></param>
    public TextComponent(string text, Style style, Chat[] children) : base(style, children)
    {
        Text = text;
    }

    /// <summary>
    ///     Create a new TextComponent
    /// </summary>
    /// <param name="children"></param>
    public TextComponent(Chat[] children) : base(Style.DefaultStyle, children) { }

    /// <summary>
    ///     The text of this component
    /// </summary>
    public string Text { get; set; } = string.Empty;


    /// <inheritdoc />
    protected override JToken EncodeJson()
    {
        return new JObject { ["text"] = Text };
    }

    /// <inheritdoc />
    protected override NbtTag EncodeNbt()
    {
        return new NbtCompound { ["text"] = new NbtString("text", Text) };
    }

    /// <inheritdoc />
    protected override string GetRawMessage(MinecraftData? data)
    {
        return Text;
    }

    internal static new TextComponent Parse(JToken token)
    {
        if (token.Type == JTokenType.String)
        {
            return new((string)token!);
        }

        if (token.Type == JTokenType.Array)
        {
            return new(ParseChildren(token));
        }

        if (token.Type != JTokenType.Object)
        {
            throw new ArgumentException($"token must be string, array or object, got {token.Type}");
        }

        // TODO: Parse style codes from raw text and add it to style
        var textToken = token.SelectToken("text")!;
        return new((string)textToken!, Style.Parse(token), ParseChildren(token));
    }

    internal static new TextComponent Parse(NbtTag tag)
    {
        if (tag.TagType == NbtTagType.String)
        {
            return new(tag.StringValue!);
        }

        if (tag.TagType == NbtTagType.List)
        {
            return new(ParseChildren(tag));
        }

        if (tag.TagType != NbtTagType.Compound)
        {
            throw new ArgumentException($"token must be string, array or object, got {tag.TagType}");
        }

        // TODO: Parse style codes from raw text and add it to style
        var textToken = tag["text"];
        return new(textToken.StringValue!, Style.Parse(tag), ParseChildren(tag));
    }
}
