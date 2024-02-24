using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class GameEventPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_GameStateChange;

    public byte  Event { get; set; }
    public float Value { get; set; }

    public GameEventPacket(byte @event, float value)
    {
        this.Event = @event;
        this.Value = value;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(this.Event);
        buffer.WriteFloat(this.Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var @event = buffer.ReadByte();
        var value  = buffer.ReadFloat();
        return new GameEventPacket(@event, value);
    }
}
#pragma warning restore CS1591
