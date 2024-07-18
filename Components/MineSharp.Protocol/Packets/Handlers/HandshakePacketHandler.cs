using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;

namespace MineSharp.Protocol.Packets.Handlers;

internal class HandshakePacketHandler : IPacketHandler
{
    private readonly MinecraftClient client;

    public HandshakePacketHandler(MinecraftClient client)
    {
        this.client = client;
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
            _ => throw new UnexpectedPacketException(
                $"unexpected outgoing packet during handshaking: {packet.GetType().FullName}")
        };
    }

    public bool HandlesIncoming(PacketType type)
    {
        return false;
    }


    private Task HandleHandshake(HandshakePacket packet)
    {
        client.UpdateGameState(packet.NextState);
        return Task.CompletedTask;
    }
}
