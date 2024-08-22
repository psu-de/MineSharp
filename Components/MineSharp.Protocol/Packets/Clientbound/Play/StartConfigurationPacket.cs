﻿using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sent during gameplay in order to redo the configuration process.
///     The client must respond with Acknowledge Configuration for the process to start.
/// </summary>
public sealed partial record StartConfigurationPacket() : IPacketStatic<StartConfigurationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_StartConfiguration;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        // No fields to write
    }

    /// <inheritdoc />
    public static StartConfigurationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        // No fields to read
        return new StartConfigurationPacket();
    }
}
