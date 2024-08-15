using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets;

public interface IPacketVersionSubType
{
    public ProtocolVersion FirstVersionUsed { get; }
}

public interface IPacketVersionSubType<TBasePacket> : IPacketVersionSubType
    where TBasePacket : IPacketStatic<TBasePacket>
{
}

public interface IPacketVersionSubTypeStatic : IPacketVersionSubType
{
    public static abstract ProtocolVersion FirstVersionUsedStatic { get; }

    /// <summary>
    ///     Read the packet from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static abstract IPacket Read(PacketBuffer buffer, MinecraftData data);
}

public interface IPacketVersionSubTypeStatic<TBasePacket> : IPacketVersionSubTypeStatic
    where TBasePacket : IPacketStatic<TBasePacket>
{
    /// <summary>
    ///     Read the packet from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static new abstract TBasePacket Read(PacketBuffer buffer, MinecraftData data);
}

public interface IPacketVersionSubTypeStatic<TSelf, TBasePacket> : IPacketVersionSubTypeStatic<TBasePacket>, ISerializableWithMinecraftData<TSelf>
    where TSelf : IPacketVersionSubTypeStatic<TSelf, TBasePacket>
    where TBasePacket : IPacketStatic<TBasePacket>
{
}
