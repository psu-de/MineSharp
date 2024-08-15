using System.Collections;
using System.Diagnostics.CodeAnalysis;
using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets;

public sealed class PacketVersionSubTypeLookup<TBasePacket> : IReadOnlyDictionary<ProtocolVersion, PacketReadDelegate<TBasePacket>>
    where TBasePacket : IPacketStatic<TBasePacket>
{
    private readonly SortedList<ProtocolVersion, PacketReadDelegate<TBasePacket>> protocolVersionToPacketReadDelegate = new();
    public bool Frozen { get; private set; }

    public void RegisterVersionPacket<TVersionPacket>()
        where TVersionPacket : IPacketVersionSubTypeStatic<TVersionPacket, TBasePacket>
    {
        if (Frozen)
        {
            throw new InvalidOperationException("This lookup is already frozen");
        }

        var firstVersionUsedStatic = TVersionPacket.FirstVersionUsedStatic;
        PacketReadDelegate<TBasePacket> readDelegate = TVersionPacket.Read;
        if (!protocolVersionToPacketReadDelegate.TryAdd(firstVersionUsedStatic, readDelegate))
        {
            throw new InvalidOperationException($"There is already a version specific packet registered for protocol version: {firstVersionUsedStatic}");
        }
    }

    public void Freeze()
    {
        Frozen = true;
    }

    public bool TryGetPacketReadDelegate(ProtocolVersion protocolVersion, [NotNullWhen(true)] out PacketReadDelegate<TBasePacket>? packetReadDelegate)
    {
        packetReadDelegate = protocolVersionToPacketReadDelegate.LowerBound(protocolVersion);
        return packetReadDelegate != null;
    }

    public PacketReadDelegate<TBasePacket> GetPacketReadDelegate(ProtocolVersion protocolVersion)
    {
        if (!TryGetPacketReadDelegate(protocolVersion, out var packetReadDelegate))
        {
            // TODO: create custom exception
            throw new Exception($"There is no version specific packet registered that is suitable for the protocol version: {protocolVersion}");
        }
        return packetReadDelegate;
    }

    public TBasePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var packetReadDelegate = GetPacketReadDelegate(buffer.ProtocolVersion);
        return packetReadDelegate(buffer, data);
    }

    #region Implementation of IReadOnlyDictionary

    public IEnumerable<ProtocolVersion> Keys => protocolVersionToPacketReadDelegate.Keys;
    public IEnumerable<PacketReadDelegate<TBasePacket>> Values => protocolVersionToPacketReadDelegate.Values;
    public int Count => protocolVersionToPacketReadDelegate.Count;
    public PacketReadDelegate<TBasePacket> this[ProtocolVersion key] => protocolVersionToPacketReadDelegate[key];

    public bool ContainsKey(ProtocolVersion key)
    {
        return protocolVersionToPacketReadDelegate.ContainsKey(key);
    }

    public bool TryGetValue(ProtocolVersion key, [MaybeNullWhen(false)] out PacketReadDelegate<TBasePacket> value)
    {
        return protocolVersionToPacketReadDelegate.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<ProtocolVersion, PacketReadDelegate<TBasePacket>>> GetEnumerator()
    {
        return protocolVersionToPacketReadDelegate.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return protocolVersionToPacketReadDelegate.GetEnumerator();
    }

    #endregion
}
