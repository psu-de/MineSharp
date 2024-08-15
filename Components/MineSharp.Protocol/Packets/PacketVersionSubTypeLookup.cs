using System.Collections;
using System.Diagnostics.CodeAnalysis;
using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets;

public sealed class PacketVersionSubTypeLookup<TBasePacket> : IReadOnlyDictionary<ProtocolVersion, PacketFactory<TBasePacket>>
    where TBasePacket : IPacketStatic<TBasePacket>
{
    private readonly SortedList<ProtocolVersion, PacketFactory<TBasePacket>> protocolVersionToPacketFactory = new();
    public bool Frozen { get; private set; }

    public void RegisterVersionPacket<TVersionPacket>()
        where TVersionPacket : IPacketVersionSubTypeStatic<TVersionPacket, TBasePacket>
    {
        if (Frozen)
        {
            throw new InvalidOperationException("This lookup is already frozen");
        }

        var firstVersionUsedStatic = TVersionPacket.FirstVersionUsedStatic;
        PacketFactory<TBasePacket> readDelegate = TVersionPacket.Read;
        if (!protocolVersionToPacketFactory.TryAdd(firstVersionUsedStatic, readDelegate))
        {
            throw new InvalidOperationException($"There is already a version specific packet registered for protocol version: {firstVersionUsedStatic}");
        }
    }

    public void Freeze()
    {
        Frozen = true;
    }

    public bool TryGetPacketFactory(ProtocolVersion protocolVersion, [NotNullWhen(true)] out PacketFactory<TBasePacket>? packetFactory)
    {
        return protocolVersionToPacketFactory.TryGetLowerBound(protocolVersion, out packetFactory);
    }

    public PacketFactory<TBasePacket> GetPacketFactory(ProtocolVersion protocolVersion)
    {
        if (!TryGetPacketFactory(protocolVersion, out var packetFactory))
        {
            // TODO: create custom exception
            throw new Exception($"There is no version specific packet registered that is suitable for the protocol version: {protocolVersion}");
        }
        return packetFactory;
    }

    public TBasePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var packetFactory = GetPacketFactory(buffer.ProtocolVersion);
        return packetFactory(buffer, data);
    }

    #region Implementation of IReadOnlyDictionary

    public IEnumerable<ProtocolVersion> Keys => protocolVersionToPacketFactory.Keys;
    public IEnumerable<PacketFactory<TBasePacket>> Values => protocolVersionToPacketFactory.Values;
    public int Count => protocolVersionToPacketFactory.Count;
    public PacketFactory<TBasePacket> this[ProtocolVersion key] => protocolVersionToPacketFactory[key];

    public bool ContainsKey(ProtocolVersion key)
    {
        return protocolVersionToPacketFactory.ContainsKey(key);
    }

    public bool TryGetValue(ProtocolVersion key, [MaybeNullWhen(false)] out PacketFactory<TBasePacket> value)
    {
        return protocolVersionToPacketFactory.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<ProtocolVersion, PacketFactory<TBasePacket>>> GetEnumerator()
    {
        return protocolVersionToPacketFactory.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return protocolVersionToPacketFactory.GetEnumerator();
    }

    #endregion
}
