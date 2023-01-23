using MineSharp.Core.Types.Enums;

namespace MineSharp.Core.Types
{
    public class BiomeInfo
    {
        public BiomeInfo(int id, string name, int category, float temperature, string precipitation, float depth, Dimension dimension, string displayName, int color, float rainfall)
        {
            Id = id;
            Name = name;
            Category = category;
            Temperature = temperature;
            Precipitation = precipitation;
            Depth = depth;
            Dimension = dimension;
            DisplayName = displayName;
            Color = color;
            Rainfall = rainfall;
        }

        public int Id { get; }
        public string Name { get; }
        public int Category { get; }
        public float Temperature { get; }
        public string Precipitation { get; }
        public float Depth { get; }
        public Dimension Dimension { get; }
        public string DisplayName { get; }
        public int Color { get; }
        public float Rainfall { get; }


        public override string ToString() => $"Biome (Name={this.Name} Id={this.Id})";
    }

    public class Biome
    {
        public BiomeInfo Info { get; }

        public Biome(BiomeInfo info)
        {
            this.Info = info;
        }
    }
}
