using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class DeclareCommandsPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_DeclareCommands;

    public PacketBuffer RawBuffer { get; set; }

    public DeclareCommandsPacket(PacketBuffer buffer)
    {
        this.RawBuffer = buffer;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteBytes(this.RawBuffer.GetBuffer());
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var clone = new PacketBuffer(buffer.ReadBytes((int)buffer.ReadableBytes));
        return new DeclareCommandsPacket(clone);
    }
}
