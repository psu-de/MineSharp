using System.Text;
using System.Text.RegularExpressions;
using fNbt;
using MineSharp.ChatComponent.Components;
using MineSharp.Data;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.ChatComponent;

/// <summary>
///     Represents a chat component
/// </summary>
public abstract partial class Chat
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///     Create a new empty chat component
    /// </summary>
    protected Chat() : this(Style.DefaultStyle)
    { }

    /// <summary>
    ///     Create a new empty chat component with a style
    /// </summary>
    /// <param name="style"></param>
    protected Chat(Style style) : this(style, [])
    { }

    /// <summary>
    ///     Create a new chat component with style and children
    /// </summary>
    /// <param name="style"></param>
    /// <param name="children"></param>
    protected Chat(Style style, Chat[] children)
    {
        Style = style;
        Children = children;
    }

    /// <summary>
    ///     The styling applied to this component
    /// </summary>
    public Style Style { get; }

    /// <summary>
    ///     The children
    /// </summary>
    public Chat[] Children { get; }

    [GeneratedRegex("§[0-9a-fk-r]")]
    private static partial Regex FormatTagRegex();

    /// <summary>
    ///     Return the message without stylecodes of this component without its children
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected abstract string GetRawMessage(MinecraftData? data);

    /// <summary>
    ///     Return the message of this component
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string GetMessage(MinecraftData? data)
    {
        return new StringBuilder(GetRawMessage(data))
              .AppendJoin(string.Empty, Children.Select(x => x.GetRawMessage(data)))
              .ToString();
    }

    /// <summary>
    ///     Encode the text component as json
    /// </summary>
    /// <returns></returns>
    protected abstract JToken EncodeJson();

    /// <summary>
    ///     Encode the text component as nbt
    /// </summary>
    /// <returns></returns>
    protected abstract NbtTag EncodeNbt();

    /// <summary>
    ///     Encode the text component as json
    /// </summary>
    /// <returns></returns>
    public JToken ToJson()
    {
        var token = EncodeJson();
        var style = Style.ToJson();

        if (token.Type != JTokenType.Object)
        {
            if (style is not null || Children.Length > 0)
            {
                Logger.Warn($"Cannot apply style or add children because encoded token is of type {token.Type}");
            }

            return token;
        }

        if (Children.Length > 0)
        {
            token["extra"] = new JArray(Children.Select(x => x.ToJson()));
        }

        if (style is null)
        {
            return token;
        }

        style.Merge(token);
        return style;
    }

    /// <summary>
    ///     Encode the text component as nbt
    /// </summary>
    /// <returns></returns>
    public NbtTag ToNbt()
    {
        var nbt = EncodeNbt();
        var style = Style.ToNbt();

        if (nbt.TagType != NbtTagType.Compound)
        {
            if (style is not null || Children.Length > 0)
            {
                Logger.Warn($"Cannot apply style or add children because encoded nbt is of type {nbt.TagType}");
            }

            return nbt;
        }

        if (Children.Length > 0)
        {
            nbt["extra"] = new NbtList("extra", Children.Select(x => x.ToNbt()));
        }

        if (style is null)
        {
            return nbt;
        }

        foreach (var tag in (nbt as NbtCompound)!.Tags)
        {
            style.Add(tag);
        }

        return style;
    }

    /// <summary>
    ///     Parse a chat component from json
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static Chat Parse(string json)
    {
        return Parse(JToken.Parse(json));
    }

    /// <summary>
    ///     Parse a chat component from json
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static Chat Parse(JToken token)
    {
        if (token.Type == JTokenType.String || token.Type == JTokenType.Array)
        {
            return TextComponent.Parse(token);
        }

        if (token.Type != JTokenType.Object)
        {
            throw new ArgumentException("Expected a string, array or object");
        }

        var obj = (token as JObject)!;
        if (obj.ContainsKey("text"))
        {
            return TextComponent.Parse(token);
        }

        if (obj.ContainsKey("translate"))
        {
            return TranslatableComponent.Parse(token);
        }

        if (obj.ContainsKey("keybind"))
        {
            return KeybindComponent.Parse(token);
        }

        throw new NotSupportedException($"This object is not supported: {token}");
    }

    /// <summary>
    ///     Parse a chat component from nbt
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static Chat Parse(NbtTag tag)
    {
        if (tag.TagType is NbtTagType.String or NbtTagType.List)
        {
            return TextComponent.Parse(tag);
        }

        if (tag.TagType != NbtTagType.Compound)
        {
            throw new ArgumentException("Expected a string, array or object");
        }

        var obj = (tag as NbtCompound)!;
        if (obj.Contains("text"))
        {
            return TextComponent.Parse(tag);
        }

        if (obj.Contains("translate"))
        {
            return TranslatableComponent.Parse(tag);
        }

        if (obj.Contains("keybind"))
        {
            return KeybindComponent.Parse(tag);
        }

        var empty = obj[""];
        if (empty is not null)
        {
            return Parse(empty);
        }

        throw new NotSupportedException($"This object is not supported: {tag}");
    }

    /// <summary>
    ///     Parse children components from token
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    protected static Chat[] ParseChildren(JToken json)
    {
        JArray array;
        if (json.Type == JTokenType.Object)
        {
            var extras = json.SelectToken("extra");
            if (extras is null)
            {
                return [];
            }

            array = (extras as JArray)!;
        }
        else
        {
            if (json.Type != JTokenType.Array)
            {
                return [];
            }

            array = (json as JArray)!;
        }

        return array.Select(Parse).ToArray();
    }

    /// <summary>
    ///     Parse children components from nbt
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    protected static Chat[] ParseChildren(NbtTag tag)
    {
        NbtList array;
        if (tag.TagType == NbtTagType.Compound)
        {
            if (!(tag as NbtCompound)!.TryGet("extra", out var extras))
            {
                return [];
            }

            array = (extras as NbtList)!;
        }
        else
        {
            if (tag.TagType != NbtTagType.List)
            {
                return [];
            }

            array = (tag as NbtList)!;
        }

        return array.Select(Parse).ToArray();
    }
}
