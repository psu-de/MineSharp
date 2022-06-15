using System.Reflection;

namespace MineSharp.Core.Types {
    public class Biome {

        public int Id { get; }
        public string Name { get; }
        public int BiomeCategory { get; }
        public float Temperature { get; }
        public string Precipitation { get; }
        public float Depth { get; }
        public Enums.Dimension Dimension { get; }
        public string DisplayName { get; }
        public int Color { get; }
        public float Rainfall { get; }

        public Biome(int id, string name, string displayName, int biomeCategory, float temperature, string precipitation, float depth, Enums.Dimension dimension, int color, float rainfall) {
            this.Id = id;
            this.Name = name;
            this.BiomeCategory = biomeCategory;
            this.Temperature = temperature;
            this.Precipitation = precipitation;
            this.Depth = depth;
            this.Dimension = dimension;
            this.DisplayName = displayName;
            this.Color = color;
            this.Rainfall = rainfall;
        }
    }
}
