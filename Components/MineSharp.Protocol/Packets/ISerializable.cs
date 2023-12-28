using MineSharp.Core.Common;

namespace MineSharp.Protocol.Packets;

/// <summary>
/// Interface for serializing and deserializing objects from and to <see cref="PacketBuffer"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISerializable<out T> where T : ISerializable<T>
{
    /// <summary>
    /// Serialize the object into the buffer
    /// </summary>
    /// <param name="buffer"></param>
    public void Write(PacketBuffer buffer);

    /// <summary>
    /// Read the object from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public abstract static T Read(PacketBuffer buffer);
}
