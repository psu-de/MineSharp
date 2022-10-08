using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;
using MineSharp.Data.Protocol.Play.Clientbound;
using MineSharp.Data.Protocol.Play.Serverbound;
using static MineSharp.Bot.MinecraftBot;
using PacketChat = MineSharp.Data.Protocol.Play.Clientbound.PacketChat;
using PacketPosition = MineSharp.Data.Protocol.Play.Clientbound.PacketPosition;

namespace MineSharp.Bot.Modules
{
    public class BaseModule : Module
    {

        private readonly TaskCompletionSource BotInitializedTsc = new TaskCompletionSource();

        public BaseModule(MinecraftBot bot) : base(bot) {}


        public Player? BotEntity { get; private set; }
        public MinecraftPlayer? Player { get; private set; }
        public float Health { get; private set; }
        public bool IsAlive => this.Health > 0;
        public float Food { get; private set; }
        public float Saturation { get; private set; }
        public Identifier? CurrentDimension { get; private set; }
        public GameMode? GameMode => this.Player?.GameMode;


        public event BotEmptyEvent? HealthChanged;

        public event BotEmptyEvent? Respawned;

        public event BotChatEvent? Died;

        public event BotChatSenderEvent? ChatReceived;

        protected override async Task Load()
        {
            await this.SetEnabled(true);

            var task1 = this.Bot.WaitForPacket<PacketLogin>();
            var task2 = this.Bot.WaitForPacket<PacketPosition>();

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

            this.Player = new MinecraftPlayer(this.Bot.Options.UsernameOrEmail, this.Bot.Session.UUID, 0, (GameMode)packet1.GameMode!, this.BotEntity);

            this.Health = 20.0f;
            this.Saturation = 20.0f;
            this.Food = 20.0f;
            this.CurrentDimension = (Identifier)packet1.WorldName!;

            this.Logger.Info($"Initialized Player entity: Location=({packet2.X} / {packet2.Y} / {packet2.Z})");

            this.BotInitializedTsc.SetResult();

            //TODO: Add to entities and players mapping

            this.Bot.On<PacketUpdateHealth>(this.handleUpdateHealth);
            this.Bot.On<PacketDeathCombatEvent>(this.handleDeathCombat);
            this.Bot.On<PacketRespawn>(this.handleRespawnPacket);
            this.Bot.On<PacketChat>(this.handleChatPacket);

            await this.Bot.Client.SendPacket(new PacketPositionLook(
                packet2.X!, packet2.Y!, packet2.Z!, packet2.Yaw!, packet2.Pitch!, this.BotEntity.IsOnGround));

            await this.SetEnabled(true);
        }


        private Task handleUpdateHealth(PacketUpdateHealth packet)
        {
            this.Health = packet.Health!;
            this.Food = packet.Food!;
            this.Saturation = packet.FoodSaturation!;
            this.HealthChanged?.Invoke(this.Bot);
            return Task.CompletedTask;
        }

        private Task handleDeathCombat(PacketDeathCombatEvent packet)
        {
            this.Health = 0;
            this.Died?.Invoke(this.Bot, new Chat(packet.Message!));
            return Task.CompletedTask;
        }

        private Task handleRespawnPacket(PacketRespawn packet)
        {
            this.CurrentDimension = (Identifier)packet.WorldName!;
            this.Respawned?.Invoke(this.Bot);
            return Task.CompletedTask;
        }

        private Task handleChatPacket(PacketChat packet)
        {
            // TODO: handle other chat positions
            if (packet.Position == 0)
            {
                if (!this.Bot.PlayerMapping.TryGetValue(packet.Sender, out var player))
                {
                    this.Logger.Warning($"Unknown player uuid: {packet.Sender}");
                } else
                {
                    this.ChatReceived?.Invoke(this.Bot, new Chat(packet.Message), player);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Respawns the bot
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when bot is still alive</exception>
        public async Task Respawn()
        {
            if (this.IsAlive) throw new InvalidOperationException("Cannot respawn the bot when its still living");

            await this.Bot.Client.SendPacket(
                    new PacketClientCommand(0))
                .ContinueWith(x => this.Health = 20.0f);
        }

        /// <summary>
        ///     Returns a Task that finishes once <see cref="BotEntity" /> has been initialized.
        /// </summary>
        public Task WaitForBot()
        {
            if (this.BotEntity != null) return Task.CompletedTask;
            return this.BotInitializedTsc.Task;
        }

        /// <summary>
        ///     Attacks a given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Task Attack(Entity entity)
        {
            // TODO: Cooldown
            this.Bot.AssertPlayerLoaded();

            if (entity.Position.DistanceSquared(this.BotEntity!.Position) > 36) throw new InvalidOperationException("Too far");

            var packet = new PacketUseEntity(
                entity.ServerId,
                1,
                new PacketUseEntity.XSwitch(null), new PacketUseEntity.YSwitch(null), new PacketUseEntity.ZSwitch(null),
                new PacketUseEntity.HandSwitch(0), this.Bot.PlayerControls.IsSneaking); // Maybe invert sneak?
            return this.Bot.Client.SendPacket(packet);
        }

        /// <summary>
        ///     Sends a public chat message to the server
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Chat(string message)
        {
            var packet = new Data.Protocol.Play.Serverbound.PacketChat(message);
            return this.Bot.Client.SendPacket(packet);
        }
    }
}
