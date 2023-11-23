using MineSharp.Core.Common.Effects;

namespace MineSharp.Data.Effects.Versions;

internal class Effects_1_20_2 : DataVersion<EffectType, EffectInfo>
{
    private static Dictionary<EffectType, EffectInfo> Values { get; } = new Dictionary<EffectType, EffectInfo>()
    {
        { EffectType.Speed, new EffectInfo(0, EffectType.Speed, "Speed", "Speed", true) },
        { EffectType.Slowness, new EffectInfo(1, EffectType.Slowness, "Slowness", "Slowness", false) },
        { EffectType.Haste, new EffectInfo(2, EffectType.Haste, "Haste", "Haste", true) },
        { EffectType.MiningFatigue, new EffectInfo(3, EffectType.MiningFatigue, "MiningFatigue", "Mining Fatigue", false) },
        { EffectType.Strength, new EffectInfo(4, EffectType.Strength, "Strength", "Strength", true) },
        { EffectType.InstantHealth, new EffectInfo(5, EffectType.InstantHealth, "InstantHealth", "Instant Health", true) },
        { EffectType.InstantDamage, new EffectInfo(6, EffectType.InstantDamage, "InstantDamage", "Instant Damage", false) },
        { EffectType.JumpBoost, new EffectInfo(7, EffectType.JumpBoost, "JumpBoost", "Jump Boost", true) },
        { EffectType.Nausea, new EffectInfo(8, EffectType.Nausea, "Nausea", "Nausea", false) },
        { EffectType.Regeneration, new EffectInfo(9, EffectType.Regeneration, "Regeneration", "Regeneration", true) },
        { EffectType.Resistance, new EffectInfo(10, EffectType.Resistance, "Resistance", "Resistance", true) },
        { EffectType.FireResistance, new EffectInfo(11, EffectType.FireResistance, "FireResistance", "Fire Resistance", true) },
        { EffectType.WaterBreathing, new EffectInfo(12, EffectType.WaterBreathing, "WaterBreathing", "Water Breathing", true) },
        { EffectType.Invisibility, new EffectInfo(13, EffectType.Invisibility, "Invisibility", "Invisibility", true) },
        { EffectType.Blindness, new EffectInfo(14, EffectType.Blindness, "Blindness", "Blindness", false) },
        { EffectType.NightVision, new EffectInfo(15, EffectType.NightVision, "NightVision", "Night Vision", true) },
        { EffectType.Hunger, new EffectInfo(16, EffectType.Hunger, "Hunger", "Hunger", false) },
        { EffectType.Weakness, new EffectInfo(17, EffectType.Weakness, "Weakness", "Weakness", false) },
        { EffectType.Poison, new EffectInfo(18, EffectType.Poison, "Poison", "Poison", false) },
        { EffectType.Wither, new EffectInfo(19, EffectType.Wither, "Wither", "Wither", false) },
        { EffectType.HealthBoost, new EffectInfo(20, EffectType.HealthBoost, "HealthBoost", "Health Boost", true) },
        { EffectType.Absorption, new EffectInfo(21, EffectType.Absorption, "Absorption", "Absorption", true) },
        { EffectType.Saturation, new EffectInfo(22, EffectType.Saturation, "Saturation", "Saturation", true) },
        { EffectType.Glowing, new EffectInfo(23, EffectType.Glowing, "Glowing", "Glowing", false) },
        { EffectType.Levitation, new EffectInfo(24, EffectType.Levitation, "Levitation", "Levitation", false) },
        { EffectType.Luck, new EffectInfo(25, EffectType.Luck, "Luck", "Luck", true) },
        { EffectType.BadLuck, new EffectInfo(26, EffectType.BadLuck, "BadLuck", "Bad Luck", false) },
        { EffectType.SlowFalling, new EffectInfo(27, EffectType.SlowFalling, "SlowFalling", "Slow Falling", true) },
        { EffectType.ConduitPower, new EffectInfo(28, EffectType.ConduitPower, "ConduitPower", "Conduit Power", true) },
        { EffectType.DolphinsGrace, new EffectInfo(29, EffectType.DolphinsGrace, "DolphinsGrace", "Dolphin's Grace", true) },
        { EffectType.BadOmen, new EffectInfo(30, EffectType.BadOmen, "BadOmen", "Bad Omen", false) },
        { EffectType.HeroOfTheVillage, new EffectInfo(31, EffectType.HeroOfTheVillage, "HeroOfTheVillage", "Hero of the Village", true) },
        { EffectType.Darkness, new EffectInfo(32, EffectType.Darkness, "Darkness", "Darkness", false) },
    };
    public override Dictionary<EffectType, EffectInfo> Palette => Values;
}
