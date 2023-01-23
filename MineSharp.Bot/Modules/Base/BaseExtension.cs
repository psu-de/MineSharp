using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;

namespace MineSharp.Bot
{
    public partial class MinecraftBot
    {


        public Entity? BotEntity => this.BaseModule?.BotEntity;
        public MinecraftPlayer? Player => this.BaseModule?.Player;

        public float Health => this.BaseModule!.Health;
        public bool IsAlive => this.BaseModule!.IsAlive;
        public float Food => this.BaseModule!.Food;
        public float Saturation => this.BaseModule!.Saturation;
        public Identifier? CurrentDimension => this.BaseModule!.CurrentDimension;
        public GameMode? GameMode => this.BaseModule!.GameMode;

        /// <summary>
        ///     This event fires whenever the Bots Health changed (
        ///     <see cref="Protocol.Packets.Clientbound.Play.UpdateHealthPacket" /> is received). Food and Saturation were also
        ///     updated when this event fires.
        /// </summary>
        public event BotEmptyEvent HealthChanged {
            add => this.BaseModule!.HealthChanged += value;
            remove => this.BaseModule!.HealthChanged -= value;
        }

        /// <summary>
        ///     This event fires whenever the Bot respawned / changed dimension (
        ///     <see cref="Protocol.Packets.Clientbound.Play.RespawnPacket" /> is received).
        /// </summary>
        public event BotEmptyEvent Respawned {
            add => this.BaseModule!.Respawned += value;
            remove => this.BaseModule!.HealthChanged -= value;
        }

        /// <summary>
        ///     This event fires whenever the Bot dies (<see cref="Protocol.Packets.Clientbound.Play.DeathCombatEventPacket" /> is
        ///     received).
        /// </summary>
        public event BotChatEvent Died {
            add => this.BaseModule!.Died += value;
            remove => this.BaseModule!.Died -= value;
        }

        /// <summary>
        ///     Fires when a chat message from another player is received
        /// </summary>
        public event BotChatMessageEvent ChatMessageReceived {
            add => this.BaseModule!.ChatMessageReceived += value;
            remove => this.BaseModule!.ChatMessageReceived -= value;
        }

        public void AssertPlayerLoaded()
        {
            if (this.BotEntity == null)
                throw new Exception("BotEntity is null. Use await Bot.WaitForBot()");
        }


        [BotFunction("Basic", "Waits until the bot entity has loaded. BotEntity has been set at this point.")]
        public Task WaitForBot() => this.BaseModule!.WaitForBot();


        [BotFunction("Basic", "Respawns the bot. Only possible when the bot is dead.")]
        public Task Respawn() => this.BaseModule!.Respawn();

        [BotFunction("Basic", "Attacks a given entity")]
        public Task Attack(Entity entity) => this.BaseModule!.Attack(entity);

        [BotFunction("Basic", "Sends a chat message to the server")]
        public Task Chat(string message) => this.BaseModule!.Chat(message);
    }
}
