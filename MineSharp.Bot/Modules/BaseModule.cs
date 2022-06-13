using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MineSharp.Bot.MinecraftBot;

namespace MineSharp.Bot.Modules {
    public class BaseModule : Module {


        /// <summary>
        /// This event fires whenever the Bots Health changed (<see cref="Protocol.Packets.Clientbound.Play.UpdateHealthPacket"/> is received). Food and Saturation were also updated when this event fires.
        /// </summary>
        public event BotEmptyEvent HealthChanged;

        /// <summary>
        /// This event fires whenever the Bot respawned / changed dimension (<see cref="Protocol.Packets.Clientbound.Play.RespawnPacket"/> is received).
        /// </summary>
        public event BotEmptyEvent Respawned;

        /// <summary>
        /// This event fires whenever the Bot dies (<see cref="Protocol.Packets.Clientbound.Play.DeathCombatEventPacket"/> is received).
        /// </summary>
        public event BotChatEvent Died;


        public Player BotEntity { get; private set; }
        public float Health { get; private set; }
        public bool IsAlive => Health > 0;
        public float Food { get; private set; }
        public float Saturation { get; private set; }
        public Identifier CurrentDimension { get; private set; }
        public GameMode GameMode => BotEntity.GameMode;

        private TaskCompletionSource BotInitializedTsc = new TaskCompletionSource();

        public BaseModule(MinecraftBot bot) : base(bot) { }

        protected override async Task Load() {
            await this.SetEnabled(true);

            var task1 = this.Bot.WaitForPacket<Protocol.Packets.Clientbound.Play.JoinGamePacket>();
            var task2 = this.Bot.WaitForPacket<Protocol.Packets.Clientbound.Play.PlayerPositionAndLookPacket>();

            await Task.WhenAll(task1, task2);
            var packet1 = await task1;
            var packet2 = await task2;

            this.BotEntity = new Player(Bot.Options.UsernameOrEmail, 
                Bot.Session.UUID, 
                0, 
                packet1.Gamemode, 
                packet1.EntityID, 
                new Vector3(packet2.X, packet2.Y, packet2.Z), 
                packet2.Pitch, 
                packet2.Yaw);

            this.Health = 20.0f;
            this.Saturation = 20.0f;
            this.Food = 20.0f;
            this.CurrentDimension = packet1.DimensionName;

            Logger.Info($"Initialized Player entity: Location=({packet2.X} / {packet2.Y} / {packet2.Z})");

            BotInitializedTsc.SetResult();

            //TODO: Add to entities and players mapping

            this.Bot.On<UpdateHealthPacket>(handleUpdateHealth);
            this.Bot.On<DeathCombatEventPacket>(handleDeathCombat);
            this.Bot.On<RespawnPacket>(handleRespawnPacket);

            await this.Bot.Client.SendPacket(new Protocol.Packets.Serverbound.Play.PlayerPositionAndRotationPacket(
                packet2.X, packet2.Y, packet2.Z, packet2.Yaw, packet2.Pitch, this.BotEntity.IsOnGround));

            await this.SetEnabled(true);
        }


        private Task handleUpdateHealth(UpdateHealthPacket packet) {
            this.Health = packet.Health;
            this.Food = packet.Food;
            this.Saturation = packet.FoodSaturation;
            this.HealthChanged?.Invoke(Bot);
            return Task.CompletedTask;
        }

        private Task handleDeathCombat(DeathCombatEventPacket packet) {
            this.Health = 0;
            this.Died?.Invoke(Bot, packet.Message);
            return Task.CompletedTask;
        }

        private Task handleRespawnPacket(RespawnPacket packet) {
            this.CurrentDimension = packet.DimensionName;
            this.Respawned?.Invoke(Bot);
            return Task.CompletedTask;
        }



        /// <summary>
        /// Respawns the bot
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when bot is still alive</exception>
        public async Task Respawn () {
            if (this.IsAlive) throw new InvalidOperationException("Cannot respawn the bot when its still living");

            await this.Bot.Client.SendPacket(
                new ClientStatusPacket(ClientStatusPacket.ClientStatusAction.PerformRespawn))
                .ContinueWith((x) => this.Health = 20.0f);
        }

        /// <summary>
        /// Returns a Task that finishes once <see cref="BotEntity"/> has been initialized.
        /// </summary>
        public Task WaitForBot() {
            if (BotEntity != null) return Task.CompletedTask;
            return BotInitializedTsc.Task;
        }
    }
}
