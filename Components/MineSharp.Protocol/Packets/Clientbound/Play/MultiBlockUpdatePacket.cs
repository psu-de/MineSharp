using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class MultiBlockUpdatePacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_MultiBlockChange;

    public long ChunkSection { get; set; }
    public bool SuppressLightUpdates { get; set; }
    public long[] Blocks { get; set; }

    public MultiBlockUpdatePacket(long chunkSection, bool suppressLightUpdates, long[] blocks)
    {
        this.ChunkSection = chunkSection;
        this.SuppressLightUpdates = suppressLightUpdates;
        this.Blocks = blocks;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(this.ChunkSection);
        buffer.WriteBool(this.SuppressLightUpdates);
        buffer.WriteVarIntArray(this.Blocks, (buf, val) => buf.WriteLong(val));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var chunkSection = buffer.ReadLong();
        var suppressLightUpdates = buffer.ReadBool();
        var blocks = buffer.ReadVarIntArray(buf => buf.ReadVarLong());
        return new MultiBlockUpdatePacket(chunkSection, suppressLightUpdates, blocks);
    }
}
