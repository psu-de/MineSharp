using MineSharp.Core;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record PlayerActionPacket : IPacketStatic<PlayerActionPacket>
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

    public int Status { get; init; }
    public Position Location { get; init; }
    public BlockFace Face { get; init; }
    public int? SequenceId { get; init; }

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(Status);
        buffer.WritePosition(Location);
        buffer.WriteByte((byte)Face);

        if (data.Version.Protocol >= ProtocolVersion.V_1_19_0)
        {
            buffer.WriteVarInt(SequenceId!.Value);
        }
    }

    public static PlayerActionPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        if (data.Version.Protocol >= ProtocolVersion.V_1_19_0)
        {
            return new PlayerActionPacket(
                buffer.ReadVarInt(),
                buffer.ReadPosition(),
                (BlockFace)buffer.ReadByte(),
                buffer.ReadVarInt());
        }

        return new PlayerActionPacket(
            buffer.ReadVarInt(),
            buffer.ReadPosition(),
            (BlockFace)buffer.ReadByte());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591
