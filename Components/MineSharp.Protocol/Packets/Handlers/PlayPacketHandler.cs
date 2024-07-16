using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using KeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Play.KeepAlivePacket;

namespace MineSharp.Protocol.Packets.Handlers;

internal class PlayPacketHandler : IPacketHandler
{
    private MinecraftClient _client;
    private MinecraftData   _data;

    public PlayPacketHandler(MinecraftClient client, MinecraftData data)
    {
        this._client = client;
        this._data   = data;
    }

    public Task HandleIncoming(IPacket packet)
    {
        return packet switch
        {
            KeepAlivePacket keepAlive             => HandleKeepAlive(keepAlive),
            BundleDelimiterPacket bundleDelimiter => HandleBundleDelimiter(bundleDelimiter),
            PingPacket ping                       => HandlePing(ping),
            DisconnectPacket disconnect           => HandleDisconnect(disconnect),
            _                                     => Task.CompletedTask
        };
    }

    public Task HandleOutgoing(IPacket packet)
    {
        return Task.CompletedTask;
    }

    public bool HandlesIncoming(PacketType type)
        => type is PacketType.CB_Play_KeepAlive or PacketType.CB_Play_BundleDelimiter or PacketType.CB_Play_Ping or PacketType.CB_Play_KickDisconnect;

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        this._client.SendPacket(new Serverbound.Play.KeepAlivePacket(packet.KeepAliveId));
        return Task.CompletedTask;
    }

    private Task HandleBundleDelimiter(BundleDelimiterPacket bundleDelimiter)
    {
        this._client.HandleBundleDelimiter();
        return Task.CompletedTask;
    }

    private Task HandlePing(PingPacket ping)
    {
        this._client.SendPacket(new PongPacket(ping.Id));
        return Task.CompletedTask;
    }

    private Task HandleDisconnect(DisconnectPacket packet)
    {
        _ = Task.Run(() => this._client.Disconnect(packet.Reason.GetMessage(this._data)));
        return Task.CompletedTask;
    }
}
