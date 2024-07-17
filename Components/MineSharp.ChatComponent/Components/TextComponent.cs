using fNbt;
using MineSharp.Data;
using Newtonsoft.Json.Linq;

namespace MineSharp.ChatComponent.Components;

/// <summary>
/// Represents a text component
/// </summary>
public class TextComponent : Chat
{
    /// <summary>
    /// The text of this component
    /// </summary>
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// Create a new TextComponent
    /// </summary>
    /// <param name="text"></param>
    public TextComponent(string text)
    {
        this.Text = text;
    }

    /// <summary>
    /// Create a new TextComponent
    /// </summary>
    /// <param name="text"></param>
    /// <param name="style"></param>
    public TextComponent(string text, Style style) : base(style)
    {
        this.Text = text;
    }

    /// <summary>
    /// Create a new TextComponent
    /// </summary>
    /// <param name="text"></param>
    /// <param name="style"></param>
    /// <param name="children"></param>
    public TextComponent(string text, Style style, Chat[] children) : base(style, children)
    {
        this.Text = text;
    }
    
    /// <summary>
    /// Create a new TextComponent
    /// </summary>
    /// <param name="children"></param>
    public TextComponent(Chat[] children) : base(Style.DefaultStyle, children) { }


    /// <inheritdoc />
    protected override JToken EncodeJson()
    {
        return new JObject { ["text"] = this.Text };
    }

    /// <inheritdoc />
    protected override NbtTag EncodeNbt()
    {
        return new NbtCompound { ["text"] = new NbtString("text", this.Text) };
    }

    /// <inheritdoc />
    protected override string GetRawMessage(MinecraftData? data)
        => this.Text;

    internal static new TextComponent Parse(JToken token)
    {
        if (token.Type == JTokenType.String)
            return new TextComponent((string)token!);

        if (token.Type == JTokenType.Array)
            return new TextComponent(ParseChildren(token));

        if (token.Type != JTokenType.Object)
            throw new ArgumentException($"token must be string, array or object, got {token.Type}");
        
        // TODO: Parse style codes from raw text and add it to style
        var textToken = token.SelectToken("text")!;
        return new TextComponent((string)textToken!, Style.Parse(token), ParseChildren(token));
    }
    
    internal static new TextComponent Parse(NbtTag tag)
    {
        if (tag.TagType == NbtTagType.String)
            return new TextComponent(tag.StringValue!);

        if (tag.TagType == NbtTagType.List)
            return new TextComponent(ParseChildren(tag));

        if (tag.TagType != NbtTagType.Compound)
            throw new ArgumentException($"token must be string, array or object, got {tag.TagType}");

        // TODO: Parse style codes from raw text and add it to style
        var textToken = tag["text"];
        return new TextComponent(textToken.StringValue!, Style.Parse(tag), ParseChildren(tag));
    }
}
