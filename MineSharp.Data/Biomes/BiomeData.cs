

/////////////////////////////////////////////////////////
//  Generated Biome Data for Minecraft Version 1.18.1  //
/////////////////////////////////////////////////////////

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
						_ => throw new ArgumentException("Biome with id " + id + " not found!")
    
        };


		public static Biome CreateBiome(Type type) {

			if (!type.IsAssignableTo(typeof(Biome)))
				throw new ArgumentException();

			return (Biome)Activator.CreateInstance(type)!;
		}

		public static Biome CreateBiome(int id) {
			var type = GetBiomeTypeById(id);
			return CreateBiome(type);
		}
		
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
		

		public TheVoid() : base(0, "the_void", "The Void", 0, 0.5F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 0, 0.5F) {}

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
		

		public Plains() : base(1, "plains", "Plains", 1, 0.8F, "rain", 0.125F, MineSharp.Core.Types.Enums.Dimension.Overworld, 9286496, 0.4F) {}

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
		

		public SunflowerPlains() : base(2, "sunflower_plains", "Sunflower Plains", 1, 0.8F, "rain", 0.125F, MineSharp.Core.Types.Enums.Dimension.Overworld, 11918216, 0.4F) {}

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
		

		public SnowyPlains() : base(3, "snowy_plains", "Snowy Plains", 2, 0F, "snow", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 16777215, 0.5F) {}

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
		

		public IceSpikes() : base(4, "ice_spikes", "Ice Spikes", 2, 0F, "snow", 0.425F, MineSharp.Core.Types.Enums.Dimension.Overworld, 11853020, 0.5F) {}

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
		

		public Desert() : base(5, "desert", "Desert", 3, 2F, "none", 0.125F, MineSharp.Core.Types.Enums.Dimension.Overworld, 16421912, 0F) {}

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
		

		public Swamp() : base(6, "swamp", "Swamp", 4, 0.8F, "rain", -0.2F, MineSharp.Core.Types.Enums.Dimension.Overworld, 522674, 0.9F) {}

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
		

		public Forest() : base(7, "forest", "Forest", 5, 0.7F, "rain", 0.1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 353825, 0.8F) {}

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
		

		public FlowerForest() : base(8, "flower_forest", "Flower Forest", 5, 0.7F, "rain", 0.1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 2985545, 0.8F) {}

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
		

		public BirchForest() : base(9, "birch_forest", "Birch Forest", 5, 0.6F, "rain", 0.1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 3175492, 0.6F) {}

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
		

		public DarkForest() : base(10, "dark_forest", "Dark Forest", 5, 0.7F, "rain", 0.1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 4215066, 0.8F) {}

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
		

		public OldGrowthBirchForest() : base(11, "old_growth_birch_forest", "Old Growth Birch Forest", 5, 0.6F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 5807212, 0.6F) {}

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
		

		public OldGrowthPineTaiga() : base(12, "old_growth_pine_taiga", "Old Growth Pine Taiga", 6, 0.3F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 5858897, 0.8F) {}

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
		

		public OldGrowthSpruceTaiga() : base(13, "old_growth_spruce_taiga", "Old Growth Spruce Taiga", 6, 0.25F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 8490617, 0.8F) {}

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
		

		public Taiga() : base(14, "taiga", "Taiga", 6, 0.25F, "rain", 0.2F, MineSharp.Core.Types.Enums.Dimension.Overworld, 747097, 0.8F) {}

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
		

		public SnowyTaiga() : base(15, "snowy_taiga", "Snowy Taiga", 6, -0.5F, "snow", 0.2F, MineSharp.Core.Types.Enums.Dimension.Overworld, 3233098, 0.4F) {}

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
		

		public Savanna() : base(16, "savanna", "Savanna", 7, 2F, "none", 0.125F, MineSharp.Core.Types.Enums.Dimension.Overworld, 12431967, 0F) {}

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
		

		public SavannaPlateau() : base(17, "savanna_plateau", "Savanna Plateau", 7, 2F, "none", 1.5F, MineSharp.Core.Types.Enums.Dimension.Overworld, 10984804, 0F) {}

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
		

		public WindsweptHills() : base(18, "windswept_hills", "Windswept Hills", 8, 0.2F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 6316128, 0.3F) {}

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
		

		public WindsweptGravellyHills() : base(19, "windswept_gravelly_hills", "Windswept Gravelly Hills", 8, 0.2F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 8947848, 0.3F) {}

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
		

		public WindsweptForest() : base(20, "windswept_forest", "Windswept Forest", 8, 0.2F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 2250012, 0.3F) {}

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
		

		public WindsweptSavanna() : base(21, "windswept_savanna", "Windswept Savanna", 7, 2F, "none", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 15063687, 0F) {}

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
		

		public Jungle() : base(22, "jungle", "Jungle", 9, 0.95F, "rain", 0.1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 5470985, 0.9F) {}

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
		

		public SparseJungle() : base(23, "sparse_jungle", "Sparse Jungle", 9, 0.95F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 6458135, 0.8F) {}

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
		

		public BambooJungle() : base(24, "bamboo_jungle", "Bamboo Jungle", 9, 0.95F, "rain", 0.1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 7769620, 0.9F) {}

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
		

		public Badlands() : base(25, "badlands", "Badlands", 10, 2F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 14238997, 0F) {}

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
		

		public ErodedBadlands() : base(26, "eroded_badlands", "Eroded Badlands", 10, 2F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 16739645, 0F) {}

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
		

		public WoodedBadlands() : base(27, "wooded_badlands", "Wooded Badlands", 10, 2F, "none", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 11573093, 0F) {}

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
		

		public Meadow() : base(28, "meadow", "Meadow", 11, 0.5F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 9217136, 0.8F) {}

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
		

		public Grove() : base(29, "grove", "Grove", 5, -0.2F, "snow", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 14675173, 0.8F) {}

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
		

		public SnowySlopes() : base(30, "snowy_slopes", "Snowy Slopes", 11, -0.3F, "snow", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 14348785, 0.9F) {}

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
		

		public FrozenPeaks() : base(31, "frozen_peaks", "Frozen Peaks", 11, -0.7F, "snow", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 15399931, 0.9F) {}

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
		

		public JaggedPeaks() : base(32, "jagged_peaks", "Jagged Peaks", 11, -0.7F, "snow", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 14937325, 0.9F) {}

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
		

		public StonyPeaks() : base(33, "stony_peaks", "Stony Peaks", 11, 1F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 13750737, 0.3F) {}

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
		

		public River() : base(34, "river", "River", 12, 0.5F, "rain", -0.5F, MineSharp.Core.Types.Enums.Dimension.Overworld, 255, 0.5F) {}

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
		

		public FrozenRiver() : base(35, "frozen_river", "Frozen River", 12, 0F, "snow", -0.5F, MineSharp.Core.Types.Enums.Dimension.Overworld, 10526975, 0.5F) {}

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
		

		public Beach() : base(36, "beach", "Beach", 13, 0.8F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 16440917, 0.4F) {}

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
		

		public SnowyBeach() : base(37, "snowy_beach", "Snowy Beach", 13, 0.05F, "snow", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 16445632, 0.3F) {}

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
		

		public StonyShore() : base(38, "stony_shore", "Stony Shore", 13, 0.2F, "rain", 0F, MineSharp.Core.Types.Enums.Dimension.Overworld, 10658436, 0.3F) {}

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
		

		public WarmOcean() : base(39, "warm_ocean", "Warm Ocean", 14, 0.5F, "rain", -1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 172, 0.5F) {}

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
		

		public LukewarmOcean() : base(40, "lukewarm_ocean", "Lukewarm Ocean", 14, 0.5F, "rain", -1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 144, 0.5F) {}

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
		

		public DeepLukewarmOcean() : base(41, "deep_lukewarm_ocean", "Deep Lukewarm Ocean", 14, 0.5F, "rain", -1.8F, MineSharp.Core.Types.Enums.Dimension.Overworld, 64, 0.5F) {}

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
		

		public Ocean() : base(42, "ocean", "Ocean", 14, 0.5F, "rain", -1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 112, 0.5F) {}

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
		

		public DeepOcean() : base(43, "deep_ocean", "Deep Ocean", 14, 0.5F, "rain", -1.8F, MineSharp.Core.Types.Enums.Dimension.Overworld, 48, 0.5F) {}

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
		

		public ColdOcean() : base(44, "cold_ocean", "Cold Ocean", 14, 0.5F, "rain", -1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 2105456, 0.5F) {}

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
		

		public DeepColdOcean() : base(45, "deep_cold_ocean", "Deep Cold Ocean", 14, 0.5F, "rain", -1.8F, MineSharp.Core.Types.Enums.Dimension.Overworld, 2105400, 0.5F) {}

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
		

		public FrozenOcean() : base(46, "frozen_ocean", "Frozen Ocean", 14, 0F, "snow", -1F, MineSharp.Core.Types.Enums.Dimension.Overworld, 7368918, 0.5F) {}

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
		

		public DeepFrozenOcean() : base(47, "deep_frozen_ocean", "Deep Frozen Ocean", 14, 0.5F, "rain", -1.8F, MineSharp.Core.Types.Enums.Dimension.Overworld, 4210832, 0.5F) {}

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
		

		public MushroomFields() : base(48, "mushroom_fields", "Mushroom Fields", 15, 0.9F, "rain", 0.2F, MineSharp.Core.Types.Enums.Dimension.Overworld, 16711935, 1F) {}

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
		

		public DripstoneCaves() : base(49, "dripstone_caves", "Dripstone Caves", 16, 0.8F, "rain", 0.125F, MineSharp.Core.Types.Enums.Dimension.Overworld, 12690831, 0.4F) {}

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
		

		public LushCaves() : base(50, "lush_caves", "Lush Caves", 16, 0.5F, "rain", 0.5F, MineSharp.Core.Types.Enums.Dimension.Overworld, 14652980, 0.5F) {}

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
		

		public NetherWastes() : base(51, "nether_wastes", "Nether Wastes", 17, 2F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.Nether, 12532539, 0F) {}

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
		

		public WarpedForest() : base(52, "warped_forest", "Warped Forest", 17, 2F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.Nether, 4821115, 0F) {}

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
		

		public CrimsonForest() : base(53, "crimson_forest", "Crimson Forest", 17, 2F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.Nether, 14485512, 0F) {}

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
		

		public SoulSandValley() : base(54, "soul_sand_valley", "Soul Sand Valley", 17, 2F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.Nether, 6174768, 0F) {}

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
		

		public BasaltDeltas() : base(55, "basalt_deltas", "Basalt Deltas", 17, 2F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.Nether, 4208182, 0F) {}

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
		

		public TheEnd() : base(56, "the_end", "The End", 18, 0.5F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.End, 8421631, 0.5F) {}

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
		

		public EndHighlands() : base(57, "end_highlands", "End Highlands", 18, 0.5F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.End, 12828041, 0.5F) {}

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
		

		public EndMidlands() : base(58, "end_midlands", "End Midlands", 18, 0.5F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.End, 15464630, 0.5F) {}

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
		

		public SmallEndIslands() : base(59, "small_end_islands", "Small End Islands", 18, 0.5F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.End, 42, 0.5F) {}

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
		

		public EndBarrens() : base(60, "end_barrens", "End Barrens", 18, 0.5F, "none", 0.1F, MineSharp.Core.Types.Enums.Dimension.End, 9474162, 0.5F) {}

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

