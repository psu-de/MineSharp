using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class GameEventPacket : IPacket
{
    public GameEventPacket(byte @event, float value)
    {
        Event = @event;
        Value = value;
    }

    public byte Event { get; set; }
    public float Value { get; set; }
    public PacketType Type => PacketType.CB_Play_GameStateChange;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(Event);
        buffer.WriteFloat(Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var @event = buffer.ReadByte();
        var value = buffer.ReadFloat();
        return new GameEventPacket(@event, value);
    }
}
#pragma warning restore CS1591
