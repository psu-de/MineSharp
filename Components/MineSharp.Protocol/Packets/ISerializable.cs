﻿using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets;

/// <summary>
///     Interface for serializing and deserializing objects from and to <see cref="PacketBuffer" />
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISerializable<out T> : IVersionAwareSerializable<T>
    where T : ISerializable<T>
{
    /// <summary>
    ///     Serialize the object into the buffer
    /// </summary>
    /// <param name="buffer"></param>
    public void Write(PacketBuffer buffer);

    /// <inheritdoc />
    void IVersionAwareSerializable<T>.Write(PacketBuffer buffer, MinecraftData version)
    {
        Write(buffer);
    }

    /// <summary>
    ///     Read the object from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static abstract T Read(PacketBuffer buffer);

    /// <inheritdoc />
    static T IVersionAwareSerializable<T>.Read(PacketBuffer buffer, MinecraftData version)
    {
        return T.Read(buffer);
    }
}
