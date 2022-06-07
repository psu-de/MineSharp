using MineSharp.Core.Types;
using MineSharp.Data.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    public partial class MinecraftBot {

        public InventoryWindow Inventory;
        public Dictionary<int, Window> OpenWindows;

        private partial void LoadWindows() {
            this.Inventory = new InventoryWindow();
            this.OpenWindows = new Dictionary<int, Window>() { { 0, Inventory } };
            this.Inventory.Clicked += Window_Clicked;

        }

        #region Packet Handling

        private void handleWindowItems(Protocol.Packets.Clientbound.Play.WindowItemsPacket packet) {

            OpenWindows[packet.WindowID].UpdateSlots(packet.SlotData);
            if (packet.WindowID == 0) {
                HeldItemChanged?.Invoke(this.HeldItem);
            }
        }

        private void handleSetSlot(Protocol.Packets.Clientbound.Play.SetSlotPacket packet) {

            OpenWindows[packet.WindowID].SetSlot(packet.Slot, packet.SlotData);

            if (packet.WindowID == 0) {
                if (packet.Slot - (int)Data.Windows.InventoryWindow.InvSlots.HotBarStart == this.SelectedHotbarIndex) {
                    HeldItemChanged?.Invoke(this.HeldItem);
                }
            }

        }

        #endregion

        private void Window_Clicked(Window sender, short slotIndex, Core.Types.Enums.WindowOperationMode mode, byte button) {
            var packet = new MineSharp.Protocol.Packets.Serverbound.Play.ClickWindowPacket(0, Inventory.StateId, slotIndex, button, mode, Inventory.GetSlotData(), Inventory.SelectedItem?.ToSlot() ?? Slot.Empty);
            this.Client.SendPacket(packet);
        }

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="Window"/> with the id <paramref name="windowID"/>
        /// </summary>
        /// <param name="windowID"></param>
        /// <returns></returns>
        public Window? GetWindow(int windowID) {
            Window? window = null;
            OpenWindows.TryGetValue(windowID, out window);
            return window;
        }

        #endregion
    }
}
