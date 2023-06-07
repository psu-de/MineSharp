using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

public class SetCompressionPacket : IPacket
{
    public static int Id => 0x03;
    
    public int Threshold { get; set; }

    public SetCompressionPacket(int threshold)
    {
        this.Threshold = threshold;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.Threshold);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var threshold = buffer.ReadVarInt();
        return new SetCompressionPacket(threshold);
    }
}
