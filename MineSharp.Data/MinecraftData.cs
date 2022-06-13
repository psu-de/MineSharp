using MineSharp.Data.Biomes;
using MineSharp.Data.Blocks;
using MineSharp.Data.Effects;
using MineSharp.Data.Enchantments;
using MineSharp.Data.Items;
using MineSharp.Data.Windows;

namespace MineSharp.Data {
    public static class MinecraftData {

        public static bool IsLoaded = false;

        public static void Load() {
            if (IsLoaded) return;

            BlockData.Load();
            BiomeData.Load();
            ItemData.Load();
            EffectData.Load();
            EnchantmentData.Load();
            WindowData.Load();

            IsLoaded = true;
        }

    }
}
