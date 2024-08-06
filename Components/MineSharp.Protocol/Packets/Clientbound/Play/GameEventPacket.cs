using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class GameEventPacket : IPacket
{
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

    public GameEventPacket(GameEvent @event, float value)
    {
        Event = @event;
        Value = value;
    }

    public GameEvent Event { get; set; }
    public float Value { get; set; }
    public PacketType Type => PacketType.CB_Play_GameStateChange;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte((byte)Event);
        buffer.WriteFloat(Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var @event = buffer.ReadByte();
        var value = buffer.ReadFloat();
        return new GameEventPacket((GameEvent)@event, value);
    }
}
#pragma warning restore CS1591
