using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.T4.Windows {
    public static class WindowData {

        private static bool isLoaded = false;

        public static List<WindowInfo> Windows = new List<WindowInfo>();
        public static Dictionary<Identifier, WindowInfo> WindowMap = new Dictionary<Identifier, WindowInfo>();

        public static void Load() {
            if (isLoaded) return;

            Register(new WindowInfo((Identifier)"minecraft:generic_9x1", "",  9));
            Register(new WindowInfo((Identifier)"minecraft:generic_9x2", "", 18));
            Register(new WindowInfo((Identifier)"minecraft:generic_9x3", "", 27));
            Register(new WindowInfo((Identifier)"minecraft:generic_9x4", "", 36));
            Register(new WindowInfo((Identifier)"minecraft:generic_9x5", "", 45));
            Register(new WindowInfo((Identifier)"minecraft:generic_9x6", "", 54));
            Register(new WindowInfo((Identifier)"minecraft:generic_3x3", "",  9));
            Register(new WindowInfo((Identifier)"minecraft:anvil", "Anvil", 3));
            Register(new WindowInfo((Identifier)"minecraft:beacon", "Beacon", 1));
            Register(new WindowInfo((Identifier)"minecraft:blast_furnace", "Blast Furnace", 3));
            Register(new WindowInfo((Identifier)"minecraft:brewing_stand", "Brewing stand", 5));
            Register(new WindowInfo((Identifier)"minecraft:crafting", "Crafting table", 10));
            Register(new WindowInfo((Identifier)"minecraft:enchantment", "Enchantment table", 2));
            Register(new WindowInfo((Identifier)"minecraft:furnace", "Furnace", 3));
            Register(new WindowInfo((Identifier)"minecraft:grindstone", "Grindstone", 3));
            Register(new WindowInfo((Identifier)"minecraft:hopper", "Hopper or minecart with hopper", 5));
            Register(new WindowInfo((Identifier)"minecraft:lectern", "Lectern", 1, excludeInventory: true));
            Register(new WindowInfo((Identifier)"minecraft:loom", "Loom", 4));
            Register(new WindowInfo((Identifier)"minecraft:merchant", "Villager, Wandering Trader", 3));
            Register(new WindowInfo((Identifier)"minecraft:shulker_box", "Shulker box", 27));
            Register(new WindowInfo((Identifier)"minecraft:smithing", "Smithing Table", 3));
            Register(new WindowInfo((Identifier)"minecraft:smoker", "Smoker", 3));
            Register(new WindowInfo((Identifier)"minecraft:cartography", "Cartography Table", 3));
            Register(new WindowInfo((Identifier)"minecraft:stonecutter", "Stonecutter", 2));

            isLoaded = true;
        }

        private static void Register(WindowInfo info) {
            WindowMap.Add(info.Name, info);
            Windows.Add(info);
        }
    }
}
