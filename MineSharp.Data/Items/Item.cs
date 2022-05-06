using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Items {
    public class Item {

        public ItemInfo Info { get; set; }
        public byte Count { get; set; }
        public int? Damage { get; set; }
        public fNbt.NbtCompound? Metadata { get; set; } // TODO: Deconstruct metadata


        public Item(ItemInfo info, byte count, int? damage, fNbt.NbtCompound? metadata) {
            Info = info;
            Count = count;
            Damage = damage;
            Metadata = metadata;
        }

        public override string ToString() {
            return $"Item(id={(int)Info.Id}, Name={Info.Name}, Count={Count}, Metadata={(Metadata == null ? "None" : Metadata.ToString())})";
        }


        public Slot ToSlot() {
            return new Slot((int)Info.Id, (short)Damage, Count, Metadata);
        }
    }
}
