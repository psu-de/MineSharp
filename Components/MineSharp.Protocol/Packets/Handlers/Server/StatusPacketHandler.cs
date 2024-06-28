using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Handlers.Server;

internal class StatusPacketHandler : IPacketHandler
{
    private MinecraftClient _client;

    public StatusPacketHandler(MinecraftClient client)
    {
        this._client = client;
    }

    public Task HandleIncoming(IPacket     packet) => Task.CompletedTask;
    public Task HandleOutgoing(IPacket     packet) => Task.CompletedTask;
    public bool HandlesIncoming(PacketType type)   => false;
}
