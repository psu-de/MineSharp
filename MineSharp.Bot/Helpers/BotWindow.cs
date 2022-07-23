using MineSharp.Core.Types;
using MineSharp.Data;
using MineSharp.Data.Windows;
using MineSharp.Windows;

namespace MineSharp.Bot.Helpers
{
    public class BotWindow : Window {

        public readonly MinecraftBot Bot;

        public BotWindow(MinecraftBot bot, WindowInfo info, Slot[]? slots = null, Window? playerInventory = null) 
            : base(info, slots, playerInventory) {
            this.Bot = bot;
        }

        public BotWindow(MinecraftBot bot, int windowId, WindowInfo info, Slot[]? slots = null, int? stateId = null,  Window? playerInventory = null) 
            : base(windowId, info, slots, stateId, playerInventory) {
            this.Bot = bot;
        }

        public new async Task PerformClick(WindowClick click) {
            base.PerformClick(click);
            var windowClickPacket = new Data.Protocol.Play.Serverbound.PacketWindowClick(
                (byte)this.Id,
                this.StateId,
                click.Slot,
                (sbyte)click.Button,
                (sbyte)click.ClickMode,
                this.GetAllSlots().Select(x => new Data.Protocol.Play.Serverbound.PacketWindowClick.ChangedSlotsElementContainer(x.SlotNumber, x.ToProtocolSlot())).ToArray(),
                this.SelectedSlot!.ToProtocolSlot());
            await this.Bot.Client.SendPacket(windowClickPacket);
        }

        public new async Task TakeItem(int itemId, byte? count = null, fNbt.NbtCompound? metadata = null,
            bool throwIfNotEnough = false)
        {
            foreach (var click in base.TakeItem(itemId, count, metadata, throwIfNotEnough))
                await this.PerformClick(click);
        }

        public new async Task PutItem(int itemId, byte? count = null, fNbt.NbtCompound? metadata = null)
        {
            foreach (var click in base.PutItem(itemId, count, metadata))
                await this.PerformClick(click);
        }
    }
}
