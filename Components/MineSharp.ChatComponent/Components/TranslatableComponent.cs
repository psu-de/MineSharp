﻿using System.Text.RegularExpressions;
using fNbt;
using MineSharp.Data;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.ChatComponent.Components;

/// <summary>
///     Represents a translatable component
/// </summary>
public class TranslatableComponent : Chat
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


    /// <summary>
    ///     Create a new translatable component
    /// </summary>
    /// <param name="translation"></param>
    public TranslatableComponent(string translation)
    {
        Translation = translation;
        With = [];
    }

    /// <summary>
    ///     Create a new translatable component
    /// </summary>
    /// <param name="translation"></param>
    /// <param name="with"></param>
    public TranslatableComponent(string translation, Chat[] with) : this(translation)
    {
        With = with;
    }

    /// <summary>
    ///     Create a new translatable component
    /// </summary>
    /// <param name="translation"></param>
    /// <param name="with"></param>
    /// <param name="style"></param>
    public TranslatableComponent(string translation, Chat[] with, Style style) : base(style)
    {
        Translation = translation;
        With = with;
    }

    /// <summary>
    ///     Create a new translatable component
    /// </summary>
    /// <param name="translation"></param>
    /// <param name="with"></param>
    /// <param name="style"></param>
    /// <param name="children"></param>
    public TranslatableComponent(string translation, Chat[] with, Style style, Chat[] children) : base(style, children)
    {
        Translation = translation;
        With = with;
    }

    /// <summary>
    ///     The name of the translation rule
    /// </summary>
    public string Translation { get; set; }

    /// <summary>
    ///     The arguments for the translation rule
    /// </summary>
    public Chat[] With { get; set; }

    /// <inheritdoc />
    protected override JToken EncodeJson()
    {
        var obj = new JObject { ["translate"] = Translation };

        if (With.Length > 0)
        {
            obj["with"] = new JArray(With.Select(x => x.ToJson()));
        }

        return obj;
    }

    /// <inheritdoc />
    protected override NbtTag EncodeNbt()
    {
        var nbt = new NbtCompound { ["translate"] = new NbtString("translate", Translation) };

        if (With.Length > 0)
        {
            nbt["with"] = new NbtList("with", With.Select(x => x.ToNbt()));
        }

        return nbt;
    }

    /// <inheritdoc />
    protected override string GetRawMessage(MinecraftData? data)
    {
        var with = With.Select(x => x.GetMessage(data)).ToArray();
        if (data == null)
        {
            Logger.Warn("Cannot translate message because no minecraft data was provided! For: {TranslationKey}", Translation);
            return string.Join(' ', with);
        }

        var rule = data.Language.GetTranslation(Translation)!;
        if (rule == null)
        {
            Logger.Warn("Cannot translate message because no translation string was found! For: {TranslationKey}", Translation);
            return string.Join(' ', with);
        }
        return TranslateString(rule, with);
    }

    private string TranslateString(string rule, string[] usings)
    {
        var usingIndex = 0;
        var result = Regex.Replace(rule, "%s", match => usings[usingIndex++]);
        result = Regex.Replace(result, "%(\\d)\\$s", match =>
        {
            var idx = match.Groups[1].Value[0] - '1';
            return usings[idx];
        });
        return result;
    }

    internal static new TranslatableComponent Parse(JToken token)
    {
        if (token.Type != JTokenType.Object)
        {
            throw new ArgumentException($"Expected token of type object, got {token.Type}");
        }

        var obj = (token as JObject)!;
        var translate = (string)obj.GetValue("translate")!;
        var with = Array.Empty<Chat>();
        if (obj.TryGetValue("with", out var withToken))
        {
            with = withToken.Select(Chat.Parse).ToArray();
        }

        return new(translate, with, Style.Parse(token), ParseChildren(token));
    }

    internal static new TranslatableComponent Parse(NbtTag tag)
    {
        if (tag.TagType != NbtTagType.Compound)
        {
            throw new ArgumentException($"Expected TAG_Compound, got {tag.TagType}");
        }

        var obj = (tag as NbtCompound)!;
        var translate = obj.Get("translate")!.StringValue;
        var with = Array.Empty<Chat>();
        if (obj.TryGet("with", out var withToken))
        {
            with = ParseArguments(withToken!);
        }

        return new(translate, with, Style.Parse(tag), ParseChildren(tag));
    }

    private static Chat[] ParseArguments(NbtTag tag)
    {
        // as of 1.20.3 and the introduction of chat components via nbt,
        // TranslatableComponents allow arguments to be the primitive types
        // of java.lang.Number (byte, double, float, integer, ...), boolean and strings
        
        // because NbtLists only allow elements of the same tag, these primitives types
        // are sometimes wrapped in a TAG_Compound

        return tag switch
        {
            NbtLongArray longArray => longArray.Value.Select(Chat (x) => new TextComponent(x.ToString())).ToArray(),

            NbtIntArray intArray => intArray.Value.Select(Chat (x) => new TextComponent(x.ToString())).ToArray(),

            NbtByteArray byteArray => byteArray.Value.Select(Chat (x) => new TextComponent(x.ToString())).ToArray(),

            NbtList list => list.Select(ParseArgument).ToArray(),
            
            _ => throw new ArgumentException($"Unexpected tag type: {tag.TagType}")
        };
    }

    private static Chat ParseArgument(NbtTag tag)
    {
        if (tag.TagType != NbtTagType.Compound)
        {
            return new TextComponent(tag.StringValue);
        }
        
        var compound = (tag as NbtCompound)!;
        if (compound.Count == 1 && compound.TryGet("", out var primitive))
        {
            // it's a primitive value wrapped in a compound
            return new TextComponent(primitive.StringValue);
        }
        
        // it's a normal chat component
        return Chat.Parse(compound);
    }
}
