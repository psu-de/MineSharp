///////////////////////////////////////////////////////////
//   Generated Biome Data for Minecraft Version 1.18.1   //
///////////////////////////////////////////////////////////
using MineSharp.Core.Types;
namespace MineSharp.Data.Biomes {
	public static class BiomePalette {
		public static Type GetBiomeTypeById(int id) => id switch {
			0 => typeof(TheVoid),
			1 => typeof(Plains),
			2 => typeof(SunflowerPlains),
			3 => typeof(SnowyPlains),
			4 => typeof(IceSpikes),
			5 => typeof(Desert),
			6 => typeof(Swamp),
			7 => typeof(Forest),
			8 => typeof(FlowerForest),
			9 => typeof(BirchForest),
			10 => typeof(DarkForest),
			11 => typeof(OldGrowthBirchForest),
			12 => typeof(OldGrowthPineTaiga),
			13 => typeof(OldGrowthSpruceTaiga),
			14 => typeof(Taiga),
			15 => typeof(SnowyTaiga),
			16 => typeof(Savanna),
			17 => typeof(SavannaPlateau),
			18 => typeof(WindsweptHills),
			19 => typeof(WindsweptGravellyHills),
			20 => typeof(WindsweptForest),
			21 => typeof(WindsweptSavanna),
			22 => typeof(Jungle),
			23 => typeof(SparseJungle),
			24 => typeof(BambooJungle),
			25 => typeof(Badlands),
			26 => typeof(ErodedBadlands),
			27 => typeof(WoodedBadlands),
			28 => typeof(Meadow),
			29 => typeof(Grove),
			30 => typeof(SnowySlopes),
			31 => typeof(FrozenPeaks),
			32 => typeof(JaggedPeaks),
			33 => typeof(StonyPeaks),
			34 => typeof(River),
			35 => typeof(FrozenRiver),
			36 => typeof(Beach),
			37 => typeof(SnowyBeach),
			38 => typeof(StonyShore),
			39 => typeof(WarmOcean),
			40 => typeof(LukewarmOcean),
			41 => typeof(DeepLukewarmOcean),
			42 => typeof(Ocean),
			43 => typeof(DeepOcean),
			44 => typeof(ColdOcean),
			45 => typeof(DeepColdOcean),
			46 => typeof(FrozenOcean),
			47 => typeof(DeepFrozenOcean),
			48 => typeof(MushroomFields),
			49 => typeof(DripstoneCaves),
			50 => typeof(LushCaves),
			51 => typeof(NetherWastes),
			52 => typeof(WarpedForest),
			53 => typeof(CrimsonForest),
			54 => typeof(SoulSandValley),
			55 => typeof(BasaltDeltas),
			56 => typeof(TheEnd),
			57 => typeof(EndHighlands),
			58 => typeof(EndMidlands),
			59 => typeof(SmallEndIslands),
			60 => typeof(EndBarrens),
			_ => throw new ArgumentException($"Biome with id {id} not found!")
		};
	}
	public enum BiomeCategory {
		None = 0,
		Plains = 1,
		Icy = 2,
		Desert = 3,
		Swamp = 4,
		Forest = 5,
		Taiga = 6,
		Savanna = 7,
		ExtremeHills = 8,
		Jungle = 9,
		Mesa = 10,
		Mountain = 11,
		River = 12,
		Beach = 13,
		Ocean = 14,
		Mushroom = 15,
		Underground = 16,
		Nether = 17,
		TheEnd = 18,
	}
	public class TheVoid : Biome {
		
		public const int BiomeId = 0;
		public const string BiomeName = "the_void";
		public const string BiomeDisplayName = "The Void";
		public const int BiomeCategory = 0;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 0;
		public const float BiomeRainfall = 0.5F;
		
		public TheVoid() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Plains : Biome {
		
		public const int BiomeId = 1;
		public const string BiomeName = "plains";
		public const string BiomeDisplayName = "Plains";
		public const int BiomeCategory = 1;
		public const float BiomeTemperature = 0.8F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.125F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 9286496;
		public const float BiomeRainfall = 0.4F;
		
		public Plains() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class SunflowerPlains : Biome {
		
		public const int BiomeId = 2;
		public const string BiomeName = "sunflower_plains";
		public const string BiomeDisplayName = "Sunflower Plains";
		public const int BiomeCategory = 1;
		public const float BiomeTemperature = 0.8F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.125F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 11918216;
		public const float BiomeRainfall = 0.4F;
		
		public SunflowerPlains() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class SnowyPlains : Biome {
		
		public const int BiomeId = 3;
		public const string BiomeName = "snowy_plains";
		public const string BiomeDisplayName = "Snowy Plains";
		public const int BiomeCategory = 2;
		public const float BiomeTemperature = 0F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 16777215;
		public const float BiomeRainfall = 0.5F;
		
		public SnowyPlains() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class IceSpikes : Biome {
		
		public const int BiomeId = 4;
		public const string BiomeName = "ice_spikes";
		public const string BiomeDisplayName = "Ice Spikes";
		public const int BiomeCategory = 2;
		public const float BiomeTemperature = 0F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = 0.425F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 11853020;
		public const float BiomeRainfall = 0.5F;
		
		public IceSpikes() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Desert : Biome {
		
		public const int BiomeId = 5;
		public const string BiomeName = "desert";
		public const string BiomeDisplayName = "Desert";
		public const int BiomeCategory = 3;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.125F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 16421912;
		public const float BiomeRainfall = 0F;
		
		public Desert() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Swamp : Biome {
		
		public const int BiomeId = 6;
		public const string BiomeName = "swamp";
		public const string BiomeDisplayName = "Swamp";
		public const int BiomeCategory = 4;
		public const float BiomeTemperature = 0.8F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -0.2F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 522674;
		public const float BiomeRainfall = 0.9F;
		
		public Swamp() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Forest : Biome {
		
		public const int BiomeId = 7;
		public const string BiomeName = "forest";
		public const string BiomeDisplayName = "Forest";
		public const int BiomeCategory = 5;
		public const float BiomeTemperature = 0.7F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 353825;
		public const float BiomeRainfall = 0.8F;
		
		public Forest() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class FlowerForest : Biome {
		
		public const int BiomeId = 8;
		public const string BiomeName = "flower_forest";
		public const string BiomeDisplayName = "Flower Forest";
		public const int BiomeCategory = 5;
		public const float BiomeTemperature = 0.7F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 2985545;
		public const float BiomeRainfall = 0.8F;
		
		public FlowerForest() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class BirchForest : Biome {
		
		public const int BiomeId = 9;
		public const string BiomeName = "birch_forest";
		public const string BiomeDisplayName = "Birch Forest";
		public const int BiomeCategory = 5;
		public const float BiomeTemperature = 0.6F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 3175492;
		public const float BiomeRainfall = 0.6F;
		
		public BirchForest() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class DarkForest : Biome {
		
		public const int BiomeId = 10;
		public const string BiomeName = "dark_forest";
		public const string BiomeDisplayName = "Dark Forest";
		public const int BiomeCategory = 5;
		public const float BiomeTemperature = 0.7F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 4215066;
		public const float BiomeRainfall = 0.8F;
		
		public DarkForest() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class OldGrowthBirchForest : Biome {
		
		public const int BiomeId = 11;
		public const string BiomeName = "old_growth_birch_forest";
		public const string BiomeDisplayName = "Old Growth Birch Forest";
		public const int BiomeCategory = 5;
		public const float BiomeTemperature = 0.6F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 5807212;
		public const float BiomeRainfall = 0.6F;
		
		public OldGrowthBirchForest() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class OldGrowthPineTaiga : Biome {
		
		public const int BiomeId = 12;
		public const string BiomeName = "old_growth_pine_taiga";
		public const string BiomeDisplayName = "Old Growth Pine Taiga";
		public const int BiomeCategory = 6;
		public const float BiomeTemperature = 0.3F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 5858897;
		public const float BiomeRainfall = 0.8F;
		
		public OldGrowthPineTaiga() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class OldGrowthSpruceTaiga : Biome {
		
		public const int BiomeId = 13;
		public const string BiomeName = "old_growth_spruce_taiga";
		public const string BiomeDisplayName = "Old Growth Spruce Taiga";
		public const int BiomeCategory = 6;
		public const float BiomeTemperature = 0.25F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 8490617;
		public const float BiomeRainfall = 0.8F;
		
		public OldGrowthSpruceTaiga() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Taiga : Biome {
		
		public const int BiomeId = 14;
		public const string BiomeName = "taiga";
		public const string BiomeDisplayName = "Taiga";
		public const int BiomeCategory = 6;
		public const float BiomeTemperature = 0.25F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.2F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 747097;
		public const float BiomeRainfall = 0.8F;
		
		public Taiga() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class SnowyTaiga : Biome {
		
		public const int BiomeId = 15;
		public const string BiomeName = "snowy_taiga";
		public const string BiomeDisplayName = "Snowy Taiga";
		public const int BiomeCategory = 6;
		public const float BiomeTemperature = -0.5F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = 0.2F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 3233098;
		public const float BiomeRainfall = 0.4F;
		
		public SnowyTaiga() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Savanna : Biome {
		
		public const int BiomeId = 16;
		public const string BiomeName = "savanna";
		public const string BiomeDisplayName = "Savanna";
		public const int BiomeCategory = 7;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.125F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 12431967;
		public const float BiomeRainfall = 0F;
		
		public Savanna() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class SavannaPlateau : Biome {
		
		public const int BiomeId = 17;
		public const string BiomeName = "savanna_plateau";
		public const string BiomeDisplayName = "Savanna Plateau";
		public const int BiomeCategory = 7;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 1.5F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 10984804;
		public const float BiomeRainfall = 0F;
		
		public SavannaPlateau() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class WindsweptHills : Biome {
		
		public const int BiomeId = 18;
		public const string BiomeName = "windswept_hills";
		public const string BiomeDisplayName = "Windswept Hills";
		public const int BiomeCategory = 8;
		public const float BiomeTemperature = 0.2F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 6316128;
		public const float BiomeRainfall = 0.3F;
		
		public WindsweptHills() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class WindsweptGravellyHills : Biome {
		
		public const int BiomeId = 19;
		public const string BiomeName = "windswept_gravelly_hills";
		public const string BiomeDisplayName = "Windswept Gravelly Hills";
		public const int BiomeCategory = 8;
		public const float BiomeTemperature = 0.2F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 8947848;
		public const float BiomeRainfall = 0.3F;
		
		public WindsweptGravellyHills() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class WindsweptForest : Biome {
		
		public const int BiomeId = 20;
		public const string BiomeName = "windswept_forest";
		public const string BiomeDisplayName = "Windswept Forest";
		public const int BiomeCategory = 8;
		public const float BiomeTemperature = 0.2F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 2250012;
		public const float BiomeRainfall = 0.3F;
		
		public WindsweptForest() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class WindsweptSavanna : Biome {
		
		public const int BiomeId = 21;
		public const string BiomeName = "windswept_savanna";
		public const string BiomeDisplayName = "Windswept Savanna";
		public const int BiomeCategory = 7;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 15063687;
		public const float BiomeRainfall = 0F;
		
		public WindsweptSavanna() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Jungle : Biome {
		
		public const int BiomeId = 22;
		public const string BiomeName = "jungle";
		public const string BiomeDisplayName = "Jungle";
		public const int BiomeCategory = 9;
		public const float BiomeTemperature = 0.95F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 5470985;
		public const float BiomeRainfall = 0.9F;
		
		public Jungle() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class SparseJungle : Biome {
		
		public const int BiomeId = 23;
		public const string BiomeName = "sparse_jungle";
		public const string BiomeDisplayName = "Sparse Jungle";
		public const int BiomeCategory = 9;
		public const float BiomeTemperature = 0.95F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 6458135;
		public const float BiomeRainfall = 0.8F;
		
		public SparseJungle() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class BambooJungle : Biome {
		
		public const int BiomeId = 24;
		public const string BiomeName = "bamboo_jungle";
		public const string BiomeDisplayName = "Bamboo Jungle";
		public const int BiomeCategory = 9;
		public const float BiomeTemperature = 0.95F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 7769620;
		public const float BiomeRainfall = 0.9F;
		
		public BambooJungle() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Badlands : Biome {
		
		public const int BiomeId = 25;
		public const string BiomeName = "badlands";
		public const string BiomeDisplayName = "Badlands";
		public const int BiomeCategory = 10;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 14238997;
		public const float BiomeRainfall = 0F;
		
		public Badlands() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class ErodedBadlands : Biome {
		
		public const int BiomeId = 26;
		public const string BiomeName = "eroded_badlands";
		public const string BiomeDisplayName = "Eroded Badlands";
		public const int BiomeCategory = 10;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 16739645;
		public const float BiomeRainfall = 0F;
		
		public ErodedBadlands() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class WoodedBadlands : Biome {
		
		public const int BiomeId = 27;
		public const string BiomeName = "wooded_badlands";
		public const string BiomeDisplayName = "Wooded Badlands";
		public const int BiomeCategory = 10;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 11573093;
		public const float BiomeRainfall = 0F;
		
		public WoodedBadlands() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Meadow : Biome {
		
		public const int BiomeId = 28;
		public const string BiomeName = "meadow";
		public const string BiomeDisplayName = "Meadow";
		public const int BiomeCategory = 11;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 9217136;
		public const float BiomeRainfall = 0.8F;
		
		public Meadow() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Grove : Biome {
		
		public const int BiomeId = 29;
		public const string BiomeName = "grove";
		public const string BiomeDisplayName = "Grove";
		public const int BiomeCategory = 5;
		public const float BiomeTemperature = -0.2F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 14675173;
		public const float BiomeRainfall = 0.8F;
		
		public Grove() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class SnowySlopes : Biome {
		
		public const int BiomeId = 30;
		public const string BiomeName = "snowy_slopes";
		public const string BiomeDisplayName = "Snowy Slopes";
		public const int BiomeCategory = 11;
		public const float BiomeTemperature = -0.3F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 14348785;
		public const float BiomeRainfall = 0.9F;
		
		public SnowySlopes() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class FrozenPeaks : Biome {
		
		public const int BiomeId = 31;
		public const string BiomeName = "frozen_peaks";
		public const string BiomeDisplayName = "Frozen Peaks";
		public const int BiomeCategory = 11;
		public const float BiomeTemperature = -0.7F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 15399931;
		public const float BiomeRainfall = 0.9F;
		
		public FrozenPeaks() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class JaggedPeaks : Biome {
		
		public const int BiomeId = 32;
		public const string BiomeName = "jagged_peaks";
		public const string BiomeDisplayName = "Jagged Peaks";
		public const int BiomeCategory = 11;
		public const float BiomeTemperature = -0.7F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 14937325;
		public const float BiomeRainfall = 0.9F;
		
		public JaggedPeaks() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class StonyPeaks : Biome {
		
		public const int BiomeId = 33;
		public const string BiomeName = "stony_peaks";
		public const string BiomeDisplayName = "Stony Peaks";
		public const int BiomeCategory = 11;
		public const float BiomeTemperature = 1F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 13750737;
		public const float BiomeRainfall = 0.3F;
		
		public StonyPeaks() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class River : Biome {
		
		public const int BiomeId = 34;
		public const string BiomeName = "river";
		public const string BiomeDisplayName = "River";
		public const int BiomeCategory = 12;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -0.5F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 255;
		public const float BiomeRainfall = 0.5F;
		
		public River() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class FrozenRiver : Biome {
		
		public const int BiomeId = 35;
		public const string BiomeName = "frozen_river";
		public const string BiomeDisplayName = "Frozen River";
		public const int BiomeCategory = 12;
		public const float BiomeTemperature = 0F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = -0.5F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 10526975;
		public const float BiomeRainfall = 0.5F;
		
		public FrozenRiver() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Beach : Biome {
		
		public const int BiomeId = 36;
		public const string BiomeName = "beach";
		public const string BiomeDisplayName = "Beach";
		public const int BiomeCategory = 13;
		public const float BiomeTemperature = 0.8F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 16440917;
		public const float BiomeRainfall = 0.4F;
		
		public Beach() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class SnowyBeach : Biome {
		
		public const int BiomeId = 37;
		public const string BiomeName = "snowy_beach";
		public const string BiomeDisplayName = "Snowy Beach";
		public const int BiomeCategory = 13;
		public const float BiomeTemperature = 0.05F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 16445632;
		public const float BiomeRainfall = 0.3F;
		
		public SnowyBeach() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class StonyShore : Biome {
		
		public const int BiomeId = 38;
		public const string BiomeName = "stony_shore";
		public const string BiomeDisplayName = "Stony Shore";
		public const int BiomeCategory = 13;
		public const float BiomeTemperature = 0.2F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 10658436;
		public const float BiomeRainfall = 0.3F;
		
		public StonyShore() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class WarmOcean : Biome {
		
		public const int BiomeId = 39;
		public const string BiomeName = "warm_ocean";
		public const string BiomeDisplayName = "Warm Ocean";
		public const int BiomeCategory = 14;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 172;
		public const float BiomeRainfall = 0.5F;
		
		public WarmOcean() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class LukewarmOcean : Biome {
		
		public const int BiomeId = 40;
		public const string BiomeName = "lukewarm_ocean";
		public const string BiomeDisplayName = "Lukewarm Ocean";
		public const int BiomeCategory = 14;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 144;
		public const float BiomeRainfall = 0.5F;
		
		public LukewarmOcean() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class DeepLukewarmOcean : Biome {
		
		public const int BiomeId = 41;
		public const string BiomeName = "deep_lukewarm_ocean";
		public const string BiomeDisplayName = "Deep Lukewarm Ocean";
		public const int BiomeCategory = 14;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -1.8F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 64;
		public const float BiomeRainfall = 0.5F;
		
		public DeepLukewarmOcean() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class Ocean : Biome {
		
		public const int BiomeId = 42;
		public const string BiomeName = "ocean";
		public const string BiomeDisplayName = "Ocean";
		public const int BiomeCategory = 14;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 112;
		public const float BiomeRainfall = 0.5F;
		
		public Ocean() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class DeepOcean : Biome {
		
		public const int BiomeId = 43;
		public const string BiomeName = "deep_ocean";
		public const string BiomeDisplayName = "Deep Ocean";
		public const int BiomeCategory = 14;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -1.8F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 48;
		public const float BiomeRainfall = 0.5F;
		
		public DeepOcean() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class ColdOcean : Biome {
		
		public const int BiomeId = 44;
		public const string BiomeName = "cold_ocean";
		public const string BiomeDisplayName = "Cold Ocean";
		public const int BiomeCategory = 14;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 2105456;
		public const float BiomeRainfall = 0.5F;
		
		public ColdOcean() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class DeepColdOcean : Biome {
		
		public const int BiomeId = 45;
		public const string BiomeName = "deep_cold_ocean";
		public const string BiomeDisplayName = "Deep Cold Ocean";
		public const int BiomeCategory = 14;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -1.8F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 2105400;
		public const float BiomeRainfall = 0.5F;
		
		public DeepColdOcean() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class FrozenOcean : Biome {
		
		public const int BiomeId = 46;
		public const string BiomeName = "frozen_ocean";
		public const string BiomeDisplayName = "Frozen Ocean";
		public const int BiomeCategory = 14;
		public const float BiomeTemperature = 0F;
		public const string BiomePrecipitation = "snow";
		public const float BiomeDepth = -1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 7368918;
		public const float BiomeRainfall = 0.5F;
		
		public FrozenOcean() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class DeepFrozenOcean : Biome {
		
		public const int BiomeId = 47;
		public const string BiomeName = "deep_frozen_ocean";
		public const string BiomeDisplayName = "Deep Frozen Ocean";
		public const int BiomeCategory = 14;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = -1.8F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 4210832;
		public const float BiomeRainfall = 0.5F;
		
		public DeepFrozenOcean() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class MushroomFields : Biome {
		
		public const int BiomeId = 48;
		public const string BiomeName = "mushroom_fields";
		public const string BiomeDisplayName = "Mushroom Fields";
		public const int BiomeCategory = 15;
		public const float BiomeTemperature = 0.9F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.2F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 16711935;
		public const float BiomeRainfall = 1F;
		
		public MushroomFields() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class DripstoneCaves : Biome {
		
		public const int BiomeId = 49;
		public const string BiomeName = "dripstone_caves";
		public const string BiomeDisplayName = "Dripstone Caves";
		public const int BiomeCategory = 16;
		public const float BiomeTemperature = 0.8F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.125F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 12690831;
		public const float BiomeRainfall = 0.4F;
		
		public DripstoneCaves() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class LushCaves : Biome {
		
		public const int BiomeId = 50;
		public const string BiomeName = "lush_caves";
		public const string BiomeDisplayName = "Lush Caves";
		public const int BiomeCategory = 16;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "rain";
		public const float BiomeDepth = 0.5F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Overworld;
		public const int BiomeColor = 14652980;
		public const float BiomeRainfall = 0.5F;
		
		public LushCaves() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class NetherWastes : Biome {
		
		public const int BiomeId = 51;
		public const string BiomeName = "nether_wastes";
		public const string BiomeDisplayName = "Nether Wastes";
		public const int BiomeCategory = 17;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Nether;
		public const int BiomeColor = 12532539;
		public const float BiomeRainfall = 0F;
		
		public NetherWastes() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class WarpedForest : Biome {
		
		public const int BiomeId = 52;
		public const string BiomeName = "warped_forest";
		public const string BiomeDisplayName = "Warped Forest";
		public const int BiomeCategory = 17;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Nether;
		public const int BiomeColor = 4821115;
		public const float BiomeRainfall = 0F;
		
		public WarpedForest() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class CrimsonForest : Biome {
		
		public const int BiomeId = 53;
		public const string BiomeName = "crimson_forest";
		public const string BiomeDisplayName = "Crimson Forest";
		public const int BiomeCategory = 17;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Nether;
		public const int BiomeColor = 14485512;
		public const float BiomeRainfall = 0F;
		
		public CrimsonForest() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class SoulSandValley : Biome {
		
		public const int BiomeId = 54;
		public const string BiomeName = "soul_sand_valley";
		public const string BiomeDisplayName = "Soul Sand Valley";
		public const int BiomeCategory = 17;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Nether;
		public const int BiomeColor = 6174768;
		public const float BiomeRainfall = 0F;
		
		public SoulSandValley() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class BasaltDeltas : Biome {
		
		public const int BiomeId = 55;
		public const string BiomeName = "basalt_deltas";
		public const string BiomeDisplayName = "Basalt Deltas";
		public const int BiomeCategory = 17;
		public const float BiomeTemperature = 2F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.Nether;
		public const int BiomeColor = 4208182;
		public const float BiomeRainfall = 0F;
		
		public BasaltDeltas() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class TheEnd : Biome {
		
		public const int BiomeId = 56;
		public const string BiomeName = "the_end";
		public const string BiomeDisplayName = "The End";
		public const int BiomeCategory = 18;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.End;
		public const int BiomeColor = 8421631;
		public const float BiomeRainfall = 0.5F;
		
		public TheEnd() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class EndHighlands : Biome {
		
		public const int BiomeId = 57;
		public const string BiomeName = "end_highlands";
		public const string BiomeDisplayName = "End Highlands";
		public const int BiomeCategory = 18;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.End;
		public const int BiomeColor = 12828041;
		public const float BiomeRainfall = 0.5F;
		
		public EndHighlands() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class EndMidlands : Biome {
		
		public const int BiomeId = 58;
		public const string BiomeName = "end_midlands";
		public const string BiomeDisplayName = "End Midlands";
		public const int BiomeCategory = 18;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.End;
		public const int BiomeColor = 15464630;
		public const float BiomeRainfall = 0.5F;
		
		public EndMidlands() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class SmallEndIslands : Biome {
		
		public const int BiomeId = 59;
		public const string BiomeName = "small_end_islands";
		public const string BiomeDisplayName = "Small End Islands";
		public const int BiomeCategory = 18;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.End;
		public const int BiomeColor = 42;
		public const float BiomeRainfall = 0.5F;
		
		public SmallEndIslands() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public class EndBarrens : Biome {
		
		public const int BiomeId = 60;
		public const string BiomeName = "end_barrens";
		public const string BiomeDisplayName = "End Barrens";
		public const int BiomeCategory = 18;
		public const float BiomeTemperature = 0.5F;
		public const string BiomePrecipitation = "none";
		public const float BiomeDepth = 0.1F;
		public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.End;
		public const int BiomeColor = 9474162;
		public const float BiomeRainfall = 0.5F;
		
		public EndBarrens() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) { }
	}
	public enum BiomeType {
		TheVoid = 0,
		Plains = 1,
		SunflowerPlains = 2,
		SnowyPlains = 3,
		IceSpikes = 4,
		Desert = 5,
		Swamp = 6,
		Forest = 7,
		FlowerForest = 8,
		BirchForest = 9,
		DarkForest = 10,
		OldGrowthBirchForest = 11,
		OldGrowthPineTaiga = 12,
		OldGrowthSpruceTaiga = 13,
		Taiga = 14,
		SnowyTaiga = 15,
		Savanna = 16,
		SavannaPlateau = 17,
		WindsweptHills = 18,
		WindsweptGravellyHills = 19,
		WindsweptForest = 20,
		WindsweptSavanna = 21,
		Jungle = 22,
		SparseJungle = 23,
		BambooJungle = 24,
		Badlands = 25,
		ErodedBadlands = 26,
		WoodedBadlands = 27,
		Meadow = 28,
		Grove = 29,
		SnowySlopes = 30,
		FrozenPeaks = 31,
		JaggedPeaks = 32,
		StonyPeaks = 33,
		River = 34,
		FrozenRiver = 35,
		Beach = 36,
		SnowyBeach = 37,
		StonyShore = 38,
		WarmOcean = 39,
		LukewarmOcean = 40,
		DeepLukewarmOcean = 41,
		Ocean = 42,
		DeepOcean = 43,
		ColdOcean = 44,
		DeepColdOcean = 45,
		FrozenOcean = 46,
		DeepFrozenOcean = 47,
		MushroomFields = 48,
		DripstoneCaves = 49,
		LushCaves = 50,
		NetherWastes = 51,
		WarpedForest = 52,
		CrimsonForest = 53,
		SoulSandValley = 54,
		BasaltDeltas = 55,
		TheEnd = 56,
		EndHighlands = 57,
		EndMidlands = 58,
		SmallEndIslands = 59,
		EndBarrens = 60,
	}
}
