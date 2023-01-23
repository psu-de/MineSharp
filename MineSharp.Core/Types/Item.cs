using fNbt;

namespace MineSharp.Core.Types
{
    public class ItemInfo
    {
        public ItemInfo(int id, string displayName, string name, byte stackSize, int? maxDurability, string[]? enchantCategories, string[]? repairWith)
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.Name = name;
            this.StackSize = stackSize;
            this.MaxDurability = maxDurability;
            this.EnchantCategories = enchantCategories;
            this.RepairWith = repairWith;
        }

        public int Id { get; }
        public string DisplayName { get; }
        public string Name { get; }
        public byte StackSize { get; }
        public int? MaxDurability { get; }
        public string[]? EnchantCategories { get; }
        public string[]? RepairWith { get; }
    }

    public class Item
    {
        public Item(ItemInfo info, byte count, int? damage, NbtCompound? metadata)
        {
            this.Info = info;
            this.Count = count;
            this.Damage = damage;
            this.Metadata = metadata;
        }


        public ItemInfo Info { get; }
        public byte Count { get; set; }
        public int? Damage { get; set; }
        public NbtCompound? Metadata { get; set; } // TODO: Deconstruct metadata

        public override string ToString() => $"Item (Name={this.Info.Name} Id={this.Info.Id} Count={this.Count} Metadata={(this.Metadata == null ? "None" : this.Metadata.ToString())})";


        public Slot ToSlot(short slotNumber) => new Slot(this, slotNumber);

        public Item Clone()
        {
            return new Item(this.Info, this.Count, this.Damage, (NbtCompound?)this.Metadata?.Clone());
        }
        
        private int GetMaterialMultiplier()
        {
            var name = this.Info.Name;
            if (name.Contains("_") && (name.EndsWith("axe") || name.EndsWith("pickaxe") || name.EndsWith("hoe") || name.EndsWith("shovel") || name.EndsWith("sword")))
            {
                var material = name.Split("_")[0];
                switch (material)
                {
                    case "wooden": return 2;
                    case "stone": return 4;
                    case "iron": return 6;
                    case "diamond": return 8;
                    case "netherite": return 9;
                    case "gold": return 12;
                }
            }
            return 1;
        }

        public int GetToolMultiplier(BlockInfo block)
        {

            if (block.Material.Contains("/"))
            {
                var type = block.Material.Split("/")[1];
                if (this.Info.Name.EndsWith(type)) return this.GetMaterialMultiplier();
            }

            return 1;
        }
    }
}
