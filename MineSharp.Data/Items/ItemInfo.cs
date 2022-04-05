using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Items {
    public class ItemInfo {

        public ItemType Id { get; private set; }
        public string DisplayName{ get; private set; }
        public string Name { get; private set; }
        public byte StackSize { get; private set; }
        public int? MaxDurability { get; private set; }  
        public string[]? EnchantCategories { get; private set; }
        public string[]? RepairWith { get; private set; }

        public ItemInfo(ItemType id, string displayName, string name, byte stackSize, int? maxDurability, string[]? enchantCategories, string[]? repairWith) {
            Id = id;
            DisplayName = displayName;
            Name = name;
            StackSize = stackSize;
            MaxDurability = maxDurability;
            EnchantCategories = enchantCategories;
            RepairWith = repairWith;
        }

        private int GetMaterialMultiplier() {
            if (this.Name.Contains("_") && (this.Name.EndsWith("axe") || this.Name.EndsWith("pickaxe") || this.Name.EndsWith("hoe") || this.Name.EndsWith("shovel") || this.Name.EndsWith("sword"))) {
                string material = this.Name.Split("_")[0];
                switch (material) {
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

        public bool CanHarvest(Blocks.BlockInfo info) {

            if (info.HarvestTools == null) return true;

            return info.HarvestTools.Contains(this.Id);
        }

        public int GetToolMultiplier(Blocks.BlockInfo info) {

            if (info.Material.Contains("/")) {
                string type = info.Material.Split("/")[1];
                if (this.Name.EndsWith(type)) return GetMaterialMultiplier();
            }

            return 1;
        }
    }
}
