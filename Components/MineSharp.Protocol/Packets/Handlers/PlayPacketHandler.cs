using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using KeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Play.KeepAlivePacket;

namespace MineSharp.Protocol.Packets.Handlers;

internal class PlayPacketHandler : IPacketHandler
{
    private readonly MinecraftClient client;
    private readonly MinecraftData data;

    public PlayPacketHandler(MinecraftClient client, MinecraftData data)
    {
        this.client = client;
        this.data = data;
    }

    public Task HandleIncoming(IPacket packet)
    {
        return packet switch
        {
            KeepAlivePacket keepAlive => HandleKeepAlive(keepAlive),
            BundleDelimiterPacket bundleDelimiter => HandleBundleDelimiter(bundleDelimiter),
            PingPacket ping => HandlePing(ping),
            DisconnectPacket disconnect => HandleDisconnect(disconnect),
            _ => Task.CompletedTask
        };
    }

    public Task HandleOutgoing(IPacket packet)
    {
        return Task.CompletedTask;
    }

    public bool HandlesIncoming(PacketType type)
    {
        return type is PacketType.CB_Play_KeepAlive or PacketType.CB_Play_BundleDelimiter or PacketType.CB_Play_Ping
            or PacketType.CB_Play_KickDisconnect;
    }

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        client.SendPacket(new Serverbound.Play.KeepAlivePacket(packet.KeepAliveId));
        return Task.CompletedTask;
    }

    private Task HandleBundleDelimiter(BundleDelimiterPacket bundleDelimiter)
    {
        client.HandleBundleDelimiter();
        return Task.CompletedTask;
    }

    private Task HandlePing(PingPacket ping)
    {
        client.SendPacket(new PongPacket(ping.Id));
        return Task.CompletedTask;
    }

    private Task HandleDisconnect(DisconnectPacket packet)
    {
        _ = Task.Run(() => client.Disconnect(packet.Reason));
        return Task.CompletedTask;
    }
}
