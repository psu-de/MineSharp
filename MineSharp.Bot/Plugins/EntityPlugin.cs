using System.Collections.Concurrent;
using MineSharp.Bot.Utils;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Events;
using MineSharp.Core.Geometry;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;

namespace MineSharp.Bot.Plugins;

/// <summary>
///     This Plugin handles all kinds of packets regarding entities.
/// </summary>
public class EntityPlugin : Plugin
{
    /// <summary>
    ///     All Entities loaded by the client.
    /// </summary>
    public readonly IDictionary<int, Entity> Entities;

    private PlayerPlugin? playerPlugin;

    /// <summary>
    ///     Create a new EntityPlugin instance
    /// </summary>
    /// <param name="bot"></param>
    public EntityPlugin(MineSharpBot bot) : base(bot)
    {
        Entities = new ConcurrentDictionary<int, Entity>();

        Bot.Client.On<SpawnEntityPacket>(HandleSpawnEntityPacket);
        Bot.Client.On<SpawnLivingEntityPacket>(HandleSpawnLivingEntityPacket);
        Bot.Client.On<RemoveEntitiesPacket>(HandleRemoveEntitiesPacket);
        Bot.Client.On<SetEntityVelocityPacket>(HandleSetEntityVelocityPacket);
        Bot.Client.On<EntityPositionPacket>(HandleUpdateEntityPositionPacket);
        Bot.Client.On<EntityPositionAndRotationPacket>(HandleUpdateEntityPositionAndRotationPacket);
        Bot.Client.On<EntityRotationPacket>(HandleUpdateEntityRotationPacket);
        Bot.Client.On<TeleportEntityPacket>(HandleTeleportEntityPacket);
        Bot.Client.On<UpdateAttributesPacket>(HandleUpdateAttributesPacket);
        Bot.Client.On<PlayerPositionPacket>(HandleSynchronizePlayerPosition);
        Bot.Client.On<SetPassengersPacket>(HandleSetPassengersPacket);
    }

    /// <summary>
    ///     Fires whenever an entity spawns in the bots visible range.
    /// </summary>
    public AsyncEvent<MineSharpBot, Entity> OnEntitySpawned = new();

    /// <summary>
    ///     Fires whenever an entity despawned in the bots visible range.
    /// </summary>
    public AsyncEvent<MineSharpBot, Entity> OnEntityDespawned = new();

    /// <summary>
    ///     Fires whenever an entity moved in the bots visible range.
    /// </summary>
    public AsyncEvent<MineSharpBot, Entity> OnEntityMoved = new();

    /// <inheritdoc />
    protected override async Task Init()
    {
        playerPlugin = Bot.GetPlugin<PlayerPlugin>();
        await playerPlugin.WaitForInitialization().WaitAsync(Bot.CancellationToken);
    }

    internal void AddEntity(Entity entity)
    {
        Entities.TryAdd(entity.ServerId, entity);
        OnEntitySpawned.Dispatch(Bot, entity);
    }

    private void MountEntity(Entity? vehicle, Entity passenger)
    {
        DismountEntity(passenger);

        passenger.Vehicle = vehicle;
        if (vehicle != null)
        {
            vehicle.Passengers.Add(passenger);
        }
    }

    private void DismountEntity(Entity passenger)
    {
        var originalVehicle = passenger.Vehicle;
        if (originalVehicle != null)
        {
            originalVehicle.Passengers.Remove(passenger);
        }

        passenger.Vehicle = null;
    }

    private void DismountPassengers(Entity vehicle)
    {
        foreach (var passenger in vehicle.Passengers)
        {
            DismountEntity(passenger);
        }

        vehicle.Passengers.Clear();
    }

    private Task HandleSpawnLivingEntityPacket(SpawnLivingEntityPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        var entityInfo = Bot.Data.Entities.ById(packet.EntityType)!;

        var newEntity = new Entity(
            entityInfo, packet.EntityId, new MutableVector3(packet.X, packet.Y, packet.Z),
            packet.Pitch,
            packet.Yaw,
            new MutableVector3(
                NetUtils.ConvertToVelocity(packet.VelocityX),
                NetUtils.ConvertToVelocity(packet.VelocityY),
                NetUtils.ConvertToVelocity(packet.VelocityZ)),
            true,
            new());

        AddEntity(newEntity);
        return Task.CompletedTask;
    }

    private Task HandleSpawnEntityPacket(SpawnEntityPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        var entityInfo = Bot.Data.Entities.ById(packet.EntityType)!;

        var newEntity = new Entity(
            entityInfo, packet.EntityId, new MutableVector3(packet.X, packet.Y, packet.Z),
            packet.Pitch,
            packet.Yaw,
            new MutableVector3(
                NetUtils.ConvertToVelocity(packet.VelocityX),
                NetUtils.ConvertToVelocity(packet.VelocityY),
                NetUtils.ConvertToVelocity(packet.VelocityZ)),
            true,
            new());

        AddEntity(newEntity);
        return Task.CompletedTask;
    }

    private Task HandleRemoveEntitiesPacket(RemoveEntitiesPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        foreach (var entityId in packet.EntityIds)
        {
            if (!Entities.Remove(entityId, out var entity))
            {
                continue;
            }

            DismountPassengers(entity);

            OnEntityDespawned.Dispatch(Bot, entity);
        }

        return Task.CompletedTask;
    }

    private Task HandleSetEntityVelocityPacket(SetEntityVelocityPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        if (!Entities.TryGetValue(packet.EntityId, out var entity))
        {
            return Task.CompletedTask;
        }

        (entity.Velocity as MutableVector3)!.Set(
            NetUtils.ConvertToVelocity(packet.VelocityX),
            NetUtils.ConvertToVelocity(packet.VelocityY),
            NetUtils.ConvertToVelocity(packet.VelocityZ)
        );

        return Task.CompletedTask;
    }

    private Task HandleUpdateEntityPositionPacket(EntityPositionPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        if (!Entities.TryGetValue(packet.EntityId, out var entity))
        {
            return Task.CompletedTask;
        }

        (entity.Position as MutableVector3)!.Add(
            NetUtils.ConvertDeltaPosition(packet.DeltaX),
            NetUtils.ConvertDeltaPosition(packet.DeltaY),
            NetUtils.ConvertDeltaPosition(packet.DeltaZ)
        );

        entity.IsOnGround = packet.OnGround;

        _ = OnEntityMoved.Dispatch(Bot, entity);
        return Task.CompletedTask;
    }

    private Task HandleUpdateEntityPositionAndRotationPacket(EntityPositionAndRotationPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        if (!Entities.TryGetValue(packet.EntityId, out var entity))
        {
            return Task.CompletedTask;
        }

        (entity.Position as MutableVector3)!.Add(
            NetUtils.ConvertDeltaPosition(packet.DeltaX),
            NetUtils.ConvertDeltaPosition(packet.DeltaY),
            NetUtils.ConvertDeltaPosition(packet.DeltaZ));

        entity.Yaw = NetUtils.FromAngleByte(packet.Yaw);
        entity.Pitch = NetUtils.FromAngleByte(packet.Pitch);
        entity.IsOnGround = packet.OnGround;

        _ = OnEntityMoved.Dispatch(Bot, entity);
        return Task.CompletedTask;
    }

    private Task HandleUpdateEntityRotationPacket(EntityRotationPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        if (!Entities.TryGetValue(packet.EntityId, out var entity))
        {
            return Task.CompletedTask;
        }

        entity.Yaw = NetUtils.FromAngleByte(packet.Yaw);
        entity.Pitch = NetUtils.FromAngleByte(packet.Pitch);
        entity.IsOnGround = packet.OnGround;

        _ = OnEntityMoved.Dispatch(Bot, entity);
        return Task.CompletedTask;
    }

    private Task HandleTeleportEntityPacket(TeleportEntityPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        if (!Entities.TryGetValue(packet.EntityId, out var entity))
        {
            return Task.CompletedTask;
        }

        (entity.Position as MutableVector3)!
           .Set(packet.X, packet.Y, packet.Z);

        entity.Yaw = NetUtils.FromAngleByte(packet.Yaw);
        entity.Pitch = NetUtils.FromAngleByte(packet.Pitch);

        _ = OnEntityMoved.Dispatch(Bot, entity);
        return Task.CompletedTask;
    }

    private Task HandleUpdateAttributesPacket(UpdateAttributesPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        if (!Entities.TryGetValue(packet.EntityId, out var entity))
        {
            return Task.CompletedTask;
        }

        foreach (var attribute in packet.Attributes)
        {
            // must not be Attributes[Key] = attribute because this might cause an exception
            entity.Attributes.AddOrUpdate(attribute.Key, attribute, (key, oldvalue) => attribute);
        }

        return Task.CompletedTask;
    }

    private async Task HandleSynchronizePlayerPosition(PlayerPositionPacket packet)
    {
        if (!IsEnabled)
        {
            return;
        }

        await WaitForInitialization();

        var position = (playerPlugin!.Entity!.Position as MutableVector3)!;

        if (packet.Flags.HasFlag(PlayerPositionPacket.PositionFlags.X))
        {
            position.Add(packet.X, 0, 0);
        }
        else
        {
            position.SetX(packet.X);
        }

        if (packet.Flags.HasFlag(PlayerPositionPacket.PositionFlags.Y))
        {
            position.Add(0, packet.Y, 0);
        }
        else
        {
            position.SetY(packet.Y);
        }

        if (packet.Flags.HasFlag(PlayerPositionPacket.PositionFlags.Z))
        {
            position.Add(0, 0, packet.Z);
        }
        else
        {
            position.SetZ(packet.Z);
        }

        if (packet.Flags.HasFlag(PlayerPositionPacket.PositionFlags.YRot))
        {
            playerPlugin!.Entity!.Pitch += packet.Pitch;
        }
        else
        {
            playerPlugin!.Entity!.Pitch = packet.Pitch;
        }

        if (packet.Flags.HasFlag(PlayerPositionPacket.PositionFlags.XRot))
        {
            playerPlugin!.Entity!.Yaw += packet.Yaw;
        }
        else
        {
            playerPlugin!.Entity!.Yaw = packet.Yaw;
        }

        // TODO: Dismount Vehicle

        await Bot.Client.SendPacket(new ConfirmTeleportPacket(packet.TeleportId));
    }

    private Task HandleSetPassengersPacket(SetPassengersPacket packet)
    {
        if (packet.EntityId != -1 || !Entities.TryGetValue(packet.EntityId, out var vehicle))
        {
            return Task.CompletedTask;
        }

        vehicle = packet.EntityId == -1 ? null : vehicle;

        foreach (var passengerId in packet.PassengersId)
        {
            if (!Entities.TryGetValue(packet.EntityId, out var passenger))
            {
                continue;
            }

            MountEntity(vehicle, passenger);
        }

        return Task.CompletedTask;
    }
}
