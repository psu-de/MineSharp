using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class MultiBlockUpdatePacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_MultiBlockChange;

    public long   ChunkSection         { get; set; }
    public bool?  SuppressLightUpdates { get; set; }
    public long[] Blocks               { get; set; }

    /// <summary>
    /// Constructor for before 1.20
    /// </summary>
    /// <param name="chunkSection"></param>
    /// <param name="suppressLightUpdates"></param>
    /// <param name="blocks"></param>
    public MultiBlockUpdatePacket(long chunkSection, bool? suppressLightUpdates, long[] blocks)
    {
        this.ChunkSection         = chunkSection;
        this.SuppressLightUpdates = suppressLightUpdates;
        this.Blocks               = blocks;
    }

    /// <summary>
    /// Constructor >= 1.20
    /// </summary>
    /// <param name="chunkSection"></param>
    /// <param name="blocks"></param>
    public MultiBlockUpdatePacket(long chunkSection, long[] blocks)
    {
        this.ChunkSection         = chunkSection;
        this.SuppressLightUpdates = null;
        this.Blocks               = blocks;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol < ProtocolVersion.V_1_20 && this.SuppressLightUpdates == null)
            throw new ArgumentNullException(nameof(this.SuppressLightUpdates));

        buffer.WriteLong(this.ChunkSection);
        if (version.Version.Protocol < ProtocolVersion.V_1_20)
            buffer.WriteBool(this.SuppressLightUpdates!.Value);
        buffer.WriteVarIntArray(this.Blocks, (buf, val) => buf.WriteLong(val));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var   chunkSection         = buffer.ReadLong();
        bool? suppressLightUpdates = null;
        if (version.Version.Protocol < ProtocolVersion.V_1_20)
            suppressLightUpdates = buffer.ReadBool();
        var blocks = buffer.ReadVarIntArray(buf => buf.ReadVarLong());
        return new MultiBlockUpdatePacket(chunkSection, suppressLightUpdates, blocks);
    }
}
#pragma warning restore CS1591
