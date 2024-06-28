using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using NLog;

namespace MineSharp.Protocol.Packets.Handlers.Server;

internal class ConfigurationPacketHandler : IPacketHandler
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly MinecraftClient _client;
    private readonly MinecraftData   _data;

    public ConfigurationPacketHandler(MinecraftClient client, MinecraftData data)
    {
        this._client = client;
        this._data   = data;
    }

    public Task HandleIncoming(IPacket packet)
    {
        return packet switch
        {
            DisconnectPacket disconnect                   => HandleDisconnect(disconnect),
            FinishConfigurationPacket finishConfiguration => HandleFinishConfiguration(finishConfiguration),
            KeepAlivePacket keepAlive                     => HandleKeepAlive(keepAlive),
            PingPacket ping                               => HandlePing(ping),

            _ => Task.CompletedTask
        };
    }

    public Task HandleOutgoing(IPacket packet)
    {
        if (packet is Serverbound.Configuration.FinishConfigurationPacket)
        {
            this._client.UpdateGameState(GameState.Play);
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
        _ = Task.Run(() => this._client.Disconnect(packet.Reason.Json));
        return Task.CompletedTask;
    }

    private Task HandleFinishConfiguration(FinishConfigurationPacket packet)
    {
        _ = this._client.SendPacket(new Serverbound.Configuration.FinishConfigurationPacket());
        return Task.CompletedTask;
    }

    private Task HandleKeepAlive(KeepAlivePacket packet)
    {
        this._client.SendPacket(new Serverbound.Configuration.KeepAlivePacket(packet.KeepAliveId));
        return Task.CompletedTask;
    }

    private Task HandlePing(PingPacket packet)
    {
        this._client.SendPacket(new Serverbound.Configuration.PongPacket(packet.Id));
        return Task.CompletedTask;
    }
}
