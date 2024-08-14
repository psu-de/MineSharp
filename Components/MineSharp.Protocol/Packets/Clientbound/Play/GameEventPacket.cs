using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.GameEventPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     Game event packet
/// </summary>
/// <param name="Event">The game event</param>
/// <param name="Value">The value associated with the event</param>
public sealed record GameEventPacket(GameEvent Event, float Value) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_GameStateChange;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte((byte)Event);
        buffer.WriteFloat(Value);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var @event = buffer.ReadByte();
        var value = buffer.ReadFloat();
        return new GameEventPacket((GameEvent)@event, value);
    }

    /// <summary>
    ///     All the possible game events
    /// </summary>
    public enum GameEvent : byte
    {
        NoRespawnBlockAvailable,
        EndRaining,
        BeginRaining,
        ChangeGameMode,
        WinGame,
        DemoEvent,
        ArrowHitPlayer,
        RainLevelChange,
        ThunderLevelChange,
        PlayPufferfishStingSound,
        PlayElderGuardianMobAppearance,
        EnableRespawnScreen,
        LimitedCrafting,
        StartWaitingForLevelChunks
    }
}

#pragma warning restore CS1591
