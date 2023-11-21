using MineSharp.Core.Common.Effects;

namespace MineSharp.Data.Effects.Versions;

internal class Effects_1_19_3 : DataVersion<EffectType, EffectInfo>
{
    private static Dictionary<EffectType, EffectInfo> Values { get; } = new Dictionary<EffectType, EffectInfo>()
    {
        { EffectType.Speed, new EffectInfo(1, "Speed", "Speed", true) },
        { EffectType.Slowness, new EffectInfo(2, "Slowness", "Slowness", false) },
        { EffectType.Haste, new EffectInfo(3, "Haste", "Haste", true) },
        { EffectType.MiningFatigue, new EffectInfo(4, "MiningFatigue", "Mining Fatigue", false) },
        { EffectType.Strength, new EffectInfo(5, "Strength", "Strength", true) },
        { EffectType.InstantHealth, new EffectInfo(6, "InstantHealth", "Instant Health", true) },
        { EffectType.InstantDamage, new EffectInfo(7, "InstantDamage", "Instant Damage", false) },
        { EffectType.JumpBoost, new EffectInfo(8, "JumpBoost", "Jump Boost", true) },
        { EffectType.Nausea, new EffectInfo(9, "Nausea", "Nausea", false) },
        { EffectType.Regeneration, new EffectInfo(10, "Regeneration", "Regeneration", true) },
        { EffectType.Resistance, new EffectInfo(11, "Resistance", "Resistance", true) },
        { EffectType.FireResistance, new EffectInfo(12, "FireResistance", "Fire Resistance", true) },
        { EffectType.WaterBreathing, new EffectInfo(13, "WaterBreathing", "Water Breathing", true) },
        { EffectType.Invisibility, new EffectInfo(14, "Invisibility", "Invisibility", true) },
        { EffectType.Blindness, new EffectInfo(15, "Blindness", "Blindness", false) },
        { EffectType.NightVision, new EffectInfo(16, "NightVision", "Night Vision", true) },
        { EffectType.Hunger, new EffectInfo(17, "Hunger", "Hunger", false) },
        { EffectType.Weakness, new EffectInfo(18, "Weakness", "Weakness", false) },
        { EffectType.Poison, new EffectInfo(19, "Poison", "Poison", false) },
        { EffectType.Wither, new EffectInfo(20, "Wither", "Wither", false) },
        { EffectType.HealthBoost, new EffectInfo(21, "HealthBoost", "Health Boost", true) },
        { EffectType.Absorption, new EffectInfo(22, "Absorption", "Absorption", true) },
        { EffectType.Saturation, new EffectInfo(23, "Saturation", "Saturation", true) },
        { EffectType.Glowing, new EffectInfo(24, "Glowing", "Glowing", false) },
        { EffectType.Levitation, new EffectInfo(25, "Levitation", "Levitation", false) },
        { EffectType.Luck, new EffectInfo(26, "Luck", "Luck", true) },
        { EffectType.BadLuck, new EffectInfo(27, "BadLuck", "Bad Luck", false) },
        { EffectType.SlowFalling, new EffectInfo(28, "SlowFalling", "Slow Falling", true) },
        { EffectType.ConduitPower, new EffectInfo(29, "ConduitPower", "Conduit Power", true) },
        { EffectType.DolphinsGrace, new EffectInfo(30, "DolphinsGrace", "Dolphin's Grace", true) },
        { EffectType.BadOmen, new EffectInfo(31, "BadOmen", "Bad Omen", false) },
        { EffectType.HeroOfTheVillage, new EffectInfo(32, "HeroOfTheVillage", "Hero of the Village", true) },
        { EffectType.Darkness, new EffectInfo(33, "Darkness", "Darkness", false) },
    };
    public override Dictionary<EffectType, EffectInfo> Palette => Values;
}
