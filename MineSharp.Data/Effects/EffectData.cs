



namespace MineSharp.Data.Effects {

    public static class EffectData {

        private static bool isLoaded = false;

        public static Dictionary<EffectType, EffectInfo> Effects = new Dictionary<EffectType, EffectInfo>();

        public static void Load() {

            if (isLoaded) return;

            Register("speed", "Speed", EffectType.Speed, true);
            Register("slowness", "Slowness", EffectType.Slowness, false);
            Register("haste", "Haste", EffectType.Haste, true);
            Register("mining_fatigue", "Mining Fatigue", EffectType.MiningFatigue, false);
            Register("strength", "Strength", EffectType.Strength, true);
            Register("instant_health", "Instant Health", EffectType.InstantHealth, true);
            Register("instant_damage", "Instant Damage", EffectType.InstantDamage, false);
            Register("instant_damage", "Instant Damage", EffectType.InstantDamage, false);
            Register("jump_boost", "Jump Boost", EffectType.JumpBoost, true);
            Register("nausea", "Nausea", EffectType.Nausea, false);
            Register("regeneration", "Regeneration", EffectType.Regeneration, true);
            Register("resistance", "Resistance", EffectType.Resistance, true);
            Register("fire_resistance", "Fire Resistance", EffectType.FireResistance, true);
            Register("water_breathing", "Water Breathing", EffectType.WaterBreathing, true);
            Register("invisibility", "Invisibility", EffectType.Invisibility, true);
            Register("blindness", "Blindness", EffectType.Blindness, false);
            Register("night_vision", "Night Vision", EffectType.NightVision, true);
            Register("hunger", "Hunger", EffectType.Hunger, false);
            Register("weakness", "Weakness", EffectType.Weakness, false);
            Register("poison", "Poison", EffectType.Poison, false);
            Register("wither", "Wither", EffectType.Wither, false);
            Register("health_boost", "Health Boost", EffectType.HealthBoost, true);
            Register("absorption", "Absorption", EffectType.Absorption, true);
            Register("saturation", "Saturation", EffectType.Saturation, true);
            Register("glowing", "Glowing", EffectType.Glowing, false);
            Register("levitation", "Levitation", EffectType.Levitation, false);
            Register("luck", "Luck", EffectType.Luck, true);
            Register("unluck", "Bad Luck", EffectType.BadLuck, false);
            Register("slow_falling", "Slow Falling", EffectType.SlowFalling, true);
            Register("conduit_power", "Conduit Power", EffectType.ConduitPower, true);
            Register("dolphins_grace", "Dolphin's Grace", EffectType.DolphinsGrace, true);
            Register("bad_omen", "Bad Omen", EffectType.BadOmen, false);
            Register("hero_of_the_village", "Hero of the Village", EffectType.HerooftheVillage, true);

            isLoaded = true;
        }

        private static void Register(string name, string displayName, EffectType type, bool isGood) {
            EffectInfo info = new EffectInfo(name, displayName, type, isGood);
            Effects.Add(type, info);
        }

    }
}

