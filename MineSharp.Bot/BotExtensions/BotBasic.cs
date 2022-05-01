
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Items;
using MineSharp.Protocol.Packets.Serverbound.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    public partial class Bot {

        /// <summary>
        /// This event fires once when the Bot joined a server
        /// </summary>
        public event BotEmptyEvent Joined;

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

        /// <summary>
        /// This event fires when the bot switched the held item (<see cref="Protocol.Packets.Clientbound.Play.HeldItemChangePacket"/> is received).
        /// </summary>
        public event BotItemEvent HeldItemChanged;




        public float Health { get; private set; }
        public bool IsAlive { get; private set; }
        public float Food { get; private set; }
        public float Saturation { get; private set; }
        public Identifier CurrentDimension { get; private set; }
        public GameMode GameMode { get; private set; }

        public byte SelectedHotbarIndex { get; private set; } = 0;
        public Item? HeldItem => this.Inventory == null ? null : this.Inventory.GetHotbarSlot(this.SelectedHotbarIndex);

        #region Packet Handling

        private void handleJoinGame(Protocol.Packets.Clientbound.Play.JoinGamePacket packet) {
            this.GameMode = packet.Gamemode;
            Task.Run(async () => {
                var p = await this.WaitForPacket<Protocol.Packets.Clientbound.Play.PlayerPositionAndLookPacket>();
                InitPlayer(packet, p as Protocol.Packets.Clientbound.Play.PlayerPositionAndLookPacket);
                this.Joined?.Invoke();
            });
        }

        private void handleUpdateHealth(Protocol.Packets.Clientbound.Play.UpdateHealthPacket packet) {
            this.Health = packet.Health;
            this.Food = packet.Food;
            this.Saturation = packet.FoodSaturation;
            this.HealthChanged?.Invoke();
        }

        private void handleDeathCombat(Protocol.Packets.Clientbound.Play.DeathCombatEventPacket packet) {
            this.IsAlive = false;
            this.Died?.Invoke(packet.Message);
        }

        private void handleRespawn(Protocol.Packets.Clientbound.Play.RespawnPacket packet) {
            this.CurrentDimension = packet.DimensionName;
            this.Respawned?.Invoke();
        }

        private void handleHeldItemChange(Protocol.Packets.Clientbound.Play.HeldItemChangePacket packet) {
            this.SelectedHotbarIndex = packet.Slot;

            if (this.HeldItem != null) {
                this.HeldItemChanged?.Invoke(this.HeldItem);
            }
        }

        #endregion
    }
}
