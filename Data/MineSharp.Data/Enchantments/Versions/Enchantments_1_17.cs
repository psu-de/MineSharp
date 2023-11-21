using MineSharp.Core.Common;
using MineSharp.Core.Common.Enchantments;

namespace MineSharp.Data.Enchantments.Versions;

internal class Enchantments_1_17 : DataVersion<EnchantmentType, EnchantmentInfo>
{
    private static Dictionary<EnchantmentType, EnchantmentInfo> Values { get; } = new Dictionary<EnchantmentType, EnchantmentInfo>()
    {
        { EnchantmentType.Protection, new EnchantmentInfo(0, "protection", "Protection", 4, new EnchantCost(11, -10), new EnchantCost(11, 1), false, false, new [] { EnchantmentType.FireProtection, EnchantmentType.BlastProtection, EnchantmentType.ProjectileProtection }, EnchantmentCategory.Armor, 10, true, true) },
        { EnchantmentType.FireProtection, new EnchantmentInfo(1, "fire_protection", "Fire Protection", 4, new EnchantCost(8, 2), new EnchantCost(8, 10), false, false, new [] { EnchantmentType.Protection, EnchantmentType.BlastProtection, EnchantmentType.ProjectileProtection }, EnchantmentCategory.Armor, 5, true, true) },
        { EnchantmentType.FeatherFalling, new EnchantmentInfo(2, "feather_falling", "Feather Falling", 4, new EnchantCost(6, -1), new EnchantCost(6, 5), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.ArmorFeet, 5, true, true) },
        { EnchantmentType.BlastProtection, new EnchantmentInfo(3, "blast_protection", "Blast Protection", 4, new EnchantCost(8, -3), new EnchantCost(8, 5), false, false, new [] { EnchantmentType.Protection, EnchantmentType.FireProtection, EnchantmentType.ProjectileProtection }, EnchantmentCategory.Armor, 2, true, true) },
        { EnchantmentType.ProjectileProtection, new EnchantmentInfo(4, "projectile_protection", "Projectile Protection", 4, new EnchantCost(6, -3), new EnchantCost(6, 3), false, false, new [] { EnchantmentType.Protection, EnchantmentType.FireProtection, EnchantmentType.BlastProtection }, EnchantmentCategory.Armor, 5, true, true) },
        { EnchantmentType.Respiration, new EnchantmentInfo(5, "respiration", "Respiration", 3, new EnchantCost(10, 0), new EnchantCost(10, 30), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.ArmorHead, 2, true, true) },
        { EnchantmentType.AquaAffinity, new EnchantmentInfo(6, "aqua_affinity", "Aqua Affinity", 1, new EnchantCost(0, 1), new EnchantCost(0, 41), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.ArmorHead, 2, true, true) },
        { EnchantmentType.Thorns, new EnchantmentInfo(7, "thorns", "Thorns", 3, new EnchantCost(20, -10), new EnchantCost(10, 51), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.ArmorChest, 1, true, true) },
        { EnchantmentType.DepthStrider, new EnchantmentInfo(8, "depth_strider", "Depth Strider", 3, new EnchantCost(10, 0), new EnchantCost(10, 15), false, false, new [] { EnchantmentType.FrostWalker }, EnchantmentCategory.ArmorFeet, 2, true, true) },
        { EnchantmentType.FrostWalker, new EnchantmentInfo(9, "frost_walker", "Frost Walker", 2, new EnchantCost(10, 0), new EnchantCost(10, 15), true, false, new [] { EnchantmentType.DepthStrider }, EnchantmentCategory.ArmorFeet, 2, true, true) },
        { EnchantmentType.BindingCurse, new EnchantmentInfo(10, "binding_curse", "Curse of Binding", 1, new EnchantCost(0, 25), new EnchantCost(0, 50), true, true, Array.Empty<EnchantmentType>(), EnchantmentCategory.Wearable, 1, true, true) },
        { EnchantmentType.SoulSpeed, new EnchantmentInfo(11, "soul_speed", "Soul Speed", 3, new EnchantCost(10, 0), new EnchantCost(10, 15), true, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.ArmorFeet, 1, false, false) },
        { EnchantmentType.Sharpness, new EnchantmentInfo(12, "sharpness", "Sharpness", 5, new EnchantCost(11, -10), new EnchantCost(11, 10), false, false, new [] { EnchantmentType.Smite, EnchantmentType.BaneOfArthropods }, EnchantmentCategory.Weapon, 10, true, true) },
        { EnchantmentType.Smite, new EnchantmentInfo(13, "smite", "Smite", 5, new EnchantCost(8, -3), new EnchantCost(8, 17), false, false, new [] { EnchantmentType.Sharpness, EnchantmentType.BaneOfArthropods }, EnchantmentCategory.Weapon, 5, true, true) },
        { EnchantmentType.BaneOfArthropods, new EnchantmentInfo(14, "bane_of_arthropods", "Bane of Arthropods", 5, new EnchantCost(8, -3), new EnchantCost(8, 17), false, false, new [] { EnchantmentType.Sharpness, EnchantmentType.Smite }, EnchantmentCategory.Weapon, 5, true, true) },
        { EnchantmentType.Knockback, new EnchantmentInfo(15, "knockback", "Knockback", 2, new EnchantCost(20, -15), new EnchantCost(10, 51), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Weapon, 5, true, true) },
        { EnchantmentType.FireAspect, new EnchantmentInfo(16, "fire_aspect", "Fire Aspect", 2, new EnchantCost(20, -10), new EnchantCost(10, 51), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Weapon, 2, true, true) },
        { EnchantmentType.Looting, new EnchantmentInfo(17, "looting", "Looting", 3, new EnchantCost(9, 6), new EnchantCost(10, 51), false, false, new [] { EnchantmentType.SilkTouch }, EnchantmentCategory.Weapon, 2, true, true) },
        { EnchantmentType.Sweeping, new EnchantmentInfo(18, "sweeping", "Sweeping Edge", 3, new EnchantCost(9, -4), new EnchantCost(9, 11), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Weapon, 2, true, true) },
        { EnchantmentType.Efficiency, new EnchantmentInfo(19, "efficiency", "Efficiency", 5, new EnchantCost(10, -9), new EnchantCost(10, 51), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Digger, 10, true, true) },
        { EnchantmentType.SilkTouch, new EnchantmentInfo(20, "silk_touch", "Silk Touch", 1, new EnchantCost(0, 15), new EnchantCost(10, 51), false, false, new [] { EnchantmentType.Looting, EnchantmentType.Fortune, EnchantmentType.LuckOfTheSea }, EnchantmentCategory.Digger, 1, true, true) },
        { EnchantmentType.Unbreaking, new EnchantmentInfo(21, "unbreaking", "Unbreaking", 3, new EnchantCost(8, -3), new EnchantCost(10, 51), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Breakable, 5, true, true) },
        { EnchantmentType.Fortune, new EnchantmentInfo(22, "fortune", "Fortune", 3, new EnchantCost(9, 6), new EnchantCost(10, 51), false, false, new [] { EnchantmentType.SilkTouch }, EnchantmentCategory.Digger, 2, true, true) },
        { EnchantmentType.Power, new EnchantmentInfo(23, "power", "Power", 5, new EnchantCost(10, -9), new EnchantCost(10, 6), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Bow, 10, true, true) },
        { EnchantmentType.Punch, new EnchantmentInfo(24, "punch", "Punch", 2, new EnchantCost(20, -8), new EnchantCost(20, 17), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Bow, 2, true, true) },
        { EnchantmentType.Flame, new EnchantmentInfo(25, "flame", "Flame", 1, new EnchantCost(0, 20), new EnchantCost(0, 50), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Bow, 2, true, true) },
        { EnchantmentType.Infinity, new EnchantmentInfo(26, "infinity", "Infinity", 1, new EnchantCost(0, 20), new EnchantCost(0, 50), false, false, new [] { EnchantmentType.Mending }, EnchantmentCategory.Bow, 1, true, true) },
        { EnchantmentType.LuckOfTheSea, new EnchantmentInfo(27, "luck_of_the_sea", "Luck of the Sea", 3, new EnchantCost(9, 6), new EnchantCost(10, 51), false, false, new [] { EnchantmentType.SilkTouch }, EnchantmentCategory.FishingRod, 2, true, true) },
        { EnchantmentType.Lure, new EnchantmentInfo(28, "lure", "Lure", 3, new EnchantCost(9, 6), new EnchantCost(10, 51), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.FishingRod, 2, true, true) },
        { EnchantmentType.Loyalty, new EnchantmentInfo(29, "loyalty", "Loyalty", 3, new EnchantCost(7, 5), new EnchantCost(0, 50), false, false, new [] { EnchantmentType.Riptide }, EnchantmentCategory.Trident, 5, true, true) },
        { EnchantmentType.Impaling, new EnchantmentInfo(30, "impaling", "Impaling", 5, new EnchantCost(8, -7), new EnchantCost(8, 13), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Trident, 2, true, true) },
        { EnchantmentType.Riptide, new EnchantmentInfo(31, "riptide", "Riptide", 3, new EnchantCost(7, 10), new EnchantCost(0, 50), false, false, new [] { EnchantmentType.Loyalty, EnchantmentType.Channeling }, EnchantmentCategory.Trident, 2, true, true) },
        { EnchantmentType.Channeling, new EnchantmentInfo(32, "channeling", "Channeling", 1, new EnchantCost(0, 25), new EnchantCost(0, 50), false, false, new [] { EnchantmentType.Riptide }, EnchantmentCategory.Trident, 1, true, true) },
        { EnchantmentType.Multishot, new EnchantmentInfo(33, "multishot", "Multishot", 1, new EnchantCost(0, 20), new EnchantCost(0, 50), false, false, new [] { EnchantmentType.Piercing }, EnchantmentCategory.Crossbow, 2, true, true) },
        { EnchantmentType.QuickCharge, new EnchantmentInfo(34, "quick_charge", "Quick Charge", 3, new EnchantCost(20, -8), new EnchantCost(0, 50), false, false, Array.Empty<EnchantmentType>(), EnchantmentCategory.Crossbow, 5, true, true) },
        { EnchantmentType.Piercing, new EnchantmentInfo(35, "piercing", "Piercing", 4, new EnchantCost(10, -9), new EnchantCost(0, 50), false, false, new [] { EnchantmentType.Multishot }, EnchantmentCategory.Crossbow, 10, true, true) },
        { EnchantmentType.Mending, new EnchantmentInfo(36, "mending", "Mending", 1, new EnchantCost(25, 0), new EnchantCost(25, 50), true, false, new [] { EnchantmentType.Infinity }, EnchantmentCategory.Breakable, 2, true, true) },
        { EnchantmentType.VanishingCurse, new EnchantmentInfo(37, "vanishing_curse", "Curse of Vanishing", 1, new EnchantCost(0, 25), new EnchantCost(0, 50), true, true, Array.Empty<EnchantmentType>(), EnchantmentCategory.Vanishable, 1, true, true) },
    };
    public override Dictionary<EnchantmentType, EnchantmentInfo> Palette => Values;
}
