using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Enchantments {
    public class EnchantmentInfo {

        public EnchantmentType Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int MaxLevel { get; set; }
        public EnchantCost MinCost { get; set; }
        public EnchantCost MaxCost { get; set; }
        public bool TreasureOnly { get; set; }
        public bool Curse { get; set; }
        public string[] Exclude { get; set; }
        public EnchantmentCategory Category { get; set; }
        public int Weight { get; set; }
        public bool Discoverable { get; set; }




        public EnchantmentInfo(EnchantmentType id, string name, string displayName, int maxLevel, EnchantCost minCost, EnchantCost maxCost, bool treasureOnly, bool curse, string[] exclude, EnchantmentCategory category, int weight, bool discoverable) {
            this.Id = id;
            this.Name = name;
            this.DisplayName = displayName;
            this.MaxLevel = maxLevel;
            this.MinCost = minCost;
            this.MaxCost = maxCost;
            this.TreasureOnly = treasureOnly;
            this.Curse = curse;
            this.Exclude = exclude;
            this.Category = category;
            this.Weight = weight;
            this.Discoverable = discoverable;
        }

    }

    public struct EnchantCost {
        public int A;
        public int B;

        public EnchantCost(int a, int b) {
            this.A = a;
            this.B = b;
        }
    }
}
