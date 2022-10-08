using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Windows;
using MineSharp.Data.Protocol.Play.Clientbound;
using MineSharp.Windows;
using static MineSharp.Bot.MinecraftBot;
using MineSharp.Data;

namespace MineSharp.Bot.Modules
{
    public class WindowsModule : Module
    {

        public event BotWindowEvent? WindowOpened;
        public event BotItemEvent? HeldItemChanged;

        private Window MainInventory { get; set; }

        public Window? Inventory { get; private set; }
        public Dictionary<int, Window> OpenedWindows = new Dictionary<int, Window>();


        public byte SelectedHotbarIndex { get; private set; }
        public Item? HeldItem => this.Inventory!.GetSlot((int)PlayerWindowSlots.HotbarStart + this.SelectedHotbarIndex).Item;

        private TaskCompletionSource inventoryLoadedTsc;

        public WindowsModule(MinecraftBot bot) : base(bot)
        {
            this.inventoryLoadedTsc = new TaskCompletionSource();
            this.MainInventory = new Window(new WindowInfo((Identifier)"", "", 4 * 9, true));
            this.MainInventory.WindowSlotUpdated += this.MainInventory_SlotUpdated;
        }

        protected override async Task Load()
        {

            this.Bot.On<PacketWindowItems>(this.handleWindowItems);
            this.Bot.On<PacketSetSlot>(this.handleSetSlot);
            this.Bot.On<PacketHeldItemSlot>(this.handleHeldItemChange);

            this.Inventory = this.OpenWindow(0, new WindowInfo((Identifier)"Inventory", "Inventory", 9, hasOffHandSlot: true));

            await this.SetEnabled(true);
        }

        public Task WaitForInventory() => this.inventoryLoadedTsc.Task;


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
            (int)BlockType.Stonecutter
        };
        public async Task<Window> OpenContainer(Block block)
        {

            if (!this.AllowedBlocksToOpen.Contains(block.Id))
            {
                throw new ArgumentException("Cannot open block of type " + block.Name);
            }

            var packet = new Data.Protocol.Play.Serverbound.PacketBlockPlace(
                (int)Core.Types.Enums.PlayerHand.MainHand, block.Position!.ToProtocolPosition(), (int)Core.Types.Enums.BlockFace.Top, 0.5f, 0.5f, 0.5f, false); // TODO: Hardcoded values
            var send = this.Bot.Client.SendPacket(packet);

            var receive = this.Bot.WaitForPacket<PacketOpenWindow>();

            await Task.WhenAll(send, receive);

            var result = await receive;

            var windowInfo = WindowData.Windows[result.InventoryType!];
            var window = this.OpenWindow(result.WindowId!, windowInfo);

            return window;
        }

        public Task CloseWindow(int id)
        {
            if (!this.OpenedWindows.TryGetValue(id, out var window))
            {
                this.Logger.Warning("Tried to close window which is not open!");
                return Task.CompletedTask;
            }

            window.WindowClicked -= this.Window_Clicked;

            window.Close();
            return this.Bot.Client.SendPacket(new PacketCloseWindow((byte)id));
        }

        public async Task SelectHotbarIndex(byte hotbarIndex)
        {

            if (hotbarIndex < 0 || hotbarIndex > 8) throw new ArgumentOutOfRangeException(nameof(hotbarIndex) + " must be between 0 and 8");

            var packet = new Data.Protocol.Play.Serverbound.PacketHeldItemSlot(hotbarIndex);
            await this.Bot.Client.SendPacket(packet);

            this.SelectedHotbarIndex = hotbarIndex;
            this.HeldItemChanged?.Invoke(this.Bot, this.HeldItem);
        }

        #region Handlers


        private Task handleSetSlot(PacketSetSlot packet)
        {
            if (!this.OpenedWindows.TryGetValue(packet.WindowId!, out var window))
            {
                this.Logger.Warning($"Received {packet.GetType().Name} for windowId={packet.WindowId!}, but its not opened");
                return Task.CompletedTask;
            }

            var slot = packet.Item!.ToSlot();
            slot.SlotNumber = packet.Slot!;

            if (slot.SlotNumber == -1)
            { // used to set selecteditem
                window.SelectedSlot = slot;
            } else
            {
                window.SetSlot(slot);
            }

            window.StateId = packet.StateId!;

            return Task.CompletedTask;
        }


        private DateTime? cacheTimestamp = null;
        private PacketWindowItems? cachedWindowItemsPacket = null;
        private Task handleWindowItems(PacketWindowItems packet)
        {

            if (!this.OpenedWindows.TryGetValue(packet.WindowId!, out var window))
            {
                this.Logger.Warning($"Received {packet.GetType().Name} for windowId={packet.WindowId!}, but its not opened");
                // Cache items in case it gets opened in a bit
                this.cachedWindowItemsPacket = packet;
                this.cacheTimestamp = DateTime.Now;

                return Task.CompletedTask;
            }

            window.UpdateSlots(packet.Items!.Select((x, i) =>
            {
                var slot = x.ToSlot();
                slot.SlotNumber = (short)i;
                return slot;
            }).ToArray());
            window.StateId = packet.StateId!;

            if (window.Id == 0 && !this.inventoryLoadedTsc.Task.IsCompleted)
            {
                this.inventoryLoadedTsc.SetResult();
            }

            return Task.CompletedTask;
        }

        private Task handleHeldItemChange(PacketHeldItemSlot packet)
        {
            this.SelectedHotbarIndex = (byte)packet.Slot!;
            this.HeldItemChanged?.Invoke(this.Bot, this.HeldItem);
            return Task.CompletedTask;
        }


        #endregion

        private Window OpenWindow(int id, WindowInfo windowInfo)
        {
            this.Logger.Debug("Opening window with id=" + id);

            if (this.OpenedWindows.TryGetValue(id, out var existingWindow))
            {
                throw new ArgumentException("Window with id " + id + " already opened");
            }

            var window = new Window(id, windowInfo, playerInventory: windowInfo.ExcludeInventory ? null : this.MainInventory);
            window.WindowClicked += this.Window_Clicked;

            this.OpenedWindows.Add(id, window);
            this.WindowOpened?.Invoke(this.Bot, window);

            if (this.cachedWindowItemsPacket != null)
            {
                if (this.cachedWindowItemsPacket.WindowId == id && DateTime.Now - this.cacheTimestamp! <= TimeSpan.FromSeconds(5))
                {
                    // use cache
                    this.Logger.Debug("Applying cache for window with id=" + id);
                    this.handleWindowItems(this.cachedWindowItemsPacket!);
                }

                // delete cache
                this.cachedWindowItemsPacket = null;
                this.cacheTimestamp = null;
            }


            return window;
        }

        private void Window_Clicked(Window window, WindowClick click)
        {
            var windowClickPacket = new Data.Protocol.Play.Serverbound.PacketWindowClick(
                (byte)window.Id, window.StateId, click.Slot, (sbyte)click.Button, (sbyte)click.ClickMode, window.GetAllSlots().Select(x => new Data.Protocol.Play.Serverbound.PacketWindowClick.ChangedSlotsElementContainer(x.SlotNumber, x.ToProtocolSlot())).ToArray(), window.SelectedSlot!.ToProtocolSlot());
            this.Bot.Client.SendPacket(windowClickPacket);
        }

        private void MainInventory_SlotUpdated(Window window, int index)
        {
            if (index == 3 * 9 + 1 + this.SelectedHotbarIndex)
            {
                this.HeldItemChanged?.Invoke(this.Bot, window.GetSlot(index).Item);
            }
        }
    }
}
