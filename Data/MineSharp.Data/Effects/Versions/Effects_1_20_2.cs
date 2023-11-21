using MineSharp.Core.Common.Effects;

namespace MineSharp.Data.Effects.Versions;

internal class Effects_1_20_2 : DataVersion<EffectType, EffectInfo>
{
    private static Dictionary<EffectType, EffectInfo> Values { get; } = new Dictionary<EffectType, EffectInfo>()
    {
        { EffectType.Speed, new EffectInfo(0, "Speed", "Speed", true) },
        { EffectType.Slowness, new EffectInfo(1, "Slowness", "Slowness", false) },
        { EffectType.Haste, new EffectInfo(2, "Haste", "Haste", true) },
        { EffectType.MiningFatigue, new EffectInfo(3, "MiningFatigue", "Mining Fatigue", false) },
        { EffectType.Strength, new EffectInfo(4, "Strength", "Strength", true) },
        { EffectType.InstantHealth, new EffectInfo(5, "InstantHealth", "Instant Health", true) },
        { EffectType.InstantDamage, new EffectInfo(6, "InstantDamage", "Instant Damage", false) },
        { EffectType.JumpBoost, new EffectInfo(7, "JumpBoost", "Jump Boost", true) },
        { EffectType.Nausea, new EffectInfo(8, "Nausea", "Nausea", false) },
        { EffectType.Regeneration, new EffectInfo(9, "Regeneration", "Regeneration", true) },
        { EffectType.Resistance, new EffectInfo(10, "Resistance", "Resistance", true) },
        { EffectType.FireResistance, new EffectInfo(11, "FireResistance", "Fire Resistance", true) },
        { EffectType.WaterBreathing, new EffectInfo(12, "WaterBreathing", "Water Breathing", true) },
        { EffectType.Invisibility, new EffectInfo(13, "Invisibility", "Invisibility", true) },
        { EffectType.Blindness, new EffectInfo(14, "Blindness", "Blindness", false) },
        { EffectType.NightVision, new EffectInfo(15, "NightVision", "Night Vision", true) },
        { EffectType.Hunger, new EffectInfo(16, "Hunger", "Hunger", false) },
        { EffectType.Weakness, new EffectInfo(17, "Weakness", "Weakness", false) },
        { EffectType.Poison, new EffectInfo(18, "Poison", "Poison", false) },
        { EffectType.Wither, new EffectInfo(19, "Wither", "Wither", false) },
        { EffectType.HealthBoost, new EffectInfo(20, "HealthBoost", "Health Boost", true) },
        { EffectType.Absorption, new EffectInfo(21, "Absorption", "Absorption", true) },
        { EffectType.Saturation, new EffectInfo(22, "Saturation", "Saturation", true) },
        { EffectType.Glowing, new EffectInfo(23, "Glowing", "Glowing", false) },
        { EffectType.Levitation, new EffectInfo(24, "Levitation", "Levitation", false) },
        { EffectType.Luck, new EffectInfo(25, "Luck", "Luck", true) },
        { EffectType.BadLuck, new EffectInfo(26, "BadLuck", "Bad Luck", false) },
        { EffectType.SlowFalling, new EffectInfo(27, "SlowFalling", "Slow Falling", true) },
        { EffectType.ConduitPower, new EffectInfo(28, "ConduitPower", "Conduit Power", true) },
        { EffectType.DolphinsGrace, new EffectInfo(29, "DolphinsGrace", "Dolphin's Grace", true) },
        { EffectType.BadOmen, new EffectInfo(30, "BadOmen", "Bad Omen", false) },
        { EffectType.HeroOfTheVillage, new EffectInfo(31, "HeroOfTheVillage", "Hero of the Village", true) },
        { EffectType.Darkness, new EffectInfo(32, "Darkness", "Darkness", false) },
    };
    public override Dictionary<EffectType, EffectInfo> Palette => Values;
}
