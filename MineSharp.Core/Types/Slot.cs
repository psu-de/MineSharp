using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Types {
    public class Slot {

        public static Slot Empty = new Slot(-1, 0, 0, null, null);

        public int ItemID { get; set; }
        public short ItemDamage { get; set; }
        public byte Count { get; set; }

        public NbtCompound? Nbt = null;

        public short? SlotNumber { get; set; } = null;

        public Slot() { }

        public Slot(int itemId, short itemDamage, byte count, NbtCompound? nbt, short? slotNumber = null) {
            this.ItemID = itemId;
            this.ItemDamage = itemDamage;
            this.Count = count;
            this.Nbt = nbt;
            this.SlotNumber = slotNumber;
        }

        public bool IsEmpty() => ItemID == -1;
    }
}
