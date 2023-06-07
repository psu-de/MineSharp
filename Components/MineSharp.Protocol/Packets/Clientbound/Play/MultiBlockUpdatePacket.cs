using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class MultiBlockUpdatePacket : IPacket
{
    public static int Id => 0x43;

    public long ChunkSection { get; set; }
    public bool SuppressLightUpdates { get; set; }
    public long[] Blocks { get; set; }

    public MultiBlockUpdatePacket(long chunkSection, bool suppressLightUpdates, long[] blocks)
    {
        this.ChunkSection = chunkSection;
        this.SuppressLightUpdates = suppressLightUpdates;
        this.Blocks = blocks;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteLong(this.ChunkSection);
        buffer.WriteBool(this.SuppressLightUpdates);
        buffer.WriteVarIntArray(this.Blocks, (buf, val) => buf.WriteLong(val));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var chunkSection = buffer.ReadLong();
        var suppressLightUpdates = buffer.ReadBool();
        var blocks = buffer.ReadVarIntArray(buf => buf.ReadLong());
        return new MultiBlockUpdatePacket(chunkSection, suppressLightUpdates, blocks);
    }
}
