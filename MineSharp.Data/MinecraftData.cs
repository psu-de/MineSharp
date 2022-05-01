using MineSharp.Data.Biomes;
using MineSharp.Data.Blocks;
using MineSharp.Data.Effects;
using MineSharp.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data {
    public static class MinecraftData {

        public static bool IsLoaded = false;

        public static void Load() {
            if (IsLoaded) return;

            BlockData.Load();
            BiomeData.Load();
            ItemData.Load();
            EffectData.Load();

            IsLoaded = true;
        }

    }
}
