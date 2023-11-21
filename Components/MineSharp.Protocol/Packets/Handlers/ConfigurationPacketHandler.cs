using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using NLog;

namespace MineSharp.Protocol.Packets.Handlers;

public class ConfigurationPacketHandler : IPacketHandler
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private readonly MinecraftClient _client;
    private readonly MinecraftData _data;

    public ConfigurationPacketHandler(MinecraftClient client, MinecraftData data)
    {
        this._client = client;
        this._data = data;
    }
    
    public Task HandleIncoming(IPacket packet)
    {
        return packet switch {
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
            this._client.UpdateGameState(GameState.Play);
        }

        return Task.CompletedTask;
    }

    private Task HandleDisconnect(DisconnectPacket packet)
    {
        _ = Task.Run(() => this._client.Disconnect(packet.Reason.JSON));
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
