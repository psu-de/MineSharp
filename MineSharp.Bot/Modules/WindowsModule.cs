using MineSharp.Bot.Helpers;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Windows;
using MineSharp.Data.Protocol.Play.Clientbound;
using MineSharp.Windows;
using static MineSharp.Bot.MinecraftBot;
using MineSharp.Data;

namespace MineSharp.Bot.Modules {
    public class WindowsModule : Module {

        public event BotWindowEvent? WindowOpened;
        public event BotItemEvent? HeldItemChanged;

        private BotWindow MainInventory { get; set; }

        public BotWindow? Inventory { get; private set; }
        public Dictionary<int, BotWindow> OpenedWindows = new Dictionary<int, BotWindow>();


        public byte SelectedHotbarIndex { get; private set; }
        public Item? HeldItem => this.Inventory!.GetSlot((int)PlayerWindowSlots.HotbarStart + SelectedHotbarIndex).Item;

        private TaskCompletionSource inventoryLoadedTsc;

        public WindowsModule(MinecraftBot bot) : base(bot) {
            inventoryLoadedTsc = new TaskCompletionSource();
            this.MainInventory = new BotWindow(Bot, new WindowInfo((Identifier)"", "", 4 * 9, true));
            this.MainInventory.WindowSlotUpdated += MainInventory_SlotUpdated;
        }

        protected override async Task Load() {

            this.Bot.On<PacketWindowItems>(this.handleWindowItems);
            this.Bot.On<PacketSetSlot>(this.handleSetSlot);
            this.Bot.On<PacketHeldItemSlot>(this.HandleHeldItemChange);

            this.Inventory = OpenWindow(0, new WindowInfo((Identifier)"Inventory", "Inventory", 9, hasOffHandSlot: true));

            await this.SetEnabled(true);
        }

        public Task WaitForInventory() => inventoryLoadedTsc.Task;


        private readonly List<int> AllowedBlocksToOpen = new List<int>() {
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

            var packet = new Data.Protocol.Play.Serverbound.PacketBlockPlace(
                (int)Core.Types.Enums.PlayerHand.MainHand, block.Position!.ToProtocolPosition(), (int)Core.Types.Enums.BlockFace.Top, 0.5f, 0.5f, 0.5f, false); // TODO: Hardcoded values
            var send = Bot.Client.SendPacket(packet);

            var receive = Bot.WaitForPacket<PacketOpenWindow>();

            await Task.WhenAll(send, receive);

            var result = await receive;

            var windowInfo = WindowData.Windows[result.InventoryType!];
            var window = OpenWindow(result.WindowId!, windowInfo);

            return window;
        }

        public Task CloseWindow(int id) {
            if (!this.OpenedWindows.TryGetValue(id, out var window)) {
                Logger.Warning("Tried to close window which is not open!");
                return Task.CompletedTask;
            }

            window.Close();
            return this.Bot.Client.SendPacket(new PacketCloseWindow((byte)id));
        }

        public async Task SelectHotbarIndex(byte hotbarIndex) {

            if (hotbarIndex < 0 || hotbarIndex > 8) throw new ArgumentOutOfRangeException(nameof(hotbarIndex) + " must be between 0 and 8");

            var packet = new Data.Protocol.Play.Serverbound.PacketHeldItemSlot(hotbarIndex);
            await Bot.Client.SendPacket(packet);

            this.SelectedHotbarIndex = hotbarIndex;
            HeldItemChanged?.Invoke(this.Bot, this.HeldItem);
        }

        #region Handlers

        private Task handleSetSlot(PacketSetSlot packet) {
            if (!OpenedWindows.TryGetValue(packet.WindowId!, out var window)) {
                Logger.Warning($"Received {packet.GetType().Name} for windowId={packet.WindowId!}, but its not opened");
                return Task.CompletedTask;
            }

            var slot = packet.Item!.ToSlot();
            slot.SlotNumber = packet.Slot!;

            if (slot.SlotNumber == -1) { // used to set selecteditem
                window.SelectedSlot = slot;
            } else {
                window.SetSlot(slot);
            }

            window.StateId = packet.StateId!;

            return Task.CompletedTask;
        }


        private DateTime? _cacheTimestamp = null;
        private PacketWindowItems? _cachedWindowItemsPacket = null;
        private Task handleWindowItems(PacketWindowItems packet) {

            if (!OpenedWindows.TryGetValue(packet.WindowId!, out var window)) {
                Logger.Warning($"Received {packet.GetType().Name} for windowId={packet.WindowId!}, but its not opened");
                // Cache items in case it gets opened in a bit
                _cachedWindowItemsPacket = packet;
                _cacheTimestamp = DateTime.Now;

                return Task.CompletedTask;
            }

            window.UpdateSlots(packet.Items!.Select((x, i) => {
                var slot = x.ToSlot();
                slot.SlotNumber = (short)i;
                return slot;
            }).ToArray());
            window.StateId = packet.StateId!;

            if (window.Id == 0 && !inventoryLoadedTsc.Task.IsCompleted) {
                inventoryLoadedTsc.SetResult();
            }

            return Task.CompletedTask;
        }

        private Task HandleHeldItemChange(PacketHeldItemSlot packet) {
            this.SelectedHotbarIndex = (byte)packet.Slot!;
            this.HeldItemChanged?.Invoke(this.Bot, this.HeldItem);
            return Task.CompletedTask;
        }

        #endregion

        private BotWindow OpenWindow(int id, WindowInfo windowInfo) {
            Logger.Debug("Opening window with id=" + id);

            if (OpenedWindows.TryGetValue(id, out var existingWindow)) {
                throw new ArgumentException("Window with id " + id + " already opened");
            }

            var window = new BotWindow(Bot, id, windowInfo, playerInventory: (windowInfo.ExcludeInventory ? null : this.MainInventory));

            OpenedWindows.Add(id, window);
            WindowOpened?.Invoke(this.Bot, window);

            if (_cachedWindowItemsPacket != null) {
                if (_cachedWindowItemsPacket.WindowId == id && DateTime.Now - _cacheTimestamp! <= TimeSpan.FromSeconds(5)) {
                    // use cache
                    Logger.Debug("Applying cache for window with id=" + id);
                    handleWindowItems(_cachedWindowItemsPacket!);
                }

                // delete cache
                _cachedWindowItemsPacket = null;
                _cacheTimestamp = null;
            }


            return window;
        }

        private void MainInventory_SlotUpdated(Window window, int index) {
            if (index == 3 * 9 + 1 + SelectedHotbarIndex) {
                this.HeldItemChanged?.Invoke(this.Bot, window.GetSlot(index).Item);
            }
        }
    }
}
