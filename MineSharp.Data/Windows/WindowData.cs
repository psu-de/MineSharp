using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Windows {
    public static class WindowData {

        private static bool isLoaded = false;

        public static List<WindowInfo> Windows = new List<WindowInfo>();

        public static void Load() {
            if (isLoaded) return;

            Register(new WindowInfo("minecraft:generic_9x1", "",  9));
            Register(new WindowInfo("minecraft:generic_9x2", "", 18));
            Register(new WindowInfo("minecraft:generic_9x3", "", 27));
            Register(new WindowInfo("minecraft:generic_9x4", "", 36));
            Register(new WindowInfo("minecraft:generic_9x5", "", 45));
            Register(new WindowInfo("minecraft:generic_9x6", "", 54));
            Register(new WindowInfo("minecraft:generic_3x3", "",  9));
            Register(new WindowInfo("minecraft:anvil", "Anvil", 3));
            Register(new WindowInfo("minecraft:beacon", "Beacon", 1));
            Register(new WindowInfo("minecraft:blast_furnace", "Blast Furnace", 3));
            Register(new WindowInfo("minecraft:brewing_stand", "Brewing stand", 5));
            Register(new WindowInfo("minecraft:crafting", "Crafting table", 10));
            Register(new WindowInfo("minecraft:enchantment", "Enchantment table", 2));
            Register(new WindowInfo("minecraft:furnace", "Furnace", 3));
            Register(new WindowInfo("minecraft:grindstone", "Grindstone", 3));
            Register(new WindowInfo("minecraft:hopper", "Hopper or minecart with hopper", 5));
            Register(new WindowInfo("minecraft:lectern", "Lectern", 1, excludeInventory: true));
            Register(new WindowInfo("minecraft:loom", "Loom", 4));
            Register(new WindowInfo("minecraft:merchant", "Villager, Wandering Trader", 3));
            Register(new WindowInfo("minecraft:shulker_box", "Shulker box", 27));
            Register(new WindowInfo("minecraft:smithing", "Smithing Table", 3));
            Register(new WindowInfo("minecraft:smoker", "Smoker", 3));
            Register(new WindowInfo("minecraft:cartography", "Cartography Table", 3));
            Register(new WindowInfo("minecraft:stonecutter", "Stonecutter", 2));

            isLoaded = true;
        }

        private static void Register(WindowInfo info) {
            Windows.Add(info);
        }
    }
}
