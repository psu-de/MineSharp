using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;
using MineSharp.Data.Protocol.Play.Clientbound;
using MineSharp.Data.Protocol.Play.Serverbound;
using static MineSharp.Bot.MinecraftBot;

namespace MineSharp.Bot.Modules {
    public class BaseModule : Module {


        public event BotEmptyEvent? HealthChanged;

        public event BotEmptyEvent? Respawned;

        public event BotChatEvent? Died;


        public Player? BotEntity { get; private set; }
        public MinecraftPlayer? Player { get; private set; }
        public float Health { get; private set; }
        public bool IsAlive => Health > 0;
        public float Food { get; private set; }
        public float Saturation { get; private set; }
        public Identifier? CurrentDimension { get; private set; }
        public GameMode? GameMode => Player?.GameMode;

        private TaskCompletionSource BotInitializedTsc = new TaskCompletionSource();

        public BaseModule(MinecraftBot bot) : base(bot) { }

        protected override async Task Load() {
            await this.SetEnabled(true);

            var task1 = this.Bot.WaitForPacket<PacketLogin>();
            var task2 = this.Bot.WaitForPacket<MineSharp.Data.Protocol.Play.Clientbound.PacketPosition>();

            await Task.WhenAll(task1, task2);
            var packet1 = await task1;
            var packet2 = await task2;

            this.BotEntity = new Player(
                packet1.EntityId!,
                new Vector3(packet2.X!, packet2.Y!, packet2.Z!),
                packet2.Pitch!,
                packet2.Yaw!,
                new Vector3(0, 0, 0),
                true,
                new Dictionary<int, Effect?>());

            this.Player = new MinecraftPlayer(Bot.Options.UsernameOrEmail,
                Bot.Session.UUID, 0, (GameMode)packet1.GameMode!, this.BotEntity);

            this.Health = 20.0f;
            this.Saturation = 20.0f;
            this.Food = 20.0f;
            this.CurrentDimension = (Identifier)packet1.WorldName!;

            Logger.Info($"Initialized Player entity: Location=({packet2.X} / {packet2.Y} / {packet2.Z})");

            BotInitializedTsc.SetResult();

            //TODO: Add to entities and players mapping

            this.Bot.On<PacketUpdateHealth>(handleUpdateHealth);
            this.Bot.On<PacketDeathCombatEvent>(handleDeathCombat);
            this.Bot.On<PacketRespawn>(handleRespawnPacket);

            await this.Bot.Client.SendPacket(new Data.Protocol.Play.Serverbound.PacketPositionLook(
                packet2.X!, packet2.Y!, packet2.Z!, packet2.Yaw!, packet2.Pitch!, this.BotEntity.IsOnGround));

            await this.SetEnabled(true);
        }


        private Task handleUpdateHealth(PacketUpdateHealth packet) {
            this.Health = packet.Health!;
            this.Food = packet.Food!;
            this.Saturation = packet.FoodSaturation!;
            this.HealthChanged?.Invoke(Bot);
            return Task.CompletedTask;
        }

        private Task handleDeathCombat(PacketDeathCombatEvent packet) {
            this.Health = 0;
            this.Died?.Invoke(Bot, new Chat(packet.Message!));
            return Task.CompletedTask;
        }

        private Task handleRespawnPacket(PacketRespawn packet) {
            this.CurrentDimension = (Identifier)packet.WorldName!;
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
                new PacketClientCommand(0))
                .ContinueWith((x) => this.Health = 20.0f);
        }

        /// <summary>
        /// Returns a Task that finishes once <see cref="BotEntity"/> has been initialized.
        /// </summary>
        public Task WaitForBot() {
            if (BotEntity != null) return Task.CompletedTask;
            return BotInitializedTsc.Task;
        }

        /// <summary>
        /// Attacks a given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Task Attack(Entity entity) {
            // TODO: Cooldown
            Bot.AssertPlayerLoaded();

            if (entity.Position.DistanceSquared(this.BotEntity!.Position) > 36) throw new InvalidOperationException("Too far");

            var packet = new Data.Protocol.Play.Serverbound.PacketEntityAction(entity.Id, 1, Bot.MovementControls.Sneak ? 0 : 1); // Maybe invert sneak?
            return this.Bot.Client.SendPacket(packet);
        }

        /// <summary>
        /// Sends a public chat message to the server
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Chat(string message) {
            var packet = new Data.Protocol.Play.Serverbound.PacketChat(message);
            return this.Bot.Client.SendPacket(packet);
        }
    }
}
