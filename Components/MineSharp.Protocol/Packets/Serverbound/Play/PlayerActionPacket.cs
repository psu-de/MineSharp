using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class PlayerActionPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_BlockDig;

    public int       Status     { get; set; }
    public Position  Location   { get; set; }
    public BlockFace Face       { get; set; }
    public int?      SequenceId { get; set; }

    /// <summary>
    /// Constructor for versions before 1.19
    /// </summary>
    /// <param name="status"></param>
    /// <param name="location"></param>
    /// <param name="face"></param>
    public PlayerActionPacket(int status, Position location, BlockFace face)
    {
        this.Status   = status;
        this.Location = location;
        this.Face     = face;
    }


    /// <summary>
    /// Constructor for >= 1.19
    /// </summary>
    /// <param name="status"></param>
    /// <param name="location"></param>
    /// <param name="face"></param>
    /// <param name="sequenceId"></param>
    public PlayerActionPacket(int status, Position location, BlockFace face, int? sequenceId)
    {
        this.Status     = status;
        this.Location   = location;
        this.Face       = face;
        this.SequenceId = sequenceId;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.Status);
        buffer.WriteULong(this.Location.ToULong());
        buffer.WriteByte((byte)this.Face);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
            buffer.WriteVarInt(this.SequenceId!.Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
            return new PlayerActionPacket(
                buffer.ReadVarInt(),
                new Position(buffer.ReadULong()),
                (BlockFace)buffer.ReadByte(),
                buffer.ReadVarInt());

        return new PlayerActionPacket(
            buffer.ReadVarInt(),
            new Position(buffer.ReadULong()),
            (BlockFace)buffer.ReadByte());
    }
}
#pragma warning restore CS1591
