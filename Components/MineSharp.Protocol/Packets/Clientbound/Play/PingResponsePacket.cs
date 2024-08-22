﻿using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Ping response packet
/// </summary>
/// <param name="Payload">The payload, should be the same as sent by the client</param>
public sealed partial record PingResponsePacket(long Payload) : IPacketStatic<PingResponsePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_PingResponse;

    /// <inheritdoc/>
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(Payload);
    }

    /// <inheritdoc/>
    public static PingResponsePacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var payload = buffer.ReadLong();

        return new(payload);
    }
}
