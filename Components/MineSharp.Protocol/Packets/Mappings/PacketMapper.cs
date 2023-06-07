using MineSharp.Data;
using MineSharp.Protocol.Extensions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Protocol.Packets.Mappings;

internal sealed class PacketMapper
{
    private readonly PacketFlow _incomingDirection;
    private readonly PacketFlow _outgoinDirection;

    private readonly Dictionary<string, string> _outgoingMapping;
    private readonly Dictionary<string, string> _incomingMapping;
    private readonly MinecraftData _data;
    
    public IPacketMapping Mapping { get; }

    public PacketMapper(IPacketMapping mapping, PacketFlow incomingDirection, MinecraftData data)
    {
        this.Mapping = mapping;
        this._incomingDirection = incomingDirection;
        this._outgoinDirection = incomingDirection switch {
            PacketFlow.Clientbound => PacketFlow.Serverbound,
            PacketFlow.Serverbound => PacketFlow.Clientbound,
            _ => throw new UnreachableException()
        };
        this._data = data;

        if (incomingDirection == PacketFlow.Clientbound)
        { 
            // We are the client.
            this._incomingMapping = this.Mapping.ClientboundMapping;
            this._outgoingMapping = InverseMapping(this.Mapping.ServerboundMapping);
        }
        else
        { 
            // we are the server
            this._incomingMapping = this.Mapping.ServerboundMapping;
            this._outgoingMapping = this.InverseMapping(this.Mapping.ClientboundMapping);
        }
    }

    private Dictionary<string, string> InverseMapping(Dictionary<string, string> mapping)
    {
        var inversed = new Dictionary<string, string>();

        // In 1.19 the chat packet was split up into two different packets
        // ChatMessagePacket and ChatCommandPacket. To inverse the mapping,
        // the second entry is ignored and it is expected that the first
        // packet mapping handles the mapping itself.
        foreach (var kvp in mapping)
        {
            inversed.TryAdd(kvp.Value, kvp.Key);
        }

        return mapping.ToDictionary(x => x.Value, x => x.Key);
    }

    public string ConvertIncomingPacketId(ref int id, GameState state)
    {
        var from = this._data;
        var to = PacketPalette.ImplementedProtocol;

        var name = from.Protocol.GetPacketName(id, state, this._incomingDirection);

        if (state != GameState.PLAY)
        {
            id = to.Protocol.GetPacketId(name, state, this._incomingDirection);
            return name;
        }
        
        if (this._incomingMapping.TryGetValue(name, out var newName))
        {
            id = to.Protocol.GetPacketId(newName, state, this._incomingDirection);
            return name;
        }

        id = to.Protocol.GetPacketId(name, state, this._incomingDirection);
        return name;
    }

    public string ConvertOutgoingPacketId(ref int id, GameState state)
    {
        var from = PacketPalette.ImplementedProtocol;
        var to = this._data;

        var name = from.Protocol.GetPacketName(id, state, this._outgoinDirection);

        if (state != GameState.PLAY)
        {
            id = to.Protocol.GetPacketId(name, state, this._outgoinDirection);
            return name;
        }
        
        if (this._outgoingMapping.TryGetValue(name, out var newName))
        {
            id = to.Protocol.GetPacketId(newName, state, this._outgoinDirection);
            return name;
        }

        id = to.Protocol.GetPacketId(name, state, this._outgoinDirection);
        return name;
    }
}
