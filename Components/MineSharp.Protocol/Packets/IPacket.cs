﻿using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets;

/// <summary>
///     Represents a Minecraft packet
/// </summary>
public interface IPacket
{
    /// <summary>
    ///     The corresponding <see cref="PacketType" />
    ///     
    /// <seealso cref="IPacketStatic.StaticType"/>
    /// </summary>
    public PacketType Type { get; }

    /// <summary>
    ///     Serialize the packet data into the buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="data"></param>
    public void Write(PacketBuffer buffer, MinecraftData data);
}

/// <summary>
///     Represents a Minecraft packet
/// </summary>
public interface IPacketStatic : IPacket
{
    /// <summary>
    ///     The corresponding <see cref="PacketType" />.
    ///     The same as <see cref="Type" /> but static.
    /// </summary>
    public static abstract PacketType StaticType { get; }

    /// <summary>
    ///     Read the packet from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static abstract IPacket Read(PacketBuffer buffer, MinecraftData data);
}

/// <summary>
///     Represents a Minecraft packet
/// </summary>
public interface IPacketStatic<TSelf> : IPacketStatic, ISerializableWithMinecraftData<TSelf>
    where TSelf : IPacketStatic<TSelf>
{
}

/// <summary>
///     Represents a Minecraft packet that is sent from the server to the client
/// </summary>
public interface IPacketClientbound : IPacket
{
}

/// <summary>
///     Represents a Minecraft packet that is sent from the client to the server
/// </summary>
public interface IPacketServerbound : IPacket
{
}

public delegate TPacket PacketFactory<out TPacket>(PacketBuffer buffer, MinecraftData data)
    where TPacket : IPacket;

public delegate IPacket PacketFactory(PacketBuffer buffer, MinecraftData data);
