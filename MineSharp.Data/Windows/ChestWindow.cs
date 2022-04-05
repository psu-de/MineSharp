using MineSharp.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Windows {
    public class ChestWindow : Window {
        public override byte WindowType => 2;

        public override string Title => "Chest";

        public override int InventoryStart => 3 * 9;

        protected override ItemInfo[]? GetAllowedItemsForSlot(int slotIndex) => null;

        public ChestWindow() : base(62) { }

        internal ChestWindow(int slotCount) : base(slotCount) { }
    }
}
