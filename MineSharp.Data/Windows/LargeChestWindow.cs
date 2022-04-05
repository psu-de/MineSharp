using MineSharp.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Windows {
    public class LargeChestWindow : ChestWindow {

        public override byte WindowType => 5;

        public override string Title => "Large Chest";

        public override int InventoryStart => 6 * 9;

        protected override ItemInfo[]? GetAllowedItemsForSlot(int slotIndex) => null;

        public LargeChestWindow() : base(89) { }

    }
}
