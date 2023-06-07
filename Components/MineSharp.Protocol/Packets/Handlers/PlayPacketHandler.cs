using MineSharp.Data;
using MineSharp.Protocol.Packets.Clientbound.Play;

namespace MineSharp.Protocol.Packets.Handlers;

public class PlayPacketHandler : IPacketHandler
{
    private MinecraftClient _client;
    private MinecraftData _data;

    public PlayPacketHandler(MinecraftClient client, MinecraftData data)
    {
        this._client = client;
        this._data = data;
    }

    public Task HandleIncoming(IPacket packet)
    {
        return packet switch {
            KeepAlivePacket keepAlive => HandleKeepAlive(keepAlive),
            _ => Task.CompletedTask
        };
    }

    public Task HandleOutgoing(IPacket packet)
    {
        return Task.CompletedTask;
    }

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        this._client.SendPacket(new Serverbound.Play.KeepAlivePacket(packet.KeepAliveId));
        return Task.CompletedTask;
    }
}
