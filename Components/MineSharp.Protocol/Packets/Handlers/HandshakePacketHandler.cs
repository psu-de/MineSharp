using MineSharp.Core.Common.Protocol;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;

namespace MineSharp.Protocol.Packets.Handlers;

internal class HandshakePacketHandler : GameStatePacketHandler
{
    private readonly MinecraftClient client;

    public HandshakePacketHandler(MinecraftClient client)
        : base(GameState.Handshaking)
    {
        this.client = client;
    }

    public override Task HandleIncoming(IPacket packet)
    {
        return Task.CompletedTask;
    }

    public override Task HandleOutgoing(IPacket packet)
    {
        return packet switch
        {
            HandshakePacket handshake => HandleHandshake(handshake),
            _ => throw new UnexpectedPacketException(
                $"unexpected outgoing packet during handshaking: {packet.GetType().FullName}")
        };
    }

    public override bool HandlesIncoming(PacketType type)
    {
        return false;
    }


    private Task HandleHandshake(HandshakePacket packet)
    {
        client.UpdateGameState(packet.NextState);
        return Task.CompletedTask;
    }
}
