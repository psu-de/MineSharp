using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

public class SetCompressionPacket : IPacket
{
    public PacketType Type => PacketType.CB_Login_Compress;
    
    public int Threshold { get; set; }

    public SetCompressionPacket(int threshold)
    {
        this.Threshold = threshold;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.Threshold);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var threshold = buffer.ReadVarInt();
        return new SetCompressionPacket(threshold);
    }
}
