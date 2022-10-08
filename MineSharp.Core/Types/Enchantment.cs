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


    public abstract class Enchantment
    {

        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public int MaxLevel { get; }
        public EnchantCost MinCost { get; }
        public EnchantCost MaxCost { get; }
        public bool TreasureOnly { get; }
        public bool Curse { get; }
        public Type[] Exclude { get; }
        public int Category { get; }
        public int Weight { get; }
        public bool Discoverable { get; }

        public int Level { get; set; }


        public Enchantment(int id, string name, string displayName, int maxLevel, EnchantCost minCost, EnchantCost maxCost, bool treasureOnly, bool curse, Type[] exclude, int category, int weight, bool discoverable)
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
            this.Discoverable = discoverable;
        }

        public Enchantment(int level, int id, string name, string displayName, int maxLevel, EnchantCost minCost, EnchantCost maxCost, bool treasureOnly, bool curse, Type[] exclude, int category, int weight, bool discoverable)
            : this(id, name, displayName, maxLevel, minCost, maxCost, treasureOnly, curse, exclude, category, weight, discoverable)
        {
            this.Level = level;
        }

        public override string ToString() => $"Enchantment (Name={this.Name} Id={this.Id} Level={this.Level})";
    }
}
