using MineSharp.Core;
using MineSharp.Core.Types;
using MineSharp.Data.Effects;
using MineSharp.Data.Entities;
using MineSharp.Data.Protocol.Play.Clientbound;
using System.Collections.Concurrent;
using static MineSharp.Bot.MinecraftBot;
using Attribute = MineSharp.Core.Types.Attribute;

namespace MineSharp.Bot.Modules.Entity
{
    public class EntityModule : Module
    {


        public ConcurrentDictionary<int, Core.Types.Entity> Entities = new ConcurrentDictionary<int, Core.Types.Entity>();
        public EntityModule(MinecraftBot bot) : base(bot) {}

        public event BotEntityEvent? EntitySpawned;

        public event BotEntityEvent? EntityDespawned;

        public event BotEntityEvent? EntityMoved;

        public event BotEntityEvent? EntityEffectChanged;

        protected override async Task Load()
        {
            await this.Bot.WaitForBot();

            this.Bot.On<PacketSpawnEntityLiving>(this.handleSpawnLivingEntity);
            this.Bot.On<PacketEntityDestroy>(this.handleDestroyEntities);
            this.Bot.On<PacketEntityVelocity>(this.handleUpdateEntityVelocity);
            this.Bot.On<PacketRelEntityMove>(this.handleEntityPosition);
            this.Bot.On<PacketEntityMoveLook>(this.handleEntityPositionAndRotation);
            this.Bot.On<PacketEntityLook>(this.handleEntityRotation);
            this.Bot.On<PacketEntityTeleport>(this.handleEntityTeleport);
            this.Bot.On<PacketEntityEffect>(this.handleEntityEffect);
            this.Bot.On<PacketRemoveEntityEffect>(this.handleRemoveEntityEffect);
            this.Bot.On<PacketEntityUpdateAttributes>(this.handleEntityUpdateAttributes);

            await this.SetEnabled(true);
        }

        private Task handleSpawnLivingEntity(PacketSpawnEntityLiving packet)
        {

            var newEntity = new Core.Types.Entity(
                EntityPalette.GetEntityInfoById(packet.Type!), packet.EntityId!, new Vector3(packet.X!, packet.Y!, packet.Z!),
                packet.Pitch!,
                packet.Yaw!,
                new Vector3(packet.VelocityX! / MinecraftConst.VelocityToBlock, packet.VelocityY! / MinecraftConst.VelocityToBlock, packet.VelocityZ! / MinecraftConst.VelocityToBlock),
                true,
                new Dictionary<int, Effect?>());

            this.Entities.TryAdd(packet.EntityId!, newEntity);
            this.EntitySpawned?.Invoke(this.Bot, newEntity);
            return Task.CompletedTask;
        }

        private Task handleDestroyEntities(PacketEntityDestroy packet)
        {
            for (var i = 0; i < packet.EntityIds!.Length; i++)
            {
                if (!this.Entities.TryRemove(packet.EntityIds[i], out var entity))
                    continue;
                this.EntityDespawned?.Invoke(this.Bot, entity);
            }
            return Task.CompletedTask;
        }

        private Task handleUpdateEntityVelocity(PacketEntityVelocity packet)
        {
            if (!this.Entities.TryGetValue(packet.EntityId!, out var entity))
                return Task.CompletedTask;

            entity.Velocity = new Vector3(packet.VelocityX! / MinecraftConst.VelocityToBlock, packet.VelocityY! / MinecraftConst.VelocityToBlock, packet.VelocityZ! / MinecraftConst.VelocityToBlock);
            return Task.CompletedTask;
        }

        private Task handleEntityPosition(PacketRelEntityMove packet)
        {

            if (!this.Entities.TryGetValue(packet.EntityId!, out var entity))
                return Task.CompletedTask;

            entity.Position.Add(new Vector3(packet.DX! / (128 * 32d), packet.DY! / (128 * 32d), packet.DZ! / (128 * 32d)));
            entity.IsOnGround = packet.OnGround!;

            this.EntityMoved?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityPositionAndRotation(PacketEntityMoveLook packet)
        {

            if (!this.Entities.TryGetValue(packet.EntityId!, out var entity))
                return Task.CompletedTask;

            entity.Position.Add(new Vector3(packet.DX! / (128 * 32d), packet.DY! / (128 * 32d), packet.DZ! / (128 * 32d)));
            entity.Yaw = packet.Yaw!;
            entity.Pitch = packet.Pitch!;
            entity.IsOnGround = packet.OnGround!;

            this.EntityMoved?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityRotation(PacketEntityLook packet)
        {

            if (!this.Entities.TryGetValue(packet.EntityId!, out var entity))
                return Task.CompletedTask;

            entity.Yaw = packet.Yaw!;
            entity.Pitch = packet.Pitch!;
            entity.IsOnGround = packet.OnGround!;

            this.EntityMoved?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityTeleport(PacketEntityTeleport packet)
        {
            if (!this.Entities.TryGetValue(packet.EntityId!, out var entity))
                return Task.CompletedTask;

            entity.Position.X = packet.X!;
            entity.Position.Y = packet.Y!;
            entity.Position.Z = packet.Z!;

            entity.Yaw = packet.Yaw!;
            entity.Pitch = packet.Pitch!;
            this.EntityMoved?.Invoke(this.Bot, entity);

            return Task.CompletedTask;
        }

        private Task handleEntityEffect(PacketEntityEffect packet)
        {
            if (!this.Entities.TryGetValue(packet.EntityId!, out var entity))
                return Task.CompletedTask;
            var effectType = EffectPalette.GetEffectInfoById(packet.EffectId!);
            var effect = new Effect(effectType, packet.Amplifier!, DateTime.Now, packet.Duration!);

            //TODO: Effect updating is so noch nich ganz richtig
            if (entity.Effects.ContainsKey(packet.EffectId!) && effect.Amplifier >= entity.Effects[packet.EffectId!]!.Amplifier)
            {
                entity.Effects[packet.EffectId!] = effect;
            } else
            {
                entity.Effects.Add(packet.EffectId!, effect);
            }

            this.EntityEffectChanged?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleRemoveEntityEffect(PacketRemoveEntityEffect packet)
        {
            if (!this.Entities.TryGetValue(packet.EffectId!, out var entity))
                return Task.CompletedTask;

            if (entity.Effects.ContainsKey(packet.EffectId!))
            {
                entity.Effects.Remove(packet.EffectId!);
            }
            this.EntityEffectChanged?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityUpdateAttributes(PacketEntityUpdateAttributes packet)
        {
            if (!this.Entities.TryGetValue(packet.EntityId, out var entity))
                return Task.CompletedTask;

            foreach (var attr in packet.Properties)
            {
                var attribute = new Attribute(attr.Key,
                    attr.Value,
                    attr.Modifiers
                        .Select(m =>
                            new Modifier(m.Uuid, m.Amount, (ModifierOp)m.Operation)).ToList());
                if (entity.Attributes.ContainsKey(attr.Key))
                {
                    entity.Attributes[attr.Key] = attribute;
                } else
                {
                    entity.Attributes.Add(attr.Key, attribute);
                }
            }


            return Task.CompletedTask;
        }


        internal void AddEntity(Core.Types.Entity entity)
        {
            this.Entities.TryAdd(entity.Info.Id, entity);
        }
    }
}
