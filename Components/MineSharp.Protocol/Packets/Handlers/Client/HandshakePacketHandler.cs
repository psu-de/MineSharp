using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;

namespace MineSharp.Protocol.Packets.Handlers.Client;

internal class HandshakePacketHandler : IPacketHandler
{
    private MinecraftClient _client;

    public HandshakePacketHandler(MinecraftClient client)
    {
        this._client = client;
    }

    public Task HandleIncoming(IPacket packet)
    {
        return Task.CompletedTask;
    }

    public Task HandleOutgoing(IPacket packet)
    {
        return packet switch
        {
            HandshakePacket handshake => HandleHandshake(handshake),
            _ => throw new UnexpectedPacketException($"Unexpected outgoing packet during handshaking: {packet.GetType().FullName}")
        };
    }

    public bool HandlesIncoming(PacketType type) => false;


    private Task HandleHandshake(HandshakePacket packet)
    {
        this._client.UpdateGameState(packet.NextState);
        return Task.CompletedTask;
    }
}
