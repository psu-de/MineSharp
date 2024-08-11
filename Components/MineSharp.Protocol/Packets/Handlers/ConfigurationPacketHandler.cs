using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using MineSharp.Protocol.Packets.Serverbound.Configuration;
using FinishConfigurationPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.FinishConfigurationPacket;
using KeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Configuration.KeepAlivePacket;

namespace MineSharp.Protocol.Packets.Handlers;

internal sealed class ConfigurationPacketHandler : GameStatePacketHandler
{
    private readonly MinecraftClient client;
    private readonly MinecraftData data;

    public ConfigurationPacketHandler(MinecraftClient client, MinecraftData data)
        : base(GameState.Configuration)
    {
        this.client = client;
        this.data = data;
    }

    public override Task StateEntered()
    {
        return client.SendClientInformationPacket();
    }

    public override Task HandleIncoming(IPacket packet)
    {
        return packet switch
        {
            DisconnectPacket disconnect => HandleDisconnect(disconnect),
            FinishConfigurationPacket finishConfiguration => HandleFinishConfiguration(finishConfiguration),
            KeepAlivePacket keepAlive => HandleKeepAlive(keepAlive),
            PingPacket ping => HandlePing(ping),

            _ => Task.CompletedTask
        };
    }

    public override bool HandlesIncoming(PacketType type)
    {
        return type is PacketType.CB_Configuration_Disconnect
            or PacketType.CB_Configuration_FinishConfiguration
            or PacketType.CB_Configuration_KeepAlive
            or PacketType.CB_Configuration_Ping;
    }


    private Task HandleDisconnect(DisconnectPacket packet)
    {
        _ = Task.Run(() => client.Disconnect(packet.Reason));
        return Task.CompletedTask;
    }

    private async Task HandleFinishConfiguration(FinishConfigurationPacket packet)
    {
        await client.SendPacket(new Serverbound.Configuration.FinishConfigurationPacket());
        await client.ChangeGameState(GameState.Play);
    }

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        return client.SendPacket(new Serverbound.Configuration.KeepAlivePacket(packet.KeepAliveId));
    }

    private Task HandlePing(PingPacket packet)
    {
        return client.SendPacket(new PongPacket(packet.Id));
    }
}
