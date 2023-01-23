namespace MineSharp.Core.Types
{
    public struct EnchantCost
    {
        public int A;
        public int B;

        public EnchantCost(int a, int b)
        {
            this.A = a;
            this.B = b;
        }
    }

    public class EnchantmentInfo
    {
        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public int MaxLevel { get; }
        public EnchantCost MinCost { get; }
        public EnchantCost MaxCost { get; }
        public bool TreasureOnly { get; }
        public bool Curse { get; }
        public int[] Exclude { get; }
        public int Category { get; }
        public int Weight { get; }
        public bool Tradeable { get; }
        public bool Discoverable { get; }

        public EnchantmentInfo(int id, string name, string displayName, int maxLevel, EnchantCost minCost, EnchantCost maxCost, bool treasureOnly, bool curse, int[] exclude, int category, int weight, bool tradeable, bool discoverable)
        {
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
            this.Tradeable = tradeable;
            this.Discoverable = discoverable;
        }
    }


    public class Enchantment
    {
        public Enchantment(EnchantmentInfo info, int level)
        {
            Info = info;
            Level = level;
        }

        public EnchantmentInfo Info { get; }
        public int Level { get; set; }

        public override string ToString() => $"Enchantment (Name={this.Info.Name} Id={this.Info.Id} Level={this.Level})";
    }
}
