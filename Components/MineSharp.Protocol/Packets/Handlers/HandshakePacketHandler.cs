using MineSharp.Core.Common.Protocol;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Handlers;

internal sealed class HandshakePacketHandler : GameStatePacketHandler
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

    public override bool HandlesIncoming(PacketType type)
    {
        return false;
    }
}
