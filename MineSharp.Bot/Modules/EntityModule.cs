using MineSharp.Core;
using MineSharp.Core.Types;
using MineSharp.Data.Effects;
using MineSharp.Data.Entities;
using MineSharp.Data.Protocol.Play.Clientbound;
using System.Collections.Concurrent;
using static MineSharp.Bot.MinecraftBot;

namespace MineSharp.Bot.Modules {
    public class EntityModule : Module {
        public EntityModule(MinecraftBot bot) : base(bot) { }

        public event BotEntityEvent? EntitySpawned;

        public event BotEntityEvent? EntityDespawned;

        public event BotEntityEvent? EntityMoved;

        public event BotEntityEvent? EntityEffectChanged;


        public ConcurrentDictionary<int, Entity> Entities = new ConcurrentDictionary<int, Entity>();

        protected async override Task Load() {

            this.Bot.On<PacketSpawnEntityLiving>(handleSpawnLivingEntity);
            this.Bot.On<PacketEntityDestroy>(handleDestroyEntities);
            this.Bot.On<PacketEntityVelocity>(handleUpdateEntityVelocity);
            this.Bot.On<PacketRelEntityMove>(handleEntityPosition);
            this.Bot.On<PacketEntityMoveLook>(handleEntityPositionAndRotation);
            this.Bot.On<PacketEntityLook>(handleEntityRotation);
            this.Bot.On<PacketEntityTeleport>(handleEntityTeleport);
            this.Bot.On<PacketEntityEffect>(handleEntityEffect);
            this.Bot.On<PacketRemoveEntityEffect>(handleRemoveEntityEffect);

            await this.SetEnabled(true);
        }

        private Task handleSpawnLivingEntity(PacketSpawnEntityLiving packet) {

            var newEntity = EntityFactory.CreateEntity(
                packet.Type!, packet.EntityId!, new Vector3(packet.X!, packet.Y!, packet.Z!),
                packet.Pitch!,
                packet.Yaw!,
                new Vector3(packet.VelocityX! / MinecraftConst.VelocityToBlock, packet.VelocityY! / MinecraftConst.VelocityToBlock, packet.VelocityZ! / MinecraftConst.VelocityToBlock),
                true,
                new Dictionary<int, Effect?>());

            Entities.TryAdd(packet.EntityId!, newEntity);
            this.EntitySpawned?.Invoke(this.Bot, newEntity);
            return Task.CompletedTask;
        }

        private Task handleDestroyEntities(PacketEntityDestroy packet) {
            for (int i = 0; i < packet.EntityIds!.Length; i++) {
                if (!this.Entities.TryRemove(packet.EntityIds[i], out var entity)) 
                    continue; 
                this.EntityDespawned?.Invoke(this.Bot, entity);
            }
            return Task.CompletedTask;  
        }

        private Task handleUpdateEntityVelocity(PacketEntityVelocity packet) {
            if (!Entities.TryGetValue(packet.EntityId!, out var entity))
                return Task.CompletedTask;

            entity.Velocity = new Vector3(packet.VelocityX! / MinecraftConst.VelocityToBlock, packet.VelocityY! / MinecraftConst.VelocityToBlock, packet.VelocityZ! / MinecraftConst.VelocityToBlock);
            return Task.CompletedTask;
        }

        private Task handleEntityPosition(PacketRelEntityMove packet) {

            if (!Entities.TryGetValue(packet.EntityId!, out var entity)) 
                return Task.CompletedTask;

            entity.Position.Add(new Vector3(packet.DX! / (128 * 32d), packet.DY! / (128 * 32d), packet.DZ! / (128 * 32d)));
            entity.IsOnGround = packet.OnGround!;

            EntityMoved?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityPositionAndRotation(PacketEntityMoveLook packet) {

            if (!Entities.TryGetValue(packet.EntityId!, out var entity)) 
                return Task.CompletedTask;

            entity.Position.Add(new Vector3(packet.DX! / (128 * 32d), packet.DY! / (128 * 32d), packet.DZ! / (128 * 32d)));
            entity.Yaw = packet.Yaw!;
            entity.Pitch = packet.Pitch!;
            entity.IsOnGround = packet.OnGround!;

            EntityMoved?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityRotation(PacketEntityLook packet) {

            if (!Entities.TryGetValue(packet.EntityId!, out var entity)) 
                return Task.CompletedTask;

            entity.Yaw = packet.Yaw!;
            entity.Pitch = packet.Pitch!;
            entity.IsOnGround = packet.OnGround!;

            EntityMoved?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityTeleport(PacketEntityTeleport packet) {
            if (!Entities.TryGetValue(packet.EntityId!, out var entity)) 
                return Task.CompletedTask;

            entity.Position.X = packet.X!;
            entity.Position.Y = packet.Y!;
            entity.Position.Z = packet.Z!;

            entity.Yaw = packet.Yaw!;
            entity.Pitch = packet.Pitch!;
            this.EntityMoved?.Invoke(this.Bot, entity);

            return Task.CompletedTask;
        }

        private Task handleEntityEffect(PacketEntityEffect packet) {
            if (!this.Entities.TryGetValue(packet.EntityId!, out var entity)) 
                return Task.CompletedTask;
            var effectType = EffectPalette.GetEffectTypeById(packet.EffectId!);
            var effect = EffectFactory.CreateEffect((int)packet.EffectId!, packet.Amplifier!, DateTime.Now, packet.Duration!);

            //TODO: Effect updating is so noch nich ganz richtig
            if (entity.Effects.ContainsKey(packet.EffectId!) && effect.Amplifier >= entity.Effects[packet.EffectId!]!.Amplifier) {
                entity.Effects[packet.EffectId!] = effect;
            } else {
                entity.Effects.Add(packet.EffectId!, effect);
            }

            EntityEffectChanged?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleRemoveEntityEffect(PacketRemoveEntityEffect packet) {
            if (!this.Entities.TryGetValue(packet.EffectId!, out var entity)) 
                return Task.CompletedTask;

            if (entity.Effects.ContainsKey(packet.EffectId!)) {
                entity.Effects.Remove(packet.EffectId!);
            }
            EntityEffectChanged?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }


        internal void AddEntity(Entity entity) {
            this.Entities.TryAdd(entity.Id, entity);
        }
    }
}
