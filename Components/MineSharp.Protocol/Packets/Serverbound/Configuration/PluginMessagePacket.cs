﻿using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;

/// <summary>
///     Plugin message packet
/// </summary>
/// <param name="ChannelName">The name of the channel</param>
/// <param name="Data">The data of the plugin message</param>
public sealed record PluginMessagePacket(string ChannelName, byte[] Data) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Configuration_CustomPayload;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(ChannelName);
        buffer.WriteBytes(Data);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var channelName = buffer.ReadString();
        var data = buffer.RestBuffer();
        return new PluginMessagePacket(channelName, data);
    }
}
