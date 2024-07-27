using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using MineSharp.Protocol.Packets.Serverbound.Configuration;
using NLog;
using FinishConfigurationPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.FinishConfigurationPacket;
using KeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Configuration.KeepAlivePacket;

namespace MineSharp.Protocol.Packets.Handlers;

internal class ConfigurationPacketHandler : IPacketHandler
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly MinecraftClient client;
    private readonly MinecraftData data;

    public ConfigurationPacketHandler(MinecraftClient client, MinecraftData data)
    {
        this.client = client;
        this.data = data;
    }

    public Task HandleIncoming(IPacket packet)
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

    public Task HandleOutgoing(IPacket packet)
    {
        if (packet is Serverbound.Configuration.FinishConfigurationPacket)
        {
            client.UpdateGameState(GameState.Play);
        }

        return Task.CompletedTask;
    }

    public bool HandlesIncoming(PacketType type)
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

    private Task HandleFinishConfiguration(FinishConfigurationPacket packet)
    {
        _ = client.SendPacket(new Serverbound.Configuration.FinishConfigurationPacket());
        return Task.CompletedTask;
    }

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        client.SendPacket(new Serverbound.Configuration.KeepAlivePacket(packet.KeepAliveId));
        return Task.CompletedTask;
    }

    private Task HandlePing(PingPacket packet)
    {
        client.SendPacket(new PongPacket(packet.Id));
        return Task.CompletedTask;
    }
}
