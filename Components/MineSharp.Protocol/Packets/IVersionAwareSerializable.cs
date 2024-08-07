﻿using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets;

/// <summary>
///     Interface for serializing and deserializing objects from and to <see cref="PacketBuffer" />
///     while being aware of the Minecraft version
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IVersionAwareSerializable<out T> where T : IVersionAwareSerializable<T>
{
    /// <summary>
    ///     Serialize the object into the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="version"></param>
    public void Write(PacketBuffer buffer, MinecraftData version);

    /// <summary>
    ///     Read the object from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static abstract T Read(PacketBuffer buffer, MinecraftData version);
}