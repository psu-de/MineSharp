
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;

namespace MineSharp.Bot {
    public partial class MinecraftBot {

        /// <summary>
        /// This event fires whenever the Bots Health changed (<see cref="Protocol.Packets.Clientbound.Play.UpdateHealthPacket"/> is received). Food and Saturation were also updated when this event fires.
        /// </summary>
        public event BotEmptyEvent HealthChanged {
            add { BaseModule.HealthChanged += value; }
            remove { BaseModule.HealthChanged -= value; }
        }

        /// <summary>
        /// This event fires whenever the Bot respawned / changed dimension (<see cref="Protocol.Packets.Clientbound.Play.RespawnPacket"/> is received).
        /// </summary>
        public event BotEmptyEvent Respawned {
            add { BaseModule.Respawned += value; }
            remove { BaseModule.HealthChanged -= value; }
        }

        /// <summary>
        /// This event fires whenever the Bot dies (<see cref="Protocol.Packets.Clientbound.Play.DeathCombatEventPacket"/> is received).
        /// </summary>
        public event BotChatEvent Died {
            add { BaseModule.Died += value; }
            remove { BaseModule.Died -= value; }
        }


        public Player? BotEntity => this.BaseModule?.BotEntity;
        public MinecraftPlayer? Player => this.BaseModule?.Player;

        public float Health => BaseModule.Health;
        public bool IsAlive => BaseModule.IsAlive;
        public float Food => BaseModule.Food;
        public float Saturation => BaseModule.Saturation;
        public Identifier CurrentDimension => BaseModule.CurrentDimension;
        public GameMode GameMode => BaseModule.GameMode;

        [BotFunction("Basic", "Waits until the bot entity has loaded. BotEntity has been set at this point.")]
        public Task WaitForBot() => this.BaseModule.WaitForBot();
    }
}
