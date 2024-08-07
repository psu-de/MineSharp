﻿using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Block update packet
/// </summary>
/// <param name="Location">The location of the block update</param>
/// <param name="StateId">The new state id</param>
public sealed record BlockUpdatePacket(Position Location, int StateId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_BlockChange;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteULong(Location.ToULong());
        buffer.WriteVarInt(StateId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = new Position(buffer.ReadULong());
        var stateId = buffer.ReadVarInt();
        return new BlockUpdatePacket(location, stateId);
    }
}
