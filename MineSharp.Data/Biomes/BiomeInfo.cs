using MineSharp.Core.Types.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Biomes {
    public class BiomeInfo {
        public int Id { get; set; }
        public string Name { get; set; }
        public BiomeType Type { get; set; }
        public BiomeCategory BiomeCategory { get; set; }
        public float Temperature { get; set; }
        public string Precipitation { get; set; }
        public float Depth { get; set; }
        public MineSharp.Core.Types.Enums.Dimension Dimension { get; set; }
        public string DisplayName { get; set; }
        public int Color { get; set; }
        public float Rainfall { get; set; }

        public BiomeInfo() { }

        public BiomeInfo(int id, string name, BiomeType type, BiomeCategory biomeCategory, float temperature, string precipitation, float depth, Dimension dimension, string displayName, int color, float rainfall) {
            Id = id;
            Name = name;
            this.Type = type;
            BiomeCategory = biomeCategory;
            Temperature = temperature;
            Precipitation = precipitation;
            Depth = depth;
            Dimension = dimension;
            DisplayName = displayName;
            Color = color;
            Rainfall = rainfall;
        }
    }
}
