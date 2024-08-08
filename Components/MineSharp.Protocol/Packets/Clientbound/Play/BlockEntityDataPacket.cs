using fNbt;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sets the block entity associated with the block at the given location.
/// </summary>
/// <param name="Location">The location of the block entity</param>
/// <param name="BlockEntityType">The type of the block entity</param>
/// <param name="NbtData">The NBT data to set</param>
public sealed record BlockEntityDataPacket(Position Location, int BlockEntityType, NbtTag? NbtData) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_TileEntityData;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WritePosition(Location);
        buffer.WriteVarInt(BlockEntityType);
        buffer.WriteOptionalNbt(NbtData);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = buffer.ReadPosition();
        var type = buffer.ReadVarInt();
        var nbtData = buffer.ReadOptionalNbt();

        return new BlockEntityDataPacket(location, type, nbtData);
    }
}
