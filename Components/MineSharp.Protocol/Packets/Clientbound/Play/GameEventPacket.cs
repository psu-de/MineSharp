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
public sealed partial record GameEventPacket(GameEvent Event, float Value) : IPacketStatic<GameEventPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_GameStateChange;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteByte((byte)Event);
        buffer.WriteFloat(Value);
    }

    /// <inheritdoc />
    public static GameEventPacket Read(PacketBuffer buffer, MinecraftData data)
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
