﻿using fNbt;
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
public sealed partial record BlockEntityDataPacket(Position Location, int BlockEntityType, NbtTag? NbtData) : IPacketStatic<BlockEntityDataPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_TileEntityData;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WritePosition(Location);
        buffer.WriteVarInt(BlockEntityType);
        buffer.WriteOptionalNbt(NbtData);
    }

    /// <inheritdoc />
    public static BlockEntityDataPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var location = buffer.ReadPosition();
        var type = buffer.ReadVarInt();
        var nbtData = buffer.ReadOptionalNbt();

        return new BlockEntityDataPacket(location, type, nbtData);
    }
}
