using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Play;

namespace MineSharp.Protocol.Packets.Handlers;

internal class PlayPacketHandler : IPacketHandler
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
            BundleDelimiterPacket bundleDelimiter => HandleBundleDelimiter(bundleDelimiter),
            _ => Task.CompletedTask
        };
    }

    public Task HandleOutgoing(IPacket packet)
    {
        return Task.CompletedTask;
    }

    public bool HandlesIncoming(PacketType type)
        => type is PacketType.CB_Play_KeepAlive or PacketType.CB_Play_BundleDelimiter; 

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        this._client.SendPacket(new Serverbound.Play.KeepAlivePacket(packet.KeepAliveId));
        return Task.CompletedTask;
    }

    public Task HandleBundleDelimiter(BundleDelimiterPacket bundleDelimiter)
    {
        this._client.HandleBundleDelimiter();
        return Task.CompletedTask;
    }
}
