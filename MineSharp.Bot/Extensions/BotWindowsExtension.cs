using MineSharp.Core.Types;
using MineSharp.Windows;

namespace MineSharp.Bot {
    public partial class MinecraftBot {

        /// <summary>
        /// Fires when a new window has been opened
        /// </summary>
        public event BotWindowEvent WindowOpened {
            add { WindowsModule!.WindowOpened += value; }
            remove { WindowsModule!.WindowOpened -= value; }
        }

        /// <summary>
        /// Fires when the held item changed
        /// </summary>
        public event BotItemEvent HeldItemChanged {
            add { WindowsModule!.HeldItemChanged += value; }
            remove { WindowsModule!.HeldItemChanged -= value; }
        }

        /// <summary>
        /// The Bots Main Inventory. You can use <see cref="MineSharp.Data.Windows.PlayerWindowSlots"/> for indexing the slots
        /// </summary>
        public Window? Inventory => WindowsModule!.Inventory;

        /// <summary>
        /// All currently opened windows. Mapped from the WindowId to the corresponding <see cref="Window"/>
        /// </summary>
        public Dictionary<int, Window> OpenedWindows => WindowsModule!.OpenedWindows;

        /// <summary>
        /// The Item the bot is currently holding in the main hand
        /// </summary>
        public Item? HeldItem => WindowsModule!.HeldItem;

        /// <summary>
        /// The hotbar index currently selected
        /// </summary>
        public byte SelectedHotbarIndex => WindowsModule!.SelectedHotbarIndex;

        /// <summary>
        /// Returns a task which completes when the inventory items have been received
        /// </summary>
        /// <returns></returns>
        [BotFunction("Window", "Waits until the bots inventory has been loaded.")]
        public Task WaitForInventory() => WindowsModule!.WaitForInventory();

        /// <summary>
        /// Opens a block (chest, furnace, crafting table, ...) and returns the opened window
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [BotFunction("Window", "Opens a window from the block")]
        public Task<Window> OpenContainer(Block block) => WindowsModule!.OpenContainer(block);


        /// <summary>
        /// Closes a window
        /// </summary>
        /// <param name="windowId"></param>
        /// <returns></returns>
        [BotFunction("Window", "Closes a window")]
        public Task CloseWindow(int windowId) => WindowsModule!.CloseWindow(windowId);

        /// <summary>
        /// Closes a window
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [BotFunction("Window", "Closes a window")]
        public Task CloseWindow(Window window) => WindowsModule!.CloseWindow(window.Id);

        /// <summary>
        /// Changes the selected hotbar slot (see <see cref="HeldItem"/>)
        /// </summary>
        /// <param name="hotbarIndex">Index in the hotbar (0-8)</param>
        /// <returns>A task that will be completed when the operation is finshed</returns>
        [BotFunction("Window", "Selects a slot on the hotbar")]
        public Task SelectHotbarIndex(byte hotbarIndex) => WindowsModule!.SelectHotbarIndex(hotbarIndex);
    }
}
