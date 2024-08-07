﻿using fNbt;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Registry data packet
///     See https://wiki.vg/Protocol#Registry_Data
/// </summary>
/// <param name="RegistryData">The registry data</param>
public sealed record RegistryDataPacket(NbtCompound RegistryData) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_RegistryData;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteNbt(RegistryData);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new RegistryDataPacket(buffer.ReadNbtCompound());
    }
}

