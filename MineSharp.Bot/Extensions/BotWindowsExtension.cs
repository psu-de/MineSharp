using MineSharp.Data.Blocks;
using MineSharp.Data.Items;
using MineSharp.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    public partial class MinecraftBot {

        /// <summary>
        /// The Bots Main Inventory. You can use <see cref="MineSharp.Data.Windows.PlayerWindowSlots"/> for indexing the slots
        /// </summary>
        public Window? Inventory => WindowsModule.Inventory;

        /// <summary>
        /// All currently opened windows. Mapped from the WindowId to the corresponding <see cref="Window"/>
        /// </summary>
        public Dictionary<int, Window> OpenedWindows => WindowsModule.OpenedWindows;

        /// <summary>
        /// The Item the bot is currently holding in the main hand
        /// </summary>
        public Item? HeldItem => WindowsModule.HeldItem;

        /// <summary>
        /// The hotbar index currently selected
        /// </summary>
        public byte SelectedHotbarIndex => WindowsModule.SelectedHotbarIndex;

        /// <summary>
        /// Returns a task which completes when the inventory items have been received
        /// </summary>
        /// <returns></returns>
        public Task WaitForInventory() => WindowsModule.WaitForInventory();

        /// <summary>
        /// Opens a block (chest, furnace, crafting table, ...) and returns the opened window
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public Task<Window> OpenContainer(Block block) => WindowsModule.OpenContainer(block);


        /// <summary>
        /// Closes a window
        /// </summary>
        /// <param name="windowId"></param>
        /// <returns></returns>
        public Task CloseWindow(int windowId) => WindowsModule.CloseWindow(windowId);

        /// <summary>
        /// Closes a window
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public Task CloseWindow(Window window) => WindowsModule.CloseWindow(window.Id);

        /// <summary>
        /// Changes the selected hotbar slot (see <see cref="HeldItem"/>)
        /// </summary>
        /// <param name="hotbarIndex">Index in the hotbar (0-8)</param>
        /// <returns>A task that will be completed when the operation is finshed</returns>
        public Task SelectHotbarIndex(byte hotbarIndex) => WindowsModule.SelectHotbarIndex(hotbarIndex);
    }
}
