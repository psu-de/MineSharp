using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Items;
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
        public Item? HeldItem => this.Inventory!.GetSlot((int)PlayerWindowSlots.HotbarStart + SelectedHotbarIndex).AsItem();

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


        private List<BlockInfo> AllowedBlocksToOpen = new List<BlockInfo>() {
            BlockData.Blocks[(int)BlockType.Chest],
            BlockData.Blocks[(int)BlockType.TrappedChest],
            BlockData.Blocks[(int)BlockType.EnderChest],
            BlockData.Blocks[(int)BlockType.CraftingTable],
            BlockData.Blocks[(int)BlockType.Furnace],
            BlockData.Blocks[(int)BlockType.BlastFurnace],
            BlockData.Blocks[(int)BlockType.Smoker],
            BlockData.Blocks[(int)BlockType.Dispenser],
            BlockData.Blocks[(int)BlockType.EnchantingTable],
            BlockData.Blocks[(int)BlockType.BrewingStand],
            BlockData.Blocks[(int)BlockType.Beacon],
            BlockData.Blocks[(int)BlockType.Anvil],
            BlockData.Blocks[(int)BlockType.Hopper],

            BlockData.Blocks[(int)BlockType.ShulkerBox],
            BlockData.Blocks[(int)BlockType.BlackShulkerBox],
            BlockData.Blocks[(int)BlockType.BlueShulkerBox],
            BlockData.Blocks[(int)BlockType.BrownShulkerBox],
            BlockData.Blocks[(int)BlockType.CyanShulkerBox],
            BlockData.Blocks[(int)BlockType.GrayShulkerBox],
            BlockData.Blocks[(int)BlockType.GreenShulkerBox],
            BlockData.Blocks[(int)BlockType.LightBlueShulkerBox],
            BlockData.Blocks[(int)BlockType.LightGrayShulkerBox],
            BlockData.Blocks[(int)BlockType.LimeShulkerBox],
            BlockData.Blocks[(int)BlockType.MagentaShulkerBox],
            BlockData.Blocks[(int)BlockType.OrangeShulkerBox],
            BlockData.Blocks[(int)BlockType.PinkShulkerBox],
            BlockData.Blocks[(int)BlockType.PurpleShulkerBox],
            BlockData.Blocks[(int)BlockType.RedShulkerBox],
            BlockData.Blocks[(int)BlockType.WhiteShulkerBox],
            BlockData.Blocks[(int)BlockType.YellowShulkerBox],

            BlockData.Blocks[(int)BlockType.CartographyTable],
            BlockData.Blocks[(int)BlockType.Grindstone],
            BlockData.Blocks[(int)BlockType.Lectern],
            BlockData.Blocks[(int)BlockType.Loom],
            BlockData.Blocks[(int)BlockType.Stonecutter],
        };
        public async Task<Window> OpenContainer(Block block) {
            
            if (!AllowedBlocksToOpen.Contains(block.Info)) {
                throw new ArgumentException("Cannot open block of type " + block.Info.Name);
            }

            var packet = new Protocol.Packets.Serverbound.Play.PlayerBlockPlacementPacket(
                Core.Types.Enums.PlayerHand.MainHand, block.Position, Core.Types.Enums.BlockFace.Top, 0.5f, 0.5f, 0.5f, false); // TODO: Hardcoded values
            var send = Bot.Client.SendPacket(packet);

            var receive = Bot.WaitForPacket<OpenWindowPacket>();

            await Task.WhenAll(send, receive); //TODO: OpenWindowPacket gets not received

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
                this.HeldItemChanged?.Invoke(this.Bot, window.GetSlot(index).AsItem());
            }
        }
    }
}
