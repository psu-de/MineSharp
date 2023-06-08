using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class GameEventPacket : IPacket
{
    public static int Id => 0x1F;
    
    public byte Event { get; set; }
    public float Value { get; set; }

    public GameEventPacket(byte @event, float value)
    {
        this.Event = @event;
        this.Value = value;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteByte(this.Event);
        buffer.WriteFloat(this.Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var @event = buffer.ReadByte();
        var value = buffer.ReadFloat();
        return new GameEventPacket(@event, value);
    }
}
