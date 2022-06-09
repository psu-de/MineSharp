using MineSharp.Core.Types;
using MineSharp.Data.Windows;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Windows;

namespace MineSharp.Bot.Modules {
    public class WindowsModule : Module {

        private Window MainInventory { get; set; }

        public Window? Inventory { get; private set; }
        public Dictionary<int, Window> OpenedWindows = new Dictionary<int, Window>();

        public WindowsModule(MinecraftBot bot) : base(bot) {
        }

        protected override async Task Load() {

            this.MainInventory = new Window(new WindowInfo((Identifier)"", "", 4 * 9, true));

            this.Bot.On<WindowItemsPacket>(this.handleWindowItems);
            this.Bot.On<SetSlotPacket>(this.handleSetSlot);

            this.Inventory = OpenWindow(0, new WindowInfo((Identifier)"", "Inventory", 9, hasOffHandSlot: true));
            this.OpenedWindows.Add(0, this.Inventory);

            await this.SetEnabled(true);
        }
        
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
            window.SetSlot(slot);
            window.StateId = packet.StateID;

            return Task.CompletedTask;
        }

        private Task handleWindowItems(WindowItemsPacket packet) {

            if (!OpenedWindows.TryGetValue(packet.WindowID, out var window)) {
                Logger.Warning($"Received {packet.GetType().Name} for windowId={packet.WindowID}, but its not opened");
                return Task.CompletedTask;
            }

            window.UpdateSlots(packet.SlotData);
            window.StateId = packet.StateID;

            return Task.CompletedTask;
        }

        private Window OpenWindow(int id, WindowInfo windowInfo) {
            var window = new Window(id, windowInfo);
            if (!windowInfo.ExcludeInventory) {
                window.Extend(this.MainInventory);
            }
            window.WindowClicked += Window_Clicked;

            return window;
        }

        private void Window_Clicked(Window window, WindowClick click) {
            var windowClickPacket = new Protocol.Packets.Serverbound.Play.ClickWindowPacket(
                (byte)window.Id, window.StateId, click.Slot, click.Button, click.ClickMode, window.GetSlotData(), window.SelectedSlot);
            this.Bot.Client.SendPacket(windowClickPacket);
        }
    }
}
