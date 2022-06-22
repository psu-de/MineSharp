using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Windows;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Windows;
using static MineSharp.Bot.MinecraftBot;

namespace MineSharp.Bot.Modules {
    public class WindowsModule : Module {

        public event BotWindowEvent WindowOpened;
        public event BotItemEvent HeldItemChanged;

        private Window MainInventory { get; set; }

        public Window? Inventory { get; private set; }
        public Dictionary<int, Window> OpenedWindows = new Dictionary<int, Window>();


        public byte SelectedHotbarIndex { get; private set; }
        public Item? HeldItem => this.Inventory!.GetSlot((int)PlayerWindowSlots.HotbarStart + SelectedHotbarIndex).Item;

        private TaskCompletionSource inventoryLoadedTsc;

        public WindowsModule(MinecraftBot bot) : base(bot) {
            inventoryLoadedTsc = new TaskCompletionSource();
            this.MainInventory = new Window(new WindowInfo((Identifier)"", "", 4 * 9, true));
            this.MainInventory.WindowSlotUpdated += MainInventory_SlotUpdated;
        }

        protected override async Task Load() {

            this.Bot.On<WindowItemsPacket>(this.handleWindowItems);
            this.Bot.On<SetSlotPacket>(this.handleSetSlot);
            this.Bot.On<HeldItemChangePacket>(this.handleHeldItemChange);

            this.Inventory = OpenWindow(0, new WindowInfo((Identifier)"Inventory", "Inventory", 9, hasOffHandSlot: true));

            await this.SetEnabled(true);
        }

        public Task WaitForInventory() => inventoryLoadedTsc.Task;


        private List<int> AllowedBlocksToOpen = new List<int>() {
            (int)BlockType.Chest,
            (int)BlockType.TrappedChest,
            (int)BlockType.EnderChest,
            (int)BlockType.CraftingTable,
            (int)BlockType.Furnace,
            (int)BlockType.BlastFurnace,
            (int)BlockType.Smoker,
            (int)BlockType.Dispenser,
            (int)BlockType.EnchantingTable,
            (int)BlockType.BrewingStand,
            (int)BlockType.Beacon,
            (int)BlockType.Anvil,
            (int)BlockType.Hopper,

            (int)BlockType.ShulkerBox,
            (int)BlockType.BlackShulkerBox,
            (int)BlockType.BlueShulkerBox,
            (int)BlockType.BrownShulkerBox,
            (int)BlockType.CyanShulkerBox,
            (int)BlockType.GrayShulkerBox,
            (int)BlockType.GreenShulkerBox,
            (int)BlockType.LightBlueShulkerBox,
            (int)BlockType.LightGrayShulkerBox,
            (int)BlockType.LimeShulkerBox,
            (int)BlockType.MagentaShulkerBox,
            (int)BlockType.OrangeShulkerBox,
            (int)BlockType.PinkShulkerBox,
            (int)BlockType.PurpleShulkerBox,
            (int)BlockType.RedShulkerBox,
            (int)BlockType.WhiteShulkerBox,
            (int)BlockType.YellowShulkerBox,

            (int)BlockType.CartographyTable,
            (int)BlockType.Grindstone,
            (int)BlockType.Lectern,
            (int)BlockType.Loom,
            (int)BlockType.Stonecutter,
        };
        public async Task<Window> OpenContainer(Block block) {
            
            if (!AllowedBlocksToOpen.Contains(block.Id)) {
                throw new ArgumentException("Cannot open block of type " + block.Name);
            }

            var packet = new Protocol.Packets.Serverbound.Play.PlayerBlockPlacementPacket(
                Core.Types.Enums.PlayerHand.MainHand, block.Position, Core.Types.Enums.BlockFace.Top, 0.5f, 0.5f, 0.5f, false); // TODO: Hardcoded values
            var send = Bot.Client.SendPacket(packet);

            var receive = Bot.WaitForPacket<OpenWindowPacket>();

            await Task.WhenAll(send, receive);

            var result = await receive;

            var windowInfo = WindowData.Windows[result.WindowType];
            var window = OpenWindow(result.WindowID, windowInfo);

            return window;
        }

        public Task CloseWindow(int id) {
            if (!this.OpenedWindows.TryGetValue(id, out var window)) {
                Logger.Warning("Tried to close window which is not open!");
                return Task.CompletedTask;
            }

            window.WindowClicked -= Window_Clicked;

            window.Close();
            return this.Bot.Client.SendPacket(new CloseWindowPacket((byte)id));
        }

        public async Task SelectHotbarIndex(byte hotbarIndex) {

            if (hotbarIndex < 0 || hotbarIndex > 8) throw new ArgumentOutOfRangeException(nameof(hotbarIndex) + " must be between 0 and 8");

            var packet = new Protocol.Packets.Serverbound.Play.HeldItemChangePacket(hotbarIndex);
            await Bot.Client.SendPacket(packet);

            this.SelectedHotbarIndex = hotbarIndex;
            HeldItemChanged?.Invoke(this.Bot, this.HeldItem);
        }

        #region Handlers

        private Task handleSetSlot(SetSlotPacket packet) {
            if (!OpenedWindows.TryGetValue(packet.WindowID, out var window)) {
                Logger.Warning($"Received {packet.GetType().Name} for windowId={packet.WindowID}, but its not opened");
                return Task.CompletedTask;
            }

            if (packet.SlotData == null) {
                Logger.Warning($"SetSlotPacket received with empty slot data");
                return Task.CompletedTask;
            }

            var slot = (Slot)packet.SlotData;
            slot.SlotNumber = packet.Slot;

            if (slot.SlotNumber == -1) { // used to set selecteditem
                window.SelectedSlot = slot;
            } else {
                window.SetSlot(slot);
            }

            window.StateId = packet.StateID;

            return Task.CompletedTask;
        }


        private DateTime? cacheTimestamp = null;
        private WindowItemsPacket? cachedWindowItemsPacket = null;
        private Task handleWindowItems(WindowItemsPacket packet) {

            if (!OpenedWindows.TryGetValue(packet.WindowID, out var window)) {
                Logger.Warning($"Received {packet.GetType().Name} for windowId={packet.WindowID}, but its not opened");
                // Cache items in case it gets opened in a bit
                cachedWindowItemsPacket = packet;
                cacheTimestamp = DateTime.Now;

                return Task.CompletedTask;
            }

            window.UpdateSlots(packet.SlotData);
            window.StateId = packet.StateID;

            if (window.Id == 0 && !inventoryLoadedTsc.Task.IsCompleted) {
                inventoryLoadedTsc.SetResult();
            }

            return Task.CompletedTask;
        }

        private Task handleHeldItemChange(HeldItemChangePacket packet) {
            this.SelectedHotbarIndex = packet.Slot;
            this.HeldItemChanged?.Invoke(this.Bot, this.HeldItem);
            return Task.CompletedTask;
        }

        #endregion

        private Window OpenWindow(int id, WindowInfo windowInfo) {
            Logger.Debug("Opening window with id=" + id);

            if (OpenedWindows.TryGetValue(id, out var existingWindow)) {
                throw new ArgumentException("Window with id " + id + " already opened");
            }

            var window = new Window(id, windowInfo);
            if (!windowInfo.ExcludeInventory) {
                window.ExtendPlayerInventory(this.MainInventory);
            }
            window.WindowClicked += Window_Clicked;

            OpenedWindows.Add(id, window);
            WindowOpened?.Invoke(this.Bot, window);

            if (cachedWindowItemsPacket != null) {
                if (cachedWindowItemsPacket.WindowID == id && DateTime.Now - cacheTimestamp! <= TimeSpan.FromSeconds(5)) {
                    // use cache
                    Logger.Debug("Applying cache for window with id=" + id);
                    handleWindowItems(cachedWindowItemsPacket!);
                }

                // delete cache
                cachedWindowItemsPacket = null;
                cacheTimestamp = null;
            }


            return window;
        }

        private void Window_Clicked(Window window, WindowClick click) {
            var windowClickPacket = new Protocol.Packets.Serverbound.Play.ClickWindowPacket(
                (byte)window.Id, window.StateId, click.Slot, click.Button, click.ClickMode, window.GetAllSlots(), window.SelectedSlot);
            this.Bot.Client.SendPacket(windowClickPacket);
        }

        private void MainInventory_SlotUpdated(Window window, int index) {
            if (index == 3 * 9 + 1 + SelectedHotbarIndex) {
                this.HeldItemChanged?.Invoke(this.Bot, window.GetSlot(index).Item);
            }
        }
    }
}
