using MineSharp.Bot.Utils;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using System.Collections.Concurrent;

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
        this.Bot.Client.On<RemoveEntitiesPacket>(this.HandleRemoveEntitiesPacket);
        this.Bot.Client.On<SetEntityVelocityPacket>(this.HandleSetEntityVelocityPacket);
        this.Bot.Client.On<EntityPositionPacket>(this.HandleUpdateEntityPositionPacket);
        this.Bot.Client.On<EntityPositionAndRotationPacket>(this.HandleUpdateEntityPositionAndRotationPacket);
        this.Bot.Client.On<EntityRotationPacket>(this.HandleUpdateEntityRotationPacket);
        this.Bot.Client.On<TeleportEntityPacket>(this.HandleTeleportEntityPacket);
        this.Bot.Client.On<UpdateAttributesPacket>(this.HandleUpdateAttributesPacket);
        this.Bot.Client.On<PlayerPositionPacket>(this.HandleSynchronizePlayerPosition);
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
        this.OnEntitySpawned?.Invoke(this.Bot, entity);

        if (null != this.OnEntitySpawned)
            _ = Task.Factory.FromAsync(
                (callback, obj) => this.OnEntitySpawned.BeginInvoke(this.Bot, entity, callback, obj),
                this.OnEntitySpawned.EndInvoke,
                null);
    }

    private Task HandleSpawnEntityPacket(SpawnEntityPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;
        
        var entityInfo = this.Bot.Data.Entities.GetById(packet.EntityType);
        
        var newEntity = new Entity(
            entityInfo, packet.EntityId, new Vector3(packet.X, packet.Y, packet.Z),
            packet.Pitch,
            packet.Yaw,
            new Vector3(
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

        entity.Velocity.X = NetUtils.ConvertToVelocity(packet.VelocityX);
        entity.Velocity.Y = NetUtils.ConvertToVelocity(packet.VelocityY);
        entity.Velocity.Z = NetUtils.ConvertToVelocity(packet.VelocityZ);
        
        return Task.CompletedTask;
    }

    private Task HandleUpdateEntityPositionPacket(EntityPositionPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;
        
        if (!this.Entities.TryGetValue(packet.EntityId, out var entity))
            return Task.CompletedTask;

        entity.Position.Add(new Vector3(
            NetUtils.ConvertDeltaPosition(packet.DeltaX),
            NetUtils.ConvertDeltaPosition(packet.DeltaY),
            NetUtils.ConvertDeltaPosition(packet.DeltaZ)));
        
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

        entity.Position.Add(new Vector3(
            NetUtils.ConvertDeltaPosition(packet.DeltaX),
            NetUtils.ConvertDeltaPosition(packet.DeltaY),
            NetUtils.ConvertDeltaPosition(packet.DeltaZ)));

        entity.Yaw = NetUtils.FromAngleByte(packet.Yaw);
        entity.Pitch = NetUtils.FromAngleByte(packet.Pitch);
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

        entity.Yaw = NetUtils.FromAngleByte(packet.Yaw);
        entity.Pitch = NetUtils.FromAngleByte(packet.Pitch);
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

        entity.Position.X = packet.X;
        entity.Position.Y = packet.Y;
        entity.Position.Z = packet.Z;

        entity.Yaw = NetUtils.FromAngleByte(packet.Yaw);
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
        
        if ((packet.Flags & 0x01) == 0x01) 
            this._playerPlugin!.Entity!.Position.X += packet.X;
        else 
            this._playerPlugin!.Entity!.Position.X = packet.X;

        if ((packet.Flags & 0x02) == 0x02) 
            this._playerPlugin!.Entity!.Position.Y += packet.Y;
        else 
            this._playerPlugin!.Entity!.Position.Y = packet.Y;

        if ((packet.Flags & 0x04) == 0x04) 
            this._playerPlugin!.Entity!.Position.Z += packet.Z;
        else 
            this._playerPlugin!.Entity!.Position.Z = packet.Z;

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
}
