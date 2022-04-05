



using MineSharp.Core.Types.Enums;

namespace MineSharp.Data.Biomes {

    public static class BiomeData {

        private static bool isLoaded = false;

        public static List<BiomeInfo> Biomes = new List<BiomeInfo>();
        public static Dictionary<BiomeType, BiomeInfo> BiomeMap = new Dictionary<BiomeType, BiomeInfo>();

        public static void Load() {

            if (isLoaded) return;

            RegisterBiome(new BiomeInfo(0, "the_void", BiomeType.TheVoid, BiomeCategory.None, 0.5f, "none", 0.1f, Dimension.Overworld, "The Void", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(1, "plains", BiomeType.Plains, BiomeCategory.Plains, 0.8f, "rain", 0.125f, Dimension.Overworld, "Plains", 7907327, 0.4f));
            RegisterBiome(new BiomeInfo(2, "sunflower_plains", BiomeType.SunflowerPlains, BiomeCategory.Plains, 0.8f, "rain", 0.125f, Dimension.Overworld, "Sunflower Plains", 7907327, 0.4f));
            RegisterBiome(new BiomeInfo(3, "snowy_plains", BiomeType.SnowyPlains, BiomeCategory.Icy, 0f, "snow", 0f, Dimension.Overworld, "Snowy Plains", 8364543, 0.5f));
            RegisterBiome(new BiomeInfo(4, "ice_spikes", BiomeType.IceSpikes, BiomeCategory.Icy, 0f, "snow", 0.425f, Dimension.Overworld, "Ice Spikes", 8364543, 0.5f));
            RegisterBiome(new BiomeInfo(5, "desert", BiomeType.Desert, BiomeCategory.Desert, 2f, "none", 0.125f, Dimension.Overworld, "Desert", 7254527, 0f));
            RegisterBiome(new BiomeInfo(6, "swamp", BiomeType.Swamp, BiomeCategory.Swamp, 0.8f, "rain", -0.2f, Dimension.Overworld, "Swamp", 7907327, 0.9f));
            RegisterBiome(new BiomeInfo(7, "forest", BiomeType.Forest, BiomeCategory.Forest, 0.7f, "rain", 0.1f, Dimension.Overworld, "Forest", 7972607, 0.8f));
            RegisterBiome(new BiomeInfo(8, "flower_forest", BiomeType.FlowerForest, BiomeCategory.Forest, 0.7f, "rain", 0.1f, Dimension.Overworld, "Flower Forest", 7972607, 0.8f));
            RegisterBiome(new BiomeInfo(9, "birch_forest", BiomeType.BirchForest, BiomeCategory.Forest, 0.6f, "rain", 0.1f, Dimension.Overworld, "Birch Forest", 8037887, 0.6f));
            RegisterBiome(new BiomeInfo(10, "dark_forest", BiomeType.DarkForest, BiomeCategory.Forest, 0.7f, "rain", 0.1f, Dimension.Overworld, "Dark Forest", 7972607, 0.8f));
            RegisterBiome(new BiomeInfo(11, "old_growth_birch_forest", BiomeType.OldGrowthBirchForest, BiomeCategory.Forest, 0.6f, "rain", 0f, Dimension.Overworld, "Old Growth Birch Forest", 8037887, 0.6f));
            RegisterBiome(new BiomeInfo(12, "old_growth_pine_taiga", BiomeType.OldGrowthPineTaiga, BiomeCategory.Taiga, 0.3f, "rain", 0f, Dimension.Overworld, "Old Growth Pine Taiga", 8168447, 0.8f));
            RegisterBiome(new BiomeInfo(13, "old_growth_spruce_taiga", BiomeType.OldGrowthSpruceTaiga, BiomeCategory.Taiga, 0.25f, "rain", 0f, Dimension.Overworld, "Old Growth Spruce Taiga", 8233983, 0.8f));
            RegisterBiome(new BiomeInfo(14, "taiga", BiomeType.Taiga, BiomeCategory.Taiga, 0.25f, "rain", 0.2f, Dimension.Overworld, "Taiga", 8233983, 0.8f));
            RegisterBiome(new BiomeInfo(15, "snowy_taiga", BiomeType.SnowyTaiga, BiomeCategory.Taiga, -0.5f, "snow", 0.2f, Dimension.Overworld, "Snowy Taiga", 8625919, 0.4f));
            RegisterBiome(new BiomeInfo(16, "savanna", BiomeType.Savanna, BiomeCategory.Savanna, 2f, "none", 0.125f, Dimension.Overworld, "Savanna", 7254527, 0f));
            RegisterBiome(new BiomeInfo(17, "savanna_plateau", BiomeType.SavannaPlateau, BiomeCategory.Savanna, 2f, "none", 1.5f, Dimension.Overworld, "Savanna Plateau", 7254527, 0f));
            RegisterBiome(new BiomeInfo(18, "windswept_hills", BiomeType.WindsweptHills, BiomeCategory.Extremehills, 0.2f, "rain", 0f, Dimension.Overworld, "Windswept Hills", 8233727, 0.3f));
            RegisterBiome(new BiomeInfo(19, "windswept_gravelly_hills", BiomeType.WindsweptGravellyHills, BiomeCategory.Extremehills, 0.2f, "rain", 0f, Dimension.Overworld, "Windswept Gravelly Hills", 8233727, 0.3f));
            RegisterBiome(new BiomeInfo(20, "windswept_forest", BiomeType.WindsweptForest, BiomeCategory.Extremehills, 0.2f, "rain", 0f, Dimension.Overworld, "Windswept Forest", 8233727, 0.3f));
            RegisterBiome(new BiomeInfo(21, "windswept_savanna", BiomeType.WindsweptSavanna, BiomeCategory.Savanna, 2f, "none", 0f, Dimension.Overworld, "Windswept Savanna", 7254527, 0f));
            RegisterBiome(new BiomeInfo(22, "jungle", BiomeType.Jungle, BiomeCategory.Jungle, 0.95f, "rain", 0.1f, Dimension.Overworld, "Jungle", 7842047, 0.9f));
            RegisterBiome(new BiomeInfo(23, "sparse_jungle", BiomeType.SparseJungle, BiomeCategory.Jungle, 0.95f, "rain", 0f, Dimension.Overworld, "Sparse Jungle", 7842047, 0.8f));
            RegisterBiome(new BiomeInfo(24, "bamboo_jungle", BiomeType.BambooJungle, BiomeCategory.Jungle, 0.95f, "rain", 0.1f, Dimension.Overworld, "Bamboo Jungle", 7842047, 0.9f));
            RegisterBiome(new BiomeInfo(25, "badlands", BiomeType.Badlands, BiomeCategory.Mesa, 2f, "none", 0.1f, Dimension.Overworld, "Badlands", 7254527, 0f));
            RegisterBiome(new BiomeInfo(26, "eroded_badlands", BiomeType.ErodedBadlands, BiomeCategory.Mesa, 2f, "none", 0.1f, Dimension.Overworld, "Eroded Badlands", 7254527, 0f));
            RegisterBiome(new BiomeInfo(27, "wooded_badlands", BiomeType.WoodedBadlands, BiomeCategory.Mesa, 2f, "none", 0f, Dimension.Overworld, "Wooded Badlands", 7254527, 0f));
            RegisterBiome(new BiomeInfo(28, "meadow", BiomeType.Meadow, BiomeCategory.Mountain, 0.5f, "rain", 0f, Dimension.Overworld, "Meadow", 8103167, 0.8f));
            RegisterBiome(new BiomeInfo(29, "grove", BiomeType.Grove, BiomeCategory.Forest, -0.2f, "snow", 0f, Dimension.Overworld, "Grove", 8495359, 0.8f));
            RegisterBiome(new BiomeInfo(30, "snowy_slopes", BiomeType.SnowySlopes, BiomeCategory.Mountain, -0.3f, "snow", 0f, Dimension.Overworld, "Snowy Slopes", 8560639, 0.9f));
            RegisterBiome(new BiomeInfo(31, "frozen_peaks", BiomeType.FrozenPeaks, BiomeCategory.Mountain, -0.7f, "snow", 0f, Dimension.Overworld, "Frozen Peaks", 8756735, 0.9f));
            RegisterBiome(new BiomeInfo(32, "jagged_peaks", BiomeType.JaggedPeaks, BiomeCategory.Mountain, -0.7f, "snow", 0f, Dimension.Overworld, "Jagged Peaks", 8756735, 0.9f));
            RegisterBiome(new BiomeInfo(33, "stony_peaks", BiomeType.StonyPeaks, BiomeCategory.Mountain, 1f, "rain", 0f, Dimension.Overworld, "Stony Peaks", 7776511, 0.3f));
            RegisterBiome(new BiomeInfo(34, "river", BiomeType.River, BiomeCategory.River, 0.5f, "rain", -0.5f, Dimension.Overworld, "River", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(35, "frozen_river", BiomeType.FrozenRiver, BiomeCategory.River, 0f, "snow", -0.5f, Dimension.Overworld, "Frozen River", 8364543, 0.5f));
            RegisterBiome(new BiomeInfo(36, "beach", BiomeType.Beach, BiomeCategory.Beach, 0.8f, "rain", 0f, Dimension.Overworld, "Beach", 7907327, 0.4f));
            RegisterBiome(new BiomeInfo(37, "snowy_beach", BiomeType.SnowyBeach, BiomeCategory.Beach, 0.05f, "snow", 0f, Dimension.Overworld, "Snowy Beach", 8364543, 0.3f));
            RegisterBiome(new BiomeInfo(38, "stony_shore", BiomeType.StonyShore, BiomeCategory.Beach, 0.2f, "rain", 0f, Dimension.Overworld, "Stony Shore", 8233727, 0.3f));
            RegisterBiome(new BiomeInfo(39, "warm_ocean", BiomeType.WarmOcean, BiomeCategory.Ocean, 0.5f, "rain", -1f, Dimension.Overworld, "Warm Ocean", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(40, "lukewarm_ocean", BiomeType.LukewarmOcean, BiomeCategory.Ocean, 0.5f, "rain", -1f, Dimension.Overworld, "Lukewarm Ocean", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(41, "deep_lukewarm_ocean", BiomeType.DeepLukewarmOcean, BiomeCategory.Ocean, 0.5f, "rain", -1.8f, Dimension.Overworld, "Deep Lukewarm Ocean", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(42, "ocean", BiomeType.Ocean, BiomeCategory.Ocean, 0.5f, "rain", -1f, Dimension.Overworld, "Ocean", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(43, "deep_ocean", BiomeType.DeepOcean, BiomeCategory.Ocean, 0.5f, "rain", -1.8f, Dimension.Overworld, "Deep Ocean", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(44, "cold_ocean", BiomeType.ColdOcean, BiomeCategory.Ocean, 0.5f, "rain", -1f, Dimension.Overworld, "Cold Ocean", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(45, "deep_cold_ocean", BiomeType.DeepColdOcean, BiomeCategory.Ocean, 0.5f, "rain", -1.8f, Dimension.Overworld, "Deep Cold Ocean", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(46, "frozen_ocean", BiomeType.FrozenOcean, BiomeCategory.Ocean, 0f, "snow", -1f, Dimension.Overworld, "Frozen Ocean", 8364543, 0.5f));
            RegisterBiome(new BiomeInfo(47, "deep_frozen_ocean", BiomeType.DeepFrozenOcean, BiomeCategory.Ocean, 0.5f, "rain", -1.8f, Dimension.Overworld, "Deep Frozen Ocean", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(48, "mushroom_fields", BiomeType.MushroomFields, BiomeCategory.Mushroom, 0.9f, "rain", 0.2f, Dimension.Overworld, "Mushroom Fields", 7842047, 1f));
            RegisterBiome(new BiomeInfo(49, "dripstone_caves", BiomeType.DripstoneCaves, BiomeCategory.Underground, 0.8f, "rain", 0.125f, Dimension.Overworld, "Dripstone Caves", 7907327, 0.4f));
            RegisterBiome(new BiomeInfo(50, "lush_caves", BiomeType.LushCaves, BiomeCategory.Underground, 0.5f, "rain", 0.5f, Dimension.Overworld, "Lush Caves", 8103167, 0.5f));
            RegisterBiome(new BiomeInfo(51, "nether_wastes", BiomeType.NetherWastes, BiomeCategory.Nether, 2f, "none", 0.1f, Dimension.Nether, "Nether Wastes", 7254527, 0f));
            RegisterBiome(new BiomeInfo(52, "warped_forest", BiomeType.WarpedForest, BiomeCategory.Nether, 2f, "none", 0.1f, Dimension.Nether, "Warped Forest", 7254527, 0f));
            RegisterBiome(new BiomeInfo(53, "crimson_forest", BiomeType.CrimsonForest, BiomeCategory.Nether, 2f, "none", 0.1f, Dimension.Nether, "Crimson Forest", 7254527, 0f));
            RegisterBiome(new BiomeInfo(54, "soul_sand_valley", BiomeType.SoulSandValley, BiomeCategory.Nether, 2f, "none", 0.1f, Dimension.Nether, "Soul Sand Valley", 7254527, 0f));
            RegisterBiome(new BiomeInfo(55, "basalt_deltas", BiomeType.BasaltDeltas, BiomeCategory.Nether, 2f, "none", 0.1f, Dimension.Nether, "Basalt Deltas", 7254527, 0f));
            RegisterBiome(new BiomeInfo(56, "the_end", BiomeType.TheEnd, BiomeCategory.Theend, 0.5f, "none", 0.1f, Dimension.End, "The End", 0, 0.5f));
            RegisterBiome(new BiomeInfo(57, "end_highlands", BiomeType.EndHighlands, BiomeCategory.Theend, 0.5f, "none", 0.1f, Dimension.End, "End Highlands", 0, 0.5f));
            RegisterBiome(new BiomeInfo(58, "end_midlands", BiomeType.EndMidlands, BiomeCategory.Theend, 0.5f, "none", 0.1f, Dimension.End, "End Midlands", 0, 0.5f));
            RegisterBiome(new BiomeInfo(59, "small_end_islands", BiomeType.SmallEndIslands, BiomeCategory.Theend, 0.5f, "none", 0.1f, Dimension.End, "Small End Islands", 0, 0.5f));
            RegisterBiome(new BiomeInfo(60, "end_barrens", BiomeType.EndBarrens, BiomeCategory.Theend, 0.5f, "none", 0.1f, Dimension.End, "End Barrens", 0, 0.5f));
            
            isLoaded = true;
        }

        private static void RegisterBiome(BiomeInfo info) {
            Biomes.Add(info);
        }

    }
}

