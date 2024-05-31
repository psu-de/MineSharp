using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using NLog;

namespace MineSharp.Protocol.Packets.Handlers.Client;

internal class ConfigurationPacketHandler : IPacketHandler
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly MinecraftClient _client;
    private readonly MinecraftData _data;

    public ConfigurationPacketHandler(MinecraftClient client, MinecraftData data)
    {
        _client = client;
        _data = data;
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
            _client.UpdateGameState(GameState.Play);
        }

        return Task.CompletedTask;
    }

    public bool HandlesIncoming(PacketType type)
        => type is PacketType.CB_Configuration_Disconnect
            or PacketType.CB_Configuration_FinishConfiguration
            or PacketType.CB_Configuration_KeepAlive
            or PacketType.CB_Configuration_Ping;


    private Task HandleDisconnect(DisconnectPacket packet)
    {
        _ = Task.Run(() => _client.Disconnect(packet.Reason.Json));
        return Task.CompletedTask;
    }

    private Task HandleFinishConfiguration(FinishConfigurationPacket packet)
    {
        _ = _client.SendPacket(new Serverbound.Configuration.FinishConfigurationPacket());
        return Task.CompletedTask;
    }

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        _client.SendPacket(new Serverbound.Configuration.KeepAlivePacket(packet.KeepAliveId));
        return Task.CompletedTask;
    }

    private Task HandlePing(PingPacket packet)
    {
        _client.SendPacket(new Serverbound.Configuration.PongPacket(packet.Id));
        return Task.CompletedTask;
    }
}
