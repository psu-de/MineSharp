using fNbt;
using MineSharp.Data;
using Newtonsoft.Json.Linq;

namespace MineSharp.ChatComponent.Components;

/// <summary>
///     Represents a KeybindComponent
/// </summary>
public class KeybindComponent : Chat
{
    /// <summary>
    ///     Create a new KeybindComponent instance
    /// </summary>
    /// <param name="key"></param>
    public KeybindComponent(string key)
    {
        Key = key;
    }

    /// <summary>
    ///     Create a new KeybindComponent instance
    /// </summary>
    public KeybindComponent(string key, Style style, Chat[] children) : base(style, children)
    {
        Key = key;
    }

    /// <summary>
    ///     The name of the Key
    /// </summary>
    public string Key { get; set; }

    /// <inheritdoc />
    protected override JToken EncodeJson()
    {
        return new JObject { ["key"] = Key };
    }

    /// <inheritdoc />
    protected override NbtTag EncodeNbt()
    {
        return new NbtCompound { ["key"] = new NbtString("key", Key) };
    }

    /// <inheritdoc />
    protected override string GetRawMessage(MinecraftData? data)
    {
        return Key;
    }

    internal static new KeybindComponent Parse(JToken token)
    {
        if (token.Type != JTokenType.Object)
        {
            throw new ArgumentException($"Expected token of type object, got {token.Type}");
        }

        return new((string)token.SelectToken("keybind")!);
    }

    internal static new KeybindComponent Parse(NbtTag tag)
    {
        if (tag.TagType != NbtTagType.Compound)
        {
            throw new ArgumentException($"Expected TAG_Compound, got {tag.TagType}");
        }

        return new((tag as NbtCompound)!.Get("keybind")!.StringValue);
    }
}
