using MineSharp.Core;
using MineSharp.Core.Types;
using MineSharp.Data.Effects;
using MineSharp.Data.Entities;
using MineSharp.Protocol.Packets.Clientbound.Play;
using System.Collections.Concurrent;
using static MineSharp.Bot.MinecraftBot;

namespace MineSharp.Bot.Modules {
    public class EntityModule : Module {
        public EntityModule(MinecraftBot bot) : base(bot) { }

        public event BotEntityEvent EntitySpawned;

        public event BotEntityEvent EntityDespawned;

        public event BotEntityEvent EntityMoved;

        public event BotEntityEvent EntityEffectChanged;


        public ConcurrentDictionary<int, Entity> Entities = new ConcurrentDictionary<int, Entity>();

        protected async override Task Load() {

            this.Bot.On<SpawnLivingEntityPacket>(handleSpawnLivingEntity);
            this.Bot.On<DestroyEntitiesPacket>(handleDestroyEntities);
            this.Bot.On<EntityVelocityPacket>(handleUpdateEntityVelocity);
            this.Bot.On<EntityPositionPacket>(handleEntityPosition);
            this.Bot.On<EntityPositionAndRotationPacket>(handleEntityPositionAndRotation);
            this.Bot.On<EntityRotationPacket>(handleEntityRotation);
            this.Bot.On<EntityTeleportPacket>(handleEntityTeleport);
            this.Bot.On<EntityEffectPacket>(handleEntityEffect);
            this.Bot.On<RemoveEntityEffectPacket>(handleRemoveEntityEffect);

            await this.SetEnabled(true);
        }

        private Task handleSpawnLivingEntity(SpawnLivingEntityPacket packet) {

            var newEntity = EntityFactory.CreateEntity(
                packet.Type, packet.EntityId, new Vector3(packet.X, packet.Y, packet.Z),
                packet.Pitch,
                packet.Yaw,
                new Vector3(packet.VelocityX / MinecraftConst.VelocityToBlock, packet.VelocityY / MinecraftConst.VelocityToBlock, packet.VelocityZ / MinecraftConst.VelocityToBlock),
                true,
                new Dictionary<int, Effect?>());

            Entities.TryAdd(packet.EntityId, newEntity);
            return Task.CompletedTask;
        }

        private Task handleDestroyEntities(DestroyEntitiesPacket packet) {
            for (int i = 0; i < packet.EntityIds.Length; i++) {
                if (!this.Entities.TryRemove(packet.EntityIds[i], out var entity)) 
                    continue; 
                this.EntityDespawned?.Invoke(this.Bot, entity);
            }
            return Task.CompletedTask;  
        }

        private Task handleUpdateEntityVelocity(EntityVelocityPacket packet) {
            if (!Entities.TryGetValue(packet.EntityID, out var entity))
                return Task.CompletedTask;

            entity.Velocity = new Vector3(packet.VelocityX / MinecraftConst.VelocityToBlock, packet.VelocityY / MinecraftConst.VelocityToBlock, packet.VelocityZ / MinecraftConst.VelocityToBlock);
            return Task.CompletedTask;
        }

        private Task handleEntityPosition(EntityPositionPacket packet) {

            if (!Entities.TryGetValue(packet.EntityID, out var entity)) 
                return Task.CompletedTask;

            entity.Position.Add(new Vector3(packet.DeltaX / (128 * 32d), packet.DeltaY / (128 * 32d), packet.DeltaZ / (128 * 32d)));
            entity.IsOnGround = packet.OnGround;

            EntityMoved?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityPositionAndRotation(EntityPositionAndRotationPacket packet) {

            if (!Entities.TryGetValue(packet.EntityID, out var entity)) 
                return Task.CompletedTask;

            entity.Position.Add(new Vector3(packet.DeltaX / (128 * 32d), packet.DeltaY / (128 * 32d), packet.DeltaZ / (128 * 32d)));
            entity.Yaw = packet.Yaw;
            entity.Pitch = packet.Pitch;
            entity.IsOnGround = packet.OnGround;

            EntityMoved?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityRotation(EntityRotationPacket packet) {

            if (!Entities.TryGetValue(packet.EntityID, out var entity)) 
                return Task.CompletedTask;

            entity.Yaw = packet.Yaw;
            entity.Pitch = packet.Pitch;
            entity.IsOnGround = packet.OnGround;

            EntityMoved?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleEntityTeleport(EntityTeleportPacket packet) {
            if (!Entities.TryGetValue(packet.EntityID, out var entity)) 
                return Task.CompletedTask;

            entity.Position.X = packet.X;
            entity.Position.Y = packet.Y;
            entity.Position.Z = packet.Z;

            entity.Yaw = packet.Yaw;
            entity.Pitch = packet.Pitch;

            return Task.CompletedTask;
        }

        private Task handleEntityEffect(EntityEffectPacket packet) {
            if (!this.Entities.TryGetValue(packet.EntityID, out var entity)) 
                return Task.CompletedTask;
            var effectType = EffectPalette.GetEffectTypeById(packet.EffectID);
            var effect = EffectFactory.CreateEffect(packet.EffectID, packet.Amplifier, DateTime.Now, packet.Duration);

            //TODO: Effect updating is so noch nich ganz richtig
            if (entity.Effects.ContainsKey(packet.EffectID) && effect.Amplifier >= entity.Effects[packet.EffectID]!.Amplifier) {
                entity.Effects[packet.EffectID] = effect;
            } else {
                entity.Effects.Add(packet.EffectID, effect);
            }

            EntityEffectChanged?.Invoke(this.Bot, entity);
            return Task.CompletedTask;
        }

        private Task handleRemoveEntityEffect(RemoveEntityEffectPacket packet) {
            if (!this.Entities.TryGetValue(packet.EntityID, out var entity)) 
                return Task.CompletedTask;

            if (entity.Effects.ContainsKey(packet.EffectID)) {
                entity.Effects.Remove(packet.EffectID);
            }
            return Task.CompletedTask;
        }


        internal void AddEntity(Entity entity) {
            this.Entities.TryAdd(entity.Id, entity);
        }
    }
}
