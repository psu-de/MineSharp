using MineSharp.Data;

namespace MineSharp.Core.Serialization;

/// <summary>
///     Interface for serializing and deserializing objects from and to <see cref="PacketBuffer" />
///     while being aware of the Minecraft version
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISerializableWithMinecraftData<out T> where T : ISerializableWithMinecraftData<T>
{
    /// <summary>
    ///     Serialize the object into the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="data"></param>
    public void Write(PacketBuffer buffer, MinecraftData data);

    /// <summary>
    ///     Read the object from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static abstract T Read(PacketBuffer buffer, MinecraftData data);
}
