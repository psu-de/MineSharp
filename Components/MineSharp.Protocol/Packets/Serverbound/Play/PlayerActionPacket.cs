using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class PlayerActionPacket : IPacket
{
    /// <summary>
    ///     Constructor for versions before 1.19
    /// </summary>
    /// <param name="status"></param>
    /// <param name="location"></param>
    /// <param name="face"></param>
    public PlayerActionPacket(int status, Position location, BlockFace face)
    {
        Status = status;
        Location = location;
        Face = face;
    }


    /// <summary>
    ///     Constructor for >= 1.19
    /// </summary>
    /// <param name="status"></param>
    /// <param name="location"></param>
    /// <param name="face"></param>
    /// <param name="sequenceId"></param>
    public PlayerActionPacket(int status, Position location, BlockFace face, int? sequenceId)
    {
        Status = status;
        Location = location;
        Face = face;
        SequenceId = sequenceId;
    }

    public int Status { get; set; }
    public Position Location { get; set; }
    public BlockFace Face { get; set; }
    public int? SequenceId { get; set; }
    public PacketType Type => PacketType.SB_Play_BlockDig;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(Status);
        buffer.WriteULong(Location.ToULong());
        buffer.WriteByte((byte)Face);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            buffer.WriteVarInt(SequenceId!.Value);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            return new PlayerActionPacket(
                buffer.ReadVarInt(),
                new(buffer.ReadULong()),
                (BlockFace)buffer.ReadByte(),
                buffer.ReadVarInt());
        }

        return new PlayerActionPacket(
            buffer.ReadVarInt(),
            new(buffer.ReadULong()),
            (BlockFace)buffer.ReadByte());
    }
}
#pragma warning restore CS1591
