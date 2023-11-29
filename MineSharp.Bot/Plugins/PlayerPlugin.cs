using MineSharp.Core.Common;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using NLog;
using System.Collections.Concurrent;

namespace MineSharp.Bot.Plugins;

/// <summary>
/// This Plugins provides basic functionality for the minecraft player.
/// It keeps track of things like Health and initializes the Bot entity,
/// which is required for many other plugins.
/// </summary>
public class PlayerPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// The Minecraftplayer representing the Minecraft Bot itself.
    /// </summary>
    public MinecraftPlayer? Self { get; private set; }

    /// <summary>
    /// The Entity representing the Minecraft Bot itself.
    /// </summary>
    public Entity? Entity => this.Self?.Entity;

    /// <summary>
    /// All players on the server.
    /// </summary>
    public IDictionary<UUID, MinecraftPlayer> Players { get; }

    /// <summary>
    /// Minecraft players indexed by <see cref="MineSharp.Core.Common.Entities.Entity.ServerId"/>.
    /// Contains only loaded players (Players in the Bots visible range)
    /// </summary>
    public IDictionary<int, MinecraftPlayer> PlayerMap { get; }
    
    /// <summary>
    /// The Bots health (between 0.0 - 20.0)
    /// </summary>
    public float? Health { get; private set; }
    
    /// <summary>
    /// The Bots saturation level
    /// </summary>
    public float? Saturation { get; private set; }
    
    /// <summary>
    /// The Bots food level
    /// </summary>
    public float? Food { get; private set; }
    
    /// <summary>
    /// The Name of the dimension the bot is currently in.
    /// </summary>
    public string? Dimension { get; private set; }
    
    /// <summary>
    /// Whether the bot is alive or dead.
    /// </summary>
    public bool? IsAlive => this.Health > 0; 
    
    // TODO: Maybe move weather related stuff to world?
    
    /// <summary>
    /// Whether it is raining
    /// </summary>
    public bool IsRaining { get; set; }
    public float RainLevel { get; set; }
    public float ThunderLevel { get; set; }
    

    /// <summary>
    /// This event fires when the health, food or saturation has been updated.
    /// </summary>
    public event Events.BotEvent? OnHealthChanged;
    
    /// <summary>
    /// This event fires when the bot respawned or joined another dimension.
    /// </summary>
    public event Events.BotEvent? OnRespawned;
    
    /// <summary>
    /// This event fires when a the bot has died.
    /// </summary>
    public event Events.BotEvent? OnDied;

    /// <summary>
    /// This event fires when a player joined the server. 
    /// </summary>
    public event Events.PlayerEvent? OnPlayerJoined;

    /// <summary>
    /// This event fires when a player left the server.
    /// </summary>
    public event Events.PlayerEvent? OnPlayerLeft;

    /// <summary>
    /// This event fires when a player has come into visible range and their entity has been loaded.
    /// </summary>
    public event Events.PlayerEvent? OnPlayerLoaded;

    /// <summary>
    /// This event fires when the weather in the world changes.
    /// </summary>
    public event Events.BotEvent? OnWeatherChanged;


    private EntityPlugin? _entities;

    public PlayerPlugin(MinecraftBot bot) : base(bot)
    {
        this.Players = new ConcurrentDictionary<UUID, MinecraftPlayer>();
        this.PlayerMap = new ConcurrentDictionary<int, MinecraftPlayer>();

        this.Bot.Client.On<SetHealthPacket>(this.HandleSetHealthPacket);
        this.Bot.Client.On<CombatDeathPacket>(this.HandleCombatDeathPacket);
        this.Bot.Client.On<RespawnPacket>(this.HandleRespawnPacket);
        this.Bot.Client.On<SpawnPlayerPacket>(this.HandleSpawnPlayer);
        this.Bot.Client.On<PlayerInfoUpdatePacket>(this.HandlePlayerInfoUpdate);
        this.Bot.Client.On<PlayerInfoRemovePacket>(this.HandlePlayerInfoRemove);
        this.Bot.Client.On<GameEventPacket>(this.HandleGameEvent);
        this.Bot.Client.On<AcknowledgeBlockChangePacket>(this.HandleAcknowledgeBlockChange);
        this.Bot.Client.On<EntityStatusPacket>(this.HandleEntityStatus);
    }

    /// <summary>
    /// Respawns the bot when its dead.
    /// </summary>
    /// <returns></returns>
    public Task Respawn()
        => this.Bot.Client.SendPacket(new ClientCommandPacket(0));

    protected override async Task Init()
    {
        this._entities = this.Bot.GetPlugin<EntityPlugin>();
        var loginPacketTask = this.Bot.Client.WaitForPacket<LoginPacket>();
        var positionPacketTask = this.Bot.Client.WaitForPacket<PlayerPositionPacket>();

        await Task.WhenAll(loginPacketTask, positionPacketTask);

        var loginPacket = await loginPacketTask;
        var positionPacket = await positionPacketTask;

        var entity = new Entity(
            this.Bot.Data.Entities.GetByName("player"),
            loginPacket.EntityId,
            new Vector3(positionPacket.X, positionPacket.Y, positionPacket.Z),
            positionPacket.Pitch,
            positionPacket.Yaw,
            new Vector3(0, 0, 0),
            true,
            new Dictionary<EffectType, Effect?>());

        this.Self = new MinecraftPlayer(
            this.Bot.Session.Username,
            this.Bot.Session.UUID,
            0,
            (GameMode)loginPacket.GameMode,
            entity);

        this.PlayerMap.TryAdd(entity.ServerId, this.Self);

        this.Health = 20.0f;
        this.Food = 20.0f;
        this.Saturation = 20.0f;
        this.Dimension = loginPacket.DimensionName;
        
        Logger.Info($"Initialized Bot Entity: Position=({this.Entity!.Position}), GameMode={this.Self.GameMode}, Dimension={this.Dimension}.");

        await this.Bot.Client.SendPacket(
            new SetPlayerPositionAndRotationPacket(
                positionPacket.X,
                positionPacket.Y,
                positionPacket.Z,
                positionPacket.Yaw,
                positionPacket.Pitch,
                this.Entity.IsOnGround));
    }
    
    /// <summary>
    /// Plays a swinging arm animation for other players.
    /// </summary>
    /// <param name="hand">The hand to swing</param>
    /// <param name="token">optional cancellation token</param>
    /// <returns></returns>
    public Task SwingArm(PlayerHand hand = PlayerHand.MainHand, CancellationToken? token = null)
    {
        var packet = new SwingArmPacket(hand);
        return this.Bot.Client.SendPacket(packet, token);
    }

    private Task HandleSetHealthPacket(SetHealthPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        this.Health = packet.Health;
        this.Food = packet.Food;
        this.Saturation = packet.Saturation;
        
        this.OnHealthChanged?.Invoke(this.Bot);
        if (this.Health == 0)
            this.OnDied?.Invoke(this.Bot);
        
        return Task.CompletedTask;
    }

    private Task HandleCombatDeathPacket(CombatDeathPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        this.Health = 0;
        // this.OnDied?.Invoke(this.Bot);
        return Task.CompletedTask;
    }

    private Task HandleRespawnPacket(RespawnPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;
        
        this.OnRespawned?.Invoke(this.Bot);
        return Task.CompletedTask;
    }

    private Task HandleSpawnPlayer(SpawnPlayerPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;
        
        if (!this.Players.TryGetValue(packet.PlayerUuid, out var player))
        {
            Logger.Warn($"Received SpawnPlayer packet for unknown player: {packet.PlayerUuid}");
            return Task.CompletedTask;
        }

        var entity = new Entity(
            this.Bot.Data.Entities.GetByName("player"),
            packet.EntityId,
            new Vector3(
                packet.X,
                packet.Y,
                packet.Z),
            packet.Pitch,
            packet.Yaw,
            Vector3.Zero,
            true,
            new Dictionary<EffectType, Effect?>());
        player.Entity = entity;
        
        this._entities!.AddEntity(entity);
        this.PlayerMap.TryAdd(player.Entity!.ServerId, player);
        
        this.OnPlayerLoaded?.Invoke(this.Bot, player);
        return Task.CompletedTask;
    }

    private Task HandlePlayerInfoUpdate(PlayerInfoUpdatePacket packet)
    {
        foreach (var entry in packet.Data)
        {
            foreach (var action in entry.Actions)
            {
                this.Players.TryGetValue(entry.Player, out var player);
                
                switch (action)
                {
                    case PlayerInfoUpdatePacket.AddPlayerAction addAction:
                        if (player != null)
                        {
                            player.Username = addAction.Name;
                            break;
                        }

                        var newPlayer = new MinecraftPlayer(addAction.Name, entry.Player, -1, GameMode.Survival, null);
                        this.Players.TryAdd(entry.Player, newPlayer);
                        this.OnPlayerJoined?.Invoke(this.Bot, newPlayer);
                        break;
                    
                    case PlayerInfoUpdatePacket.UpdateGameModeAction gameModeAction:
                        if (player == null)
                        {
                            Logger.Warn($"Received player info update for unknown player {entry.Player}.");
                            break;
                        }
                        player.GameMode = gameModeAction.GameMode;
                        break;
                    
                    case PlayerInfoUpdatePacket.UpdateListedAction updateListedAction:
                        if (player == null)
                        {
                            Logger.Warn($"Received player info update for unknown player {entry.Player}.");
                            break;
                        }

                        if (!updateListedAction.Listed && this.Bot.Data.Version.Protocol <= ProtocolVersion.V_1_19_2)
                        {
                            this.OnPlayerLeft?.Invoke(this.Bot, player);
                        }
                        break;
                    
                    case PlayerInfoUpdatePacket.UpdateLatencyAction updateLatencyAction:
                        if (player == null)
                        {
                            Logger.Warn($"Received player info update for unknown player {entry.Player}.");
                            break;
                        }

                        player.Ping = updateLatencyAction.Ping;
                        break;
                    
                    case PlayerInfoUpdatePacket.UpdateDisplayName updateDisplayName:
                        if (player == null)
                        {
                            Logger.Warn($"Received player info update for unknown player {entry.Player}.");
                            break;
                        }

                        if (updateDisplayName.DisplayName == null)
                            break;
                        
                        player.Username = updateDisplayName.DisplayName!;
                        break;
                }
            }
        }

        return Task.CompletedTask;
    }

    private Task HandlePlayerInfoRemove(PlayerInfoRemovePacket packet)
    {
        foreach (var uuid in packet.Players)
        {
            if (!this.Players.Remove(uuid, out var player))
                continue;
            
            this.OnPlayerLeft?.Invoke(this.Bot, player);
        }

        return Task.CompletedTask;
    }

    private Task HandleGameEvent(GameEventPacket packet)
    {
        switch (packet.Event)
        {
            case 1: // End Raining
                this.IsRaining = false;
                this.OnWeatherChanged?.Invoke(this.Bot);
                break;
            
            case 2: // Begin Raining
                this.IsRaining = true;
                this.OnWeatherChanged?.Invoke(this.Bot);
                break;
            
            case 7: // Rain Level Changed
                this.RainLevel = packet.Value;
                this.OnWeatherChanged?.Invoke(this.Bot);
                break;
            
            case 8: // Thunder Level Changed
                this.ThunderLevel = packet.Value;
                this.OnWeatherChanged?.Invoke(this.Bot);
                break;
            
            case 3: // GameMode Change
                var gameMode = (GameMode)packet.Value;
                this.Self!.GameMode = gameMode;
                break;          
        }

        return Task.CompletedTask;
    }

    private Task HandleAcknowledgeBlockChange(AcknowledgeBlockChangePacket packet)
    {
        if (packet.Body is AcknowledgeBlockChangePacket.PacketBody_1_19 v1_19)
        {
            this.Bot.SequenceId = v1_19.SequenceId;
        }

        return Task.CompletedTask;
    }

    private Task HandleEntityStatus(EntityStatusPacket packet)
    {
        switch (packet.Status)
        {
            case >= 24 and <= 28: 
                HandlePlayerSetPermission(packet);
                break;
        }

        return Task.CompletedTask;
    }

    private void HandlePlayerSetPermission(EntityStatusPacket packet)
    {
        if (!this.PlayerMap.TryGetValue(packet.EntityId, out var player))
        {
            return;
        }
        
        var permissionLevel = (PermissionLevel)(packet.Status - 24);
        player.PermissionLevel = permissionLevel;
    }
}
