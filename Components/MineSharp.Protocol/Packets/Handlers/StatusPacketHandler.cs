namespace MineSharp.Protocol.Packets.Handlers;

public class StatusPacketHandler : IPacketHandler
{
    private MinecraftClient _client;

    public StatusPacketHandler(MinecraftClient client)
    {
        this._client = client;
    }

    public Task HandleIncoming(IPacket packet) => Task.CompletedTask;
    public Task HandleOutgoing(IPacket packet) => Task.CompletedTask;
}
