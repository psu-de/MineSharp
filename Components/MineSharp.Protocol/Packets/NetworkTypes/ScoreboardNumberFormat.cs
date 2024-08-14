using System.Collections.Frozen;
using fNbt;
using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;

namespace MineSharp.Protocol.Packets.NetworkTypes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
/// <summary>
///     Scoreboard number format type enumeration
/// </summary>
public enum ScoreboardNumberFormatType
{
    Blank = 0,
    Styled = 1,
    Fixed = 2
}

public interface IScoreboardNumberFormat
{
    public ScoreboardNumberFormatType Type { get; }

    public void Write(PacketBuffer buffer);
}

public interface IScoreboardNumberFormatStatic
{
    public static abstract ScoreboardNumberFormatType StaticType { get; }

    public static abstract IScoreboardNumberFormat Read(PacketBuffer buffer);
}

public static class ScoreboardNumberFormatRegistry
{
    public static IScoreboardNumberFormat Read(PacketBuffer buffer, ScoreboardNumberFormatType type)
    {
        if (!ScoreboardNumberFormatFactories.TryGetValue(type, out var reader))
        {
            throw new InvalidOperationException($"Unsupported UpdateTeamsMethodType: {type}");
        }
        return reader(buffer);
    }

    public static readonly FrozenDictionary<ScoreboardNumberFormatType, Func<PacketBuffer, IScoreboardNumberFormat>> ScoreboardNumberFormatFactories;

    static ScoreboardNumberFormatRegistry()
    {
        ScoreboardNumberFormatFactories = InitializeUpdateTeamsMethod();
    }

    private static FrozenDictionary<ScoreboardNumberFormatType, Func<PacketBuffer, IScoreboardNumberFormat>> InitializeUpdateTeamsMethod()
    {
        var dict = new Dictionary<ScoreboardNumberFormatType, Func<PacketBuffer, IScoreboardNumberFormat>>();

        void Register<T>()
            where T : IScoreboardNumberFormat, IScoreboardNumberFormatStatic
        {
            var mask = T.StaticType;
            var factory = T.Read;
            dict.Add(mask, factory);
        }

        Register<BlankNumberFormat>();
        Register<StyledNumberFormat>();
        Register<FixedNumberFormat>();

        return dict.ToFrozenDictionary();
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
///     Blank number format
/// </summary>
public sealed record BlankNumberFormat() : IScoreboardNumberFormat, IScoreboardNumberFormatStatic, ISerializable<BlankNumberFormat>
{
    /// <inheritdoc />
    public ScoreboardNumberFormatType Type => StaticType;
    /// <inheritdoc />
    public static ScoreboardNumberFormatType StaticType => ScoreboardNumberFormatType.Blank;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer) { }

    /// <inheritdoc />
    public static BlankNumberFormat Read(PacketBuffer buffer) => new();

    static IScoreboardNumberFormat IScoreboardNumberFormatStatic.Read(PacketBuffer buffer)
    {
        return Read(buffer);
    }
}

/// <summary>
///     Styled number format
/// </summary>
/// <param name="Styling">The styling to be used when formatting the score number</param>
public sealed record StyledNumberFormat(NbtCompound Styling) : IScoreboardNumberFormat, IScoreboardNumberFormatStatic, ISerializable<StyledNumberFormat>
{
    /// <inheritdoc />
    public ScoreboardNumberFormatType Type => StaticType;
    /// <inheritdoc />
    public static ScoreboardNumberFormatType StaticType => ScoreboardNumberFormatType.Blank;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer)
    {
        buffer.WriteNbt(Styling);
    }

    /// <inheritdoc />
    public static StyledNumberFormat Read(PacketBuffer buffer)
    {
        var styling = buffer.ReadNbtCompound();
        return new(styling);
    }

    static IScoreboardNumberFormat IScoreboardNumberFormatStatic.Read(PacketBuffer buffer)
    {
        return Read(buffer);
    }
}

/// <summary>
///     Fixed number format
/// </summary>
/// <param name="Content">The text to be used as placeholder</param>
public sealed record FixedNumberFormat(Chat Content) : IScoreboardNumberFormat, IScoreboardNumberFormatStatic, ISerializable<FixedNumberFormat>
{
    /// <inheritdoc />
    public ScoreboardNumberFormatType Type => StaticType;
    /// <inheritdoc />
    public static ScoreboardNumberFormatType StaticType => ScoreboardNumberFormatType.Blank;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer)
    {
        buffer.WriteChatComponent(Content);
    }

    /// <inheritdoc />
    public static FixedNumberFormat Read(PacketBuffer buffer)
    {
        var content = buffer.ReadChatComponent();
        return new(content);
    }

    static IScoreboardNumberFormat IScoreboardNumberFormatStatic.Read(PacketBuffer buffer)
    {
        return Read(buffer);
    }
}
