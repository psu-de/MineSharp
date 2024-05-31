using MineSharp.Bot.Utils;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using System.Collections.Concurrent;
using MineSharp.Core.Geometry;

namespace MineSharp.Bot.Plugins;

/// <summary>
/// This Plugin handles all kinds of packets regarding entities.
/// </summary>
public class EntityPlugin : Plugin
{
    /// <summary>
    /// All Entities loaded by the client.
    /// </summary>
    public readonly IDictionary<int, Entity> Entities;

    /// <summary>
    /// Fires whenever an entity spawns in the bots visible range.
    /// </summary>
    public event Events.EntityEvent? OnEntitySpawned;

    /// <summary>
    /// Fires whenever an entity despawned in the bots visible range.
    /// </summary>
    public event Events.EntityEvent? OnEntityDespawned;

    /// <summary>
    /// Fires whenever an entity moved in the bots visible range.
    /// </summary>
    public event Events.EntityEvent? OnEntityMoved;

    private PlayerPlugin? _playerPlugin;

    /// <summary>
    /// Create a new EntityPlugin instance
    /// </summary>
    /// <param name="bot"></param>
    public EntityPlugin(MineSharpBot bot) : base(bot)
    {
        this.Entities = new ConcurrentDictionary<int, Entity>();

        this.Bot.Client.On<SpawnEntityPacket>(this.HandleSpawnEntityPacket);
        this.Bot.Client.On<SpawnLivingEntityPacket>(this.HandleSpawnLivingEntityPacket);
        this.Bot.Client.On<RemoveEntitiesPacket>(this.HandleRemoveEntitiesPacket);
        this.Bot.Client.On<SetEntityVelocityPacket>(this.HandleSetEntityVelocityPacket);
        this.Bot.Client.On<EntityPositionPacket>(this.HandleUpdateEntityPositionPacket);
        this.Bot.Client.On<EntityPositionAndRotationPacket>(this.HandleUpdateEntityPositionAndRotationPacket);
        this.Bot.Client.On<EntityRotationPacket>(this.HandleUpdateEntityRotationPacket);
        this.Bot.Client.On<TeleportEntityPacket>(this.HandleTeleportEntityPacket);
        this.Bot.Client.On<UpdateAttributesPacket>(this.HandleUpdateAttributesPacket);
        this.Bot.Client.On<PlayerPositionPacket>(this.HandleSynchronizePlayerPosition);
        this.Bot.Client.On<SetPassengersPacket>(this.HandleSetPassengersPacket);
    }

    /// <inheritdoc />
    protected override async Task Init()
    {
        this._playerPlugin = this.Bot.GetPlugin<PlayerPlugin>();
        await this._playerPlugin.WaitForInitialization();
    }

    internal void AddEntity(Entity entity)
    {
        this.Entities.TryAdd(entity.ServerId, entity);

        if (null != this.OnEntitySpawned)
        {
            Task.Run(() => this.OnEntitySpawned?.Invoke(this.Bot, entity));
        }
    }

    private void MountEntity(Entity? vehicle, Entity passenger)
    {
        Entity? originalVehicle = passenger.Vehicle;
        if (originalVehicle != null)
        {
            originalVehicle.Passengers.Remove(passenger);
        }
        passenger.Vehicle = vehicle;
        if (vehicle != null)
        {
            vehicle.Passengers.Add(passenger);
        }
    }

    private Task HandleSpawnLivingEntityPacket(SpawnLivingEntityPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        var entityInfo = this.Bot.Data.Entities.ById(packet.EntityType)!;

        var newEntity = new Entity(
            entityInfo, packet.EntityId, new MutableVector3(packet.X, packet.Y, packet.Z),
            packet.Pitch,
            packet.Yaw,
            new MutableVector3(
                NetUtils.ConvertToVelocity(packet.VelocityX),
                NetUtils.ConvertToVelocity(packet.VelocityY),
                NetUtils.ConvertToVelocity(packet.VelocityZ)),
            true,
            new Dictionary<EffectType, Effect?>());

        this.AddEntity(newEntity);
        return Task.CompletedTask;
    }

    private Task HandleSpawnEntityPacket(SpawnEntityPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        var entityInfo = this.Bot.Data.Entities.ById(packet.EntityType)!;

        var newEntity = new Entity(
            entityInfo, packet.EntityId, new MutableVector3(packet.X, packet.Y, packet.Z),
            packet.Pitch,
            packet.Yaw,
            new MutableVector3(
                NetUtils.ConvertToVelocity(packet.VelocityX),
                NetUtils.ConvertToVelocity(packet.VelocityY),
                NetUtils.ConvertToVelocity(packet.VelocityZ)),
            true,
            new Dictionary<EffectType, Effect?>());

        this.AddEntity(newEntity);
        return Task.CompletedTask;
    }

    private Task HandleRemoveEntitiesPacket(RemoveEntitiesPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        foreach (var entityId in packet.EntityIds)
        {
            if (!this.Entities.Remove(entityId, out var entity))
                continue;

            this.OnEntityDespawned?.Invoke(this.Bot, entity);
        }

        return Task.CompletedTask;
    }

    private Task HandleSetEntityVelocityPacket(SetEntityVelocityPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        if (!this.Entities.TryGetValue(packet.EntityId, out var entity))
        {
            return Task.CompletedTask;
        }

        (entity.Position as MutableVector3)!.Add(
            NetUtils.ConvertToVelocity(packet.VelocityX),
            NetUtils.ConvertToVelocity(packet.VelocityY),
            NetUtils.ConvertToVelocity(packet.VelocityZ)
        );
        
        return Task.CompletedTask;
    }

    private Task HandleUpdateEntityPositionPacket(EntityPositionPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        if (!this.Entities.TryGetValue(packet.EntityId, out var entity))
            return Task.CompletedTask;

        (entity.Position as MutableVector3)!.Set(
            NetUtils.ConvertDeltaPosition(packet.DeltaX),
            NetUtils.ConvertDeltaPosition(packet.DeltaY),
            NetUtils.ConvertDeltaPosition(packet.DeltaZ)
            );

        entity.IsOnGround = packet.OnGround;

        this.OnEntityMoved?.Invoke(this.Bot, entity);
        return Task.CompletedTask;
    }

    private Task HandleUpdateEntityPositionAndRotationPacket(EntityPositionAndRotationPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        if (!this.Entities.TryGetValue(packet.EntityId, out var entity))
            return Task.CompletedTask;

        (entity.Position as MutableVector3)!.Add(
            NetUtils.ConvertDeltaPosition(packet.DeltaX),
            NetUtils.ConvertDeltaPosition(packet.DeltaY),
            NetUtils.ConvertDeltaPosition(packet.DeltaZ));

        entity.Yaw        = NetUtils.FromAngleByte(packet.Yaw);
        entity.Pitch      = NetUtils.FromAngleByte(packet.Pitch);
        entity.IsOnGround = packet.OnGround;

        this.OnEntityMoved?.Invoke(this.Bot, entity);
        return Task.CompletedTask;
    }

    private Task HandleUpdateEntityRotationPacket(EntityRotationPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        if (!this.Entities.TryGetValue(packet.EntityId, out var entity))
            return Task.CompletedTask;

        entity.Yaw        = NetUtils.FromAngleByte(packet.Yaw);
        entity.Pitch      = NetUtils.FromAngleByte(packet.Pitch);
        entity.IsOnGround = packet.OnGround;

        this.OnEntityMoved?.Invoke(this.Bot, entity);

        return Task.CompletedTask;
    }

    private Task HandleTeleportEntityPacket(TeleportEntityPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        if (!this.Entities.TryGetValue(packet.EntityId, out var entity))
            return Task.CompletedTask;

        (entity.Position as MutableVector3)!
           .Set(packet.X, packet.Y, packet.Z);

        entity.Yaw   = NetUtils.FromAngleByte(packet.Yaw);
        entity.Pitch = NetUtils.FromAngleByte(packet.Pitch);
        this.OnEntityMoved?.Invoke(this.Bot, entity);

        return Task.CompletedTask;
    }

    private Task HandleUpdateAttributesPacket(UpdateAttributesPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;

        if (!this.Entities.TryGetValue(packet.EntityId, out var entity))
            return Task.CompletedTask;

        foreach (var attribute in packet.Attributes)
        {
            if (!entity.Attributes.TryAdd(attribute.Key, attribute))
            {
                entity.Attributes[attribute.Key] = attribute;
            }
        }

        return Task.CompletedTask;
    }

    private async Task HandleSynchronizePlayerPosition(PlayerPositionPacket packet)
    {
        if (!this.IsEnabled)
            return;

        await this.WaitForInitialization();

        var position = (this._playerPlugin!.Entity!.Position as MutableVector3)!;

        if ((packet.Flags & 0x01) == 0x01)
            position.Add(packet.X, 0, 0);
        else
            position.SetX(packet.X);

        if ((packet.Flags & 0x02) == 0x02)
            position.Add(0, packet.Y, 0);
        else
            position.SetY(packet.Y);

        if ((packet.Flags & 0x04) == 0x04)
            position.Add(0, 0, packet.Z);
        else
            position.SetZ(packet.Z);

        if ((packet.Flags & 0x08) == 0x08)
            this._playerPlugin!.Entity!.Pitch += packet.Pitch;
        else
            this._playerPlugin!.Entity!.Pitch = packet.Pitch;

        if ((packet.Flags & 0x10) == 0x10)
            this._playerPlugin!.Entity!.Yaw += packet.Yaw;
        else
            this._playerPlugin!.Entity!.Yaw = packet.Yaw;

        // TODO: Dismount Vehicle

        await this.Bot.Client.SendPacket(new ConfirmTeleportPacket(packet.TeleportId));
    }

    private Task HandleSetPassengersPacket(SetPassengersPacket packet)
    {
        if (packet.EntityId != -1 || !this.Entities.TryGetValue(packet.EntityId, out var vehicle))
        {
            return Task.CompletedTask;
        }
        vehicle = packet.EntityId == -1 ? null : vehicle;

        foreach (int passengerId in packet.PassengersId)
        {
            if (!this.Entities.TryGetValue(packet.EntityId, out var passenger))
            {
                continue;
            }
            MountEntity(vehicle, passenger);
        }
        return Task.CompletedTask;
    }
}
