



namespace MineSharp.Data.Enchantments {

    public static class EnchantmentData {

        private static bool isLoaded = false;

        public static Dictionary<EnchantmentType, EnchantmentInfo> Enchantments = new Dictionary<EnchantmentType, EnchantmentInfo>();

        public static void Load() {

            if (isLoaded) return;

            Register(EnchantmentType.Protection, "protection", "Protection", 4, new EnchantCost(11, -10), new EnchantCost(11, 1), false, false, new string[] { "fire_protection", "blast_protection", "projectile_protection" }, EnchantmentCategory.Armor, 10, true);
            Register(EnchantmentType.FireProtection, "fire_protection", "Fire Protection", 4, new EnchantCost(8, 2), new EnchantCost(8, 10), false, false, new string[] { "protection", "blast_protection", "projectile_protection" }, EnchantmentCategory.Armor, 5, true);
            Register(EnchantmentType.FeatherFalling, "feather_falling", "Feather Falling", 4, new EnchantCost(6, -1), new EnchantCost(6, 5), false, false, new string[] { }, EnchantmentCategory.Armorfeet, 5, true);
            Register(EnchantmentType.BlastProtection, "blast_protection", "Blast Protection", 4, new EnchantCost(8, -3), new EnchantCost(8, 5), false, false, new string[] { "protection", "fire_protection", "projectile_protection" }, EnchantmentCategory.Armor, 2, true);
            Register(EnchantmentType.ProjectileProtection, "projectile_protection", "Projectile Protection", 4, new EnchantCost(6, -3), new EnchantCost(6, 3), false, false, new string[] { "protection", "fire_protection", "blast_protection" }, EnchantmentCategory.Armor, 5, true);
            Register(EnchantmentType.Respiration, "respiration", "Respiration", 3, new EnchantCost(10, 0), new EnchantCost(10, 30), false, false, new string[] { }, EnchantmentCategory.Armorhead, 2, true);
            Register(EnchantmentType.AquaAffinity, "aqua_affinity", "Aqua Affinity", 1, new EnchantCost(0, 1), new EnchantCost(0, 41), false, false, new string[] { }, EnchantmentCategory.Armorhead, 2, true);
            Register(EnchantmentType.Thorns, "thorns", "Thorns", 3, new EnchantCost(20, -10), new EnchantCost(10, 51), false, false, new string[] { }, EnchantmentCategory.Armorchest, 1, true);
            Register(EnchantmentType.DepthStrider, "depth_strider", "Depth Strider", 3, new EnchantCost(10, 0), new EnchantCost(10, 15), false, false, new string[] { "frost_walker" }, EnchantmentCategory.Armorfeet, 2, true);
            Register(EnchantmentType.FrostWalker, "frost_walker", "Frost Walker", 2, new EnchantCost(10, 0), new EnchantCost(10, 15), true, false, new string[] { "depth_strider" }, EnchantmentCategory.Armorfeet, 2, true);
            Register(EnchantmentType.CurseofBinding, "binding_curse", "Curse of Binding", 1, new EnchantCost(0, 25), new EnchantCost(0, 50), true, true, new string[] { }, EnchantmentCategory.Wearable, 1, true);
            Register(EnchantmentType.SoulSpeed, "soul_speed", "Soul Speed", 3, new EnchantCost(10, 0), new EnchantCost(10, 15), true, false, new string[] { }, EnchantmentCategory.Armorfeet, 1, false);
            Register(EnchantmentType.Sharpness, "sharpness", "Sharpness", 5, new EnchantCost(11, -10), new EnchantCost(11, 10), false, false, new string[] { "smite", "bane_of_arthropods" }, EnchantmentCategory.Weapon, 10, true);
            Register(EnchantmentType.Smite, "smite", "Smite", 5, new EnchantCost(8, -3), new EnchantCost(8, 17), false, false, new string[] { "sharpness", "bane_of_arthropods" }, EnchantmentCategory.Weapon, 5, true);
            Register(EnchantmentType.BaneofArthropods, "bane_of_arthropods", "Bane of Arthropods", 5, new EnchantCost(8, -3), new EnchantCost(8, 17), false, false, new string[] { "sharpness", "smite" }, EnchantmentCategory.Weapon, 5, true);
            Register(EnchantmentType.Knockback, "knockback", "Knockback", 2, new EnchantCost(20, -15), new EnchantCost(10, 51), false, false, new string[] { }, EnchantmentCategory.Weapon, 5, true);
            Register(EnchantmentType.FireAspect, "fire_aspect", "Fire Aspect", 2, new EnchantCost(20, -10), new EnchantCost(10, 51), false, false, new string[] { }, EnchantmentCategory.Weapon, 2, true);
            Register(EnchantmentType.Looting, "looting", "Looting", 3, new EnchantCost(9, 6), new EnchantCost(10, 51), false, false, new string[] { "silk_touch" }, EnchantmentCategory.Weapon, 2, true);
            Register(EnchantmentType.SweepingEdge, "sweeping", "Sweeping Edge", 3, new EnchantCost(9, -4), new EnchantCost(9, 11), false, false, new string[] { }, EnchantmentCategory.Weapon, 2, true);
            Register(EnchantmentType.Efficiency, "efficiency", "Efficiency", 5, new EnchantCost(10, -9), new EnchantCost(10, 51), false, false, new string[] { }, EnchantmentCategory.Digger, 10, true);
            Register(EnchantmentType.SilkTouch, "silk_touch", "Silk Touch", 1, new EnchantCost(0, 15), new EnchantCost(10, 51), false, false, new string[] { "looting", "fortune", "luck_of_the_sea" }, EnchantmentCategory.Digger, 1, true);
            Register(EnchantmentType.Unbreaking, "unbreaking", "Unbreaking", 3, new EnchantCost(8, -3), new EnchantCost(10, 51), false, false, new string[] { }, EnchantmentCategory.Breakable, 5, true);
            Register(EnchantmentType.Fortune, "fortune", "Fortune", 3, new EnchantCost(9, 6), new EnchantCost(10, 51), false, false, new string[] { "silk_touch" }, EnchantmentCategory.Digger, 2, true);
            Register(EnchantmentType.Power, "power", "Power", 5, new EnchantCost(10, -9), new EnchantCost(10, 6), false, false, new string[] { }, EnchantmentCategory.Bow, 10, true);
            Register(EnchantmentType.Punch, "punch", "Punch", 2, new EnchantCost(20, -8), new EnchantCost(20, 17), false, false, new string[] { }, EnchantmentCategory.Bow, 2, true);
            Register(EnchantmentType.Flame, "flame", "Flame", 1, new EnchantCost(0, 20), new EnchantCost(0, 50), false, false, new string[] { }, EnchantmentCategory.Bow, 2, true);
            Register(EnchantmentType.Infinity, "infinity", "Infinity", 1, new EnchantCost(0, 20), new EnchantCost(0, 50), false, false, new string[] { "mending" }, EnchantmentCategory.Bow, 1, true);
            Register(EnchantmentType.LuckoftheSea, "luck_of_the_sea", "Luck of the Sea", 3, new EnchantCost(9, 6), new EnchantCost(10, 51), false, false, new string[] { "silk_touch" }, EnchantmentCategory.Fishingrod, 2, true);
            Register(EnchantmentType.Lure, "lure", "Lure", 3, new EnchantCost(9, 6), new EnchantCost(10, 51), false, false, new string[] { }, EnchantmentCategory.Fishingrod, 2, true);
            Register(EnchantmentType.Loyalty, "loyalty", "Loyalty", 3, new EnchantCost(7, 5), new EnchantCost(0, 50), false, false, new string[] { "riptide" }, EnchantmentCategory.Trident, 5, true);
            Register(EnchantmentType.Impaling, "impaling", "Impaling", 5, new EnchantCost(8, -7), new EnchantCost(8, 13), false, false, new string[] { }, EnchantmentCategory.Trident, 2, true);
            Register(EnchantmentType.Riptide, "riptide", "Riptide", 3, new EnchantCost(7, 10), new EnchantCost(0, 50), false, false, new string[] { "loyalty", "channeling" }, EnchantmentCategory.Trident, 2, true);
            Register(EnchantmentType.Channeling, "channeling", "Channeling", 1, new EnchantCost(0, 25), new EnchantCost(0, 50), false, false, new string[] { "riptide" }, EnchantmentCategory.Trident, 1, true);
            Register(EnchantmentType.Multishot, "multishot", "Multishot", 1, new EnchantCost(0, 20), new EnchantCost(0, 50), false, false, new string[] { "piercing" }, EnchantmentCategory.Crossbow, 2, true);
            Register(EnchantmentType.QuickCharge, "quick_charge", "Quick Charge", 3, new EnchantCost(20, -8), new EnchantCost(0, 50), false, false, new string[] { }, EnchantmentCategory.Crossbow, 5, true);
            Register(EnchantmentType.Piercing, "piercing", "Piercing", 4, new EnchantCost(10, -9), new EnchantCost(0, 50), false, false, new string[] { "multishot" }, EnchantmentCategory.Crossbow, 10, true);
            Register(EnchantmentType.Mending, "mending", "Mending", 1, new EnchantCost(25, 0), new EnchantCost(25, 50), true, false, new string[] { "infinity" }, EnchantmentCategory.Breakable, 2, true);
            Register(EnchantmentType.CurseofVanishing, "vanishing_curse", "Curse of Vanishing", 1, new EnchantCost(0, 25), new EnchantCost(0, 50), true, true, new string[] { }, EnchantmentCategory.Vanishable, 1, true);

            isLoaded = true;
        }

        private static void Register(EnchantmentType type, string name, string displayName, int maxLevel, EnchantCost minCost, EnchantCost maxCost, bool treasureOnly, bool curse, string[] exclude, EnchantmentCategory category, int weight, bool discoverable) {
            EnchantmentInfo info = new EnchantmentInfo(type, name, displayName, maxLevel, minCost, maxCost, treasureOnly, curse, exclude, category, weight, discoverable);
            Enchantments.Add(type, info);
        }

    }
}

