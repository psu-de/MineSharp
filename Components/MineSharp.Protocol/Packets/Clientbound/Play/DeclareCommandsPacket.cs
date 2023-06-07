using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class DeclareCommandsPacket : IPacket
{
    public static int Id => 0x10;

    public PacketBuffer RawBuffer { get; set; }

    public DeclareCommandsPacket(PacketBuffer buffer)
    {
        this.RawBuffer = buffer;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteBytes(this.RawBuffer.GetBuffer());
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var clone = new PacketBuffer(buffer.ReadBytes((int)buffer.ReadableBytes));
        return new DeclareCommandsPacket(clone);
    }
}
