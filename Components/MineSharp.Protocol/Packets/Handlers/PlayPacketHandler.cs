using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using KeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Play.KeepAlivePacket;

namespace MineSharp.Protocol.Packets.Handlers;

internal class PlayPacketHandler : GameStatePacketHandler
{
    private readonly MinecraftClient client;
    private readonly MinecraftData data;

    public PlayPacketHandler(MinecraftClient client, MinecraftData data)
        : base(GameState.Play)
    {
        this.client = client;
        this.data = data;
    }

    public override Task HandleIncoming(IPacket packet)
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

    public override Task HandleOutgoing(IPacket packet)
    {
        return Task.CompletedTask;
    }

    public override bool HandlesIncoming(PacketType type)
    {
        return type is PacketType.CB_Play_KeepAlive or PacketType.CB_Play_BundleDelimiter or PacketType.CB_Play_Ping
            or PacketType.CB_Play_KickDisconnect;
    }

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        _ = client.SendPacket(new Serverbound.Play.KeepAlivePacket(packet.KeepAliveId));
        return Task.CompletedTask;
    }

    private Task HandleBundleDelimiter(BundleDelimiterPacket bundleDelimiter)
    {
        return client.HandleBundleDelimiter();
    }

    private Task HandlePing(PingPacket ping)
    {
        _ = client.SendPacket(new PongPacket(ping.Id));
        return Task.CompletedTask;
    }

    private Task HandleDisconnect(DisconnectPacket packet)
    {
        _ = Task.Run(() => client.Disconnect(packet.Reason));
        return Task.CompletedTask;
    }
}
