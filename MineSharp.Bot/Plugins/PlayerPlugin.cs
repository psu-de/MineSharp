using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using MineSharp.Bot.Exceptions;
using MineSharp.Bot.Utils;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Events;
using MineSharp.Core.Geometry;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using NLog;

namespace MineSharp.Bot.Plugins;

/// <summary>
///     This Plugins provides basic functionality for the Minecraft player.
///     It keeps track of things like Health and initializes the Bot entity,
///     which is required for many other plugins.
/// </summary>
public class PlayerPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly Task<LoginPacket> initLoginPacket;
    private readonly Task<PlayerPositionPacket> initPositionPacket;
    private readonly Task<SetHealthPacket> initHealthPacket;
    private EntityPlugin? entities;

    private PhysicsPlugin? physics;

    /// <summary>
    ///     Create a new PlayerPlugin instance
    /// </summary>
    /// <param name="bot"></param>
    public PlayerPlugin(MineSharpBot bot) : base(bot)
    {
        Players = new ConcurrentDictionary<Uuid, MinecraftPlayer>();
        PlayerMap = new ConcurrentDictionary<int, MinecraftPlayer>();

        Bot.Client.On<SetHealthPacket>(HandleSetHealthPacket);
        Bot.Client.On<CombatDeathPacket>(HandleCombatDeathPacket);
        Bot.Client.On<RespawnPacket>(HandleRespawnPacket);
        Bot.Client.On<SpawnPlayerPacket>(HandleSpawnPlayer);
        Bot.Client.On<PlayerInfoUpdatePacket>(HandlePlayerInfoUpdate);
        Bot.Client.On<PlayerInfoRemovePacket>(HandlePlayerInfoRemove);
        Bot.Client.On<GameEventPacket>(HandleGameEvent);
        Bot.Client.On<AcknowledgeBlockChangePacket>(HandleAcknowledgeBlockChange);
        Bot.Client.On<EntityStatusPacket>(HandleEntityStatus);

        // already start listening to the packets here, as they sometimes get lost when calling in init() 
        initLoginPacket = Bot.Client.WaitForPacket<LoginPacket>();
        initPositionPacket = Bot.Client.WaitForPacket<PlayerPositionPacket>();
        initHealthPacket = Bot.Client.WaitForPacket<SetHealthPacket>();
    }

    /// <summary>
    ///     The MinecraftPlayer representing the Minecraft Bot itself.
    /// </summary>
    public MinecraftPlayer? Self { get; private set; }

    /// <summary>
    ///     The Entity representing the Minecraft Bot itself.
    /// </summary>
    public Entity? Entity => Self?.Entity;

    /// <summary>
    ///     All players on the server.
    /// </summary>
    public IDictionary<Uuid, MinecraftPlayer> Players { get; }

    /// <summary>
    ///     Minecraft players indexed by <see cref="MineSharp.Core.Common.Entities.Entity.ServerId" />.
    ///     Contains only loaded players (Players in the Bots visible range)
    /// </summary>
    public IDictionary<int, MinecraftPlayer> PlayerMap { get; }

    /// <summary>
    ///     The Bots health (between 0.0 - 20.0)
    /// </summary>
    public float? Health { get; private set; }

    /// <summary>
    ///     The Bots saturation level
    /// </summary>
    public float? Saturation { get; private set; }

    /// <summary>
    ///     The Bots food level
    /// </summary>
    public float? Food { get; private set; }

    /// <summary>
    ///     The Name of the dimension the bot is currently in.
    /// </summary>
    public Identifier? DimensionName { get; private set; }

    /// <summary>
    ///     Whether the bot is alive or dead.
    /// </summary>
    public bool? IsAlive => Health > 0;

    // TODO: Maybe move weather related stuff to world?

    /// <summary>
    ///     Whether it is raining
    /// </summary>
    public bool IsRaining { get; set; }

    /// <summary>
    ///     The rain level
    /// </summary>
    public float RainLevel { get; set; }

    /// <summary>
    ///     The thunder level
    /// </summary>
    public float ThunderLevel { get; set; }


    /// <summary>
    ///     This event fires when the health, food or saturation has been updated.
    /// </summary>
    public AsyncEvent<MineSharpBot> OnHealthChanged = new();

    /// <summary>
    ///     This event fires when the bot respawned or joined another dimension.
    /// </summary>
    public AsyncEvent<MineSharpBot> OnRespawned = new();

    /// <summary>
    ///     This event fires when a the bot has died.
    /// </summary>
    public AsyncEvent<MineSharpBot> OnDied = new();

    /// <summary>
    ///     This event fires when a player joined the server.
    /// </summary>
    public AsyncEvent<MineSharpBot, MinecraftPlayer> OnPlayerJoined = new();

    /// <summary>
    ///     This event fires when a player left the server.
    /// </summary>
    public AsyncEvent<MineSharpBot, MinecraftPlayer> OnPlayerLeft = new();

    /// <summary>
    ///     This event fires when a player has come into visible range and their entity has been loaded.
    /// </summary>
    public AsyncEvent<MineSharpBot, MinecraftPlayer> OnPlayerLoaded = new();

    /// <summary>
    ///     This event fires when the weather in the world changes.
    /// </summary>
    public AsyncEvent<MineSharpBot> OnWeatherChanged = new();

    /// <inheritdoc />
    protected override async Task Init()
    {
        entities = Bot.GetPlugin<EntityPlugin>();

        await Task.WhenAll(initLoginPacket, initPositionPacket).WaitAsync(Bot.CancellationToken);

        var loginPacket = await initLoginPacket;
        var positionPacket = await initPositionPacket;

        var entity = new Entity(
            Bot.Data.Entities.ByType(EntityType.Player)!,
            loginPacket.EntityId,
            new MutableVector3(positionPacket.X, positionPacket.Y, positionPacket.Z),
            positionPacket.Pitch,
            positionPacket.Yaw,
            new MutableVector3(0, 0, 0),
            false,
            new());

        Self = new(
            Bot.Session.Username,
            Bot.Session.Uuid,
            0,
            (GameMode)loginPacket.GameMode,
            entity,
            ParseDimension(loginPacket.DimensionType));

        DimensionName = loginPacket.DimensionName;

        var healthPacket = await initHealthPacket.WaitAsync(Bot.CancellationToken);

        Health = healthPacket.Health;
        Food = healthPacket.Food;
        Saturation = healthPacket.Saturation;

        Logger.Info(
            "Initialized Bot Entity: Position=({Position}), GameMode={GameMode}, Health={Health}, Dimension={DimensionName} ({Dimension}).", Entity!.Position, Self.GameMode, Health, DimensionName, Self!.Dimension);

        PlayerMap.TryAdd(entity.ServerId, Self);

        if (Health > 0)
        {
            await Bot.Client.SendPacket(
                new SetPlayerPositionAndRotationPacket(
                    positionPacket.X,
                    positionPacket.Y,
                    positionPacket.Z,
                    positionPacket.Yaw,
                    positionPacket.Pitch,
                    Entity.IsOnGround));
        }

        try
        {
            physics = Bot.GetPlugin<PhysicsPlugin>();
        }
        catch (PluginNotLoadedException) { }
    }

    /// <summary>
    ///     Respawns the bot when its dead.
    /// </summary>
    /// <returns></returns>
    public Task Respawn()
    {
        return Bot.Client.SendPacket(new ClientCommandPacket(0));
    }


    /// <summary>
    ///     Plays a swinging arm animation for other players.
    /// </summary>
    /// <param name="hand">The hand to swing</param>
    /// <param name="token">optional cancellation token</param>
    /// <returns></returns>
    public Task SwingArm(PlayerHand hand = PlayerHand.MainHand, CancellationToken token = default)
    {
        var packet = new SwingArmPacket(hand);
        return Bot.Client.SendPacket(packet, token);
    }

    /// <summary>
    ///     Attacks the given entity with the currently equipped item
    /// </summary>
    /// <param name="entity">The entity to attack</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown when the entity is too far away.</exception>
    public Task Attack(Entity entity)
    {
        if (36 < Entity?.Position.DistanceToSquared(entity.Position))
        {
            throw new InvalidOperationException("Entity is too far away");
        }

        var packet = new InteractPacket(
            entity.ServerId,
            InteractPacket.InteractionType.Attack,
            physics?.Engine!.State.IsCrouching ?? false);

        return Task.WhenAll(
            Bot.Client.SendPacket(packet),
            SwingArm());
    }


    private Task HandleSetHealthPacket(SetHealthPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        Health = packet.Health;
        Food = packet.Food;
        Saturation = packet.Saturation;

        OnHealthChanged.Dispatch(Bot);
        if (Health == 0)
        {
            OnDied.Dispatch(Bot);
        }

        return Task.CompletedTask;
    }

    private Task HandleCombatDeathPacket(CombatDeathPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        Health = 0;

        // this.OnDied?.Invoke(this.Bot);
        return Task.CompletedTask;
    }

    private Task HandleRespawnPacket(RespawnPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        Self!.Dimension = ParseDimension(packet.DimensionType);

        OnRespawned.Dispatch(Bot);
        return Task.CompletedTask;
    }

    private Task HandleSpawnPlayer(SpawnPlayerPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        if (!Players.TryGetValue(packet.PlayerUuid, out var player))
        {
            Logger.Warn($"Received SpawnPlayer packet for unknown player: {packet.PlayerUuid}");
            return Task.CompletedTask;
        }

        var entity = new Entity(
            Bot.Data.Entities.ByType(EntityType.Player)!,
            packet.EntityId,
            new MutableVector3(
                packet.X,
                packet.Y,
                packet.Z),
            NetUtils.FromAngleByte((sbyte)packet.Pitch),
            NetUtils.FromAngleByte((sbyte)packet.Yaw),
            new MutableVector3(0, 0, 0),
            true,
            new());
        player.Entity = entity;

        entities!.AddEntity(entity);
        PlayerMap.TryAdd(player.Entity!.ServerId, player);

        OnPlayerLoaded.Dispatch(Bot, player);
        return Task.CompletedTask;
    }

    private Task HandlePlayerInfoUpdate(PlayerInfoUpdatePacket packet)
    {
        foreach (var entry in packet.Data)
        {
            foreach (var action in entry.Actions)
            {
                Players.TryGetValue(entry.Player, out var player);

                switch (action)
                {
                    case PlayerInfoUpdatePacket.AddPlayerAction addAction:
                        if (player != null)
                        {
                            player.Username = addAction.Name;
                            break;
                        }

                        var newPlayer = new MinecraftPlayer(addAction.Name, entry.Player, -1, GameMode.Survival, null,
                                                            Core.Common.Dimension.Overworld);
                        Players.TryAdd(entry.Player, newPlayer);
                        OnPlayerJoined.Dispatch(Bot, newPlayer);
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

                        if (!updateListedAction.Listed && Bot.Data.Version.Protocol <= ProtocolVersion.V_1_19_2)
                        {
                            OnPlayerLeft.Dispatch(Bot, player);
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
                        {
                            break;
                        }

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
            if (!Players.Remove(uuid, out var player))
            {
                continue;
            }

            OnPlayerLeft.Dispatch(Bot, player);
        }

        return Task.CompletedTask;
    }

    private Task HandleGameEvent(GameEventPacket packet)
    {
        switch (packet.Event)
        {
            case GameEventPacket.GameEvent.EndRaining:
                IsRaining = false;
                OnWeatherChanged.Dispatch(Bot);
                break;

            case GameEventPacket.GameEvent.BeginRaining:
                IsRaining = true;
                OnWeatherChanged.Dispatch(Bot);
                break;

            case GameEventPacket.GameEvent.RainLevelChange:
                RainLevel = packet.Value;
                OnWeatherChanged.Dispatch(Bot);
                break;

            case GameEventPacket.GameEvent.ThunderLevelChange:
                ThunderLevel = packet.Value;
                OnWeatherChanged.Dispatch(Bot);
                break;

            case GameEventPacket.GameEvent.ChangeGameMode:
                var gameMode = (GameMode)packet.Value;
                Self!.GameMode = gameMode;
                break;
        }

        return Task.CompletedTask;
    }

    private Task HandleAcknowledgeBlockChange(AcknowledgeBlockChangePacket packet)
    {
        if (packet.Body is AcknowledgeBlockChangePacket.PacketBody119 v119)
        {
            Bot.SequenceId = v119.SequenceId;
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
        if (!PlayerMap.TryGetValue(packet.EntityId, out var player))
        {
            return;
        }

        var permissionLevel = (PermissionLevel)(packet.Status - 24);
        player.PermissionLevel = permissionLevel;
    }

    private static readonly FrozenDictionary<Identifier, Dimension> DimensionByIdentifier = new Dictionary<Identifier, Dimension>
    {
        { Identifier.Parse("minecraft:overworld"), Dimension.Overworld },
        { Identifier.Parse("minecraft:the_nether"), Dimension.Nether },
        { Identifier.Parse("minecraft:the_end"), Dimension.End }
    }.ToFrozenDictionary();

    private Dimension ParseDimension(Identifier dimensionName)
    {
        if (!DimensionByIdentifier.TryGetValue(dimensionName, out var dimension))
        {
            throw new UnreachableException($"{nameof(dimensionName)} was: {dimensionName}");
        }
        return dimension;
    }
}
