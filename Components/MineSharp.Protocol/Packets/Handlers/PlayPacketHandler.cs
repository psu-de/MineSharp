using MineSharp.Core;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using KeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Play.KeepAlivePacket;

namespace MineSharp.Protocol.Packets.Handlers;

internal sealed class PlayPacketHandler : GameStatePacketHandler
{
    private readonly MinecraftClient client;
    private readonly MinecraftData data;

    public PlayPacketHandler(MinecraftClient client, MinecraftData data)
        : base(GameState.Play)
    {
        this.client = client;
        this.data = data;
    }

    public override Task StateEntered()
    {
        client.GameJoinedTcs.SetResult();
        return Task.CompletedTask;
    }

    public override Task HandleIncoming(IPacket packet)
    {
        return packet switch
        {
            KeepAlivePacket keepAlive => HandleKeepAlive(keepAlive),
            PingPacket ping => HandlePing(ping),
            DisconnectPacket disconnect => HandleDisconnect(disconnect),
            LoginPacket login => HandleLogin(login),
            _ => Task.CompletedTask
        };
    }

    public override bool HandlesIncoming(PacketType type)
    {
        return type is PacketType.CB_Play_KeepAlive or PacketType.CB_Play_Ping
            or PacketType.CB_Play_KickDisconnect or PacketType.CB_Play_Login;
    }

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        return client.SendPacket(new Serverbound.Play.KeepAlivePacket(packet.KeepAliveId));
    }

    private Task HandlePing(PingPacket ping)
    {
        return client.SendPacket(new PongPacket(ping.Id));
    }

    private Task HandleDisconnect(DisconnectPacket packet)
    {
        _ = Task.Run(() => client.Disconnect(packet.Reason));
        return Task.CompletedTask;
    }

    private Task HandleLogin(LoginPacket packet)
    {
        return data.Version.Protocol switch
        {
            <= ProtocolVersion.V_1_20 => client.SendClientInformationPacket(GameState),
            _ => Task.CompletedTask
        };
    }
}
