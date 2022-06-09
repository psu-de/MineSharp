
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;
using MineSharp.Data.Items;
using MineSharp.Protocol.Packets.Serverbound.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    public partial class MinecraftBot {


        //TODO: Events
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


        public Player? BotEntity => this.BaseModule?.BotEntity;

        public float Health => BaseModule.Health;
        public bool IsAlive => BaseModule.IsAlive;
        public float Food => BaseModule.Food;
        public float Saturation => BaseModule.Saturation;
        public Identifier CurrentDimension => BaseModule.CurrentDimension;
        public GameMode GameMode => BaseModule.GameMode;

        public byte SelectedHotbarIndex { get; private set; } = 0;
        public Item? HeldItem => throw new NotImplementedException(); //this.Inventory == null ? null : this.Inventory.GetHotbarSlot(this.SelectedHotbarIndex);

        #region Packet Handling

        private void handleHeldItemChange(Protocol.Packets.Clientbound.Play.HeldItemChangePacket packet) {
            this.SelectedHotbarIndex = packet.Slot;

            if (this.HeldItem != null) {
                this.HeldItemChanged?.Invoke(this.HeldItem);
            }
        }


        public Task WaitForBot() => this.BaseModule.WaitForBot();

        #endregion
    }
}
