using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.Handlers.Client;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;

namespace MineSharp.Protocol.Packets.Handlers.Server;

internal class HandshakePacketHandler : IPacketHandler
{
    private MinecraftClient _client;

    public HandshakePacketHandler(MinecraftClient client)
    {
        _client = client;
    }


    public bool HandlesIncoming(PacketType type) {
        return type switch
        {
            PacketType.SB_Handshake_SetProtocol => true,
            _ => false
        };
    }


    public Task HandleIncoming(IPacket packet)
    {
        return packet switch
        {
            HandshakePacket handshakePacket => HandleHandshake(handshakePacket)
        };
    }

    public Task HandleOutgoing(IPacket packet)
    {
        return Task.CompletedTask;
    }


    private Task HandleHandshake(HandshakePacket packet)
    {
        if (_client.Data.Version.Protocol != packet.ProtocolVersion)
        {
            _client.Disconnect();
            return Task.CompletedTask;
        }
        _client.HandshakeUpdateHostAndPort(packet.Host, packet.Port);
        _client.UpdateGameState(packet.NextState);
        return Task.CompletedTask;
    }
}
