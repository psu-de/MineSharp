using MineSharp.Core.Common;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using NLog;

namespace MineSharp.Bot.Plugins;

/// <summary>
/// This Plugins provides basic functionality for the minecraft player.
/// It keeps track of things like Health and initializes the Bot entity,
/// which is required for many other plugins.
/// </summary>
public class PlayerPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    public Entity? Entity { get; private set; }
    
    public float? Health { get; private set; }
    public float? Saturation { get; private set; }
    public float? Food { get; private set; }
    
    public GameMode? GameMode { get; private set; }
    public string? Dimension { get; private set; }
    public bool? IsAlive => this.Health > 0; 

    public event Events.BotEvent? OnHealthChanged;
    public event Events.BotEvent? OnRespawned;
    public event Events.BotEvent? OnDied;

    public PlayerPlugin(MinecraftBot bot) : base(bot)
    {
        this.Bot.Client.On<SetHealthPacket>(this.HandleSetHealthPacket);
        this.Bot.Client.On<CombatDeathPacket>(this.HandleCombatDeathPacket);
        this.Bot.Client.On<RespawnPacket>(this.HandleRespawnPacket);
    }

    public Task Respawn()
        => this.Bot.Client.SendPacket(new ClientCommandPacket(0));

    protected override async Task Init()
    {
        var loginPacketTask = this.Bot.Client.WaitForPacket<LoginPacket>();
        var positionPacketTask = this.Bot.Client.WaitForPacket<SynchronizePlayerPositionPacket>();

        await Task.WhenAll(loginPacketTask, positionPacketTask);

        var loginPacket = await loginPacketTask;
        var positionPacket = await positionPacketTask;

        this.Entity = new Entity(
            this.Bot.Data.Entities.GetByName("player"),
            loginPacket.EntityId,
            new Vector3(positionPacket.X, positionPacket.Y, positionPacket.Z),
            positionPacket.Pitch,
            positionPacket.Yaw,
            new Vector3(0, 0, 0),
            true,
            new Dictionary<int, Effect?>());

        this.Health = 20.0f;
        this.Food = 20.0f;
        this.Saturation = 20.0f;
        this.Dimension = loginPacket.DimensionName;
        this.GameMode = (GameMode)loginPacket.GameMode;
        
        Logger.Info($"Initialized Bot Entity: Position=({this.Entity.Position}), GameMode={this.GameMode}, Dimension={this.Dimension}.");

        await this.Bot.Client.SendPacket(
            new SetPlayerPositionAndRotationPacket(
                positionPacket.X,
                positionPacket.Y,
                positionPacket.Z,
                positionPacket.Yaw,
                positionPacket.Pitch,
                this.Entity.IsOnGround));
    }

    private Task HandleSetHealthPacket(SetHealthPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        this.Health = packet.Health;
        this.Food = packet.Food;
        this.Saturation = packet.Saturation;
        
        this.OnHealthChanged?.Invoke(this.Bot);
        
        return Task.CompletedTask;
    }

    private Task HandleCombatDeathPacket(CombatDeathPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        this.Health = 0;
        this.OnDied?.Invoke(this.Bot);
        return Task.CompletedTask;
    }

    private Task HandleRespawnPacket(RespawnPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;
        
        this.OnRespawned?.Invoke(this.Bot);
        return Task.CompletedTask;
    }
}
