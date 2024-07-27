using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Handlers;

internal class StatusPacketHandler : IPacketHandler
{
    private MinecraftClient client;

    public StatusPacketHandler(MinecraftClient client)
    {
        this.client = client;
    }

    public Task HandleIncoming(IPacket packet)
    {
        return Task.CompletedTask;
    }

    public Task HandleOutgoing(IPacket packet)
    {
        return Task.CompletedTask;
    }

    public bool HandlesIncoming(PacketType type)
    {
        return false;
    }
}
