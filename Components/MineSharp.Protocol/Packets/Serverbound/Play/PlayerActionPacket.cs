using MineSharp.Core;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record PlayerActionPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_BlockDig;

    // Here is no non-argument constructor allowed
    // Do not use
#pragma warning disable CS8618
    private PlayerActionPacket()
#pragma warning restore CS8618
    {
    }

    /// <summary>
    ///     Constructor for versions before 1.19
    /// </summary>
    /// <param name="status"></param>
    /// <param name="location"></param>
    /// <param name="face"></param>
    public PlayerActionPacket(PlayerActionStatus status, Position location, BlockFace face)
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
    public PlayerActionPacket(PlayerActionStatus status, Position location, BlockFace face, int? sequenceId)
    {
        Status = status;
        Location = location;
        Face = face;
        SequenceId = sequenceId;
    }

    public PlayerActionStatus Status { get; init; }
    public Position Location { get; init; }
    public BlockFace Face { get; init; }
    public int? SequenceId { get; init; }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)Status);
        buffer.WritePosition(Location);
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
                (PlayerActionStatus)buffer.ReadVarInt(),
                buffer.ReadPosition(),
                (BlockFace)buffer.ReadByte(),
                buffer.ReadVarInt());
        }

        return new PlayerActionPacket(
            (PlayerActionStatus)buffer.ReadVarInt(),
            buffer.ReadPosition(),
            (BlockFace)buffer.ReadByte());
    }
}
#pragma warning restore CS1591
