////////////////////////////////////////////////////////////
//   Generated Effect Data for Minecraft Version 1.18.1   //
////////////////////////////////////////////////////////////
using MineSharp.Core.Types;
using System.Collections.Generic;
namespace MineSharp.Data.Effects
{
	public static class EffectPalette
	{
		public static readonly EffectInfo SpeedEffectInfo = new EffectInfo(1, "Speed", "Speed", true);
		public static readonly EffectInfo SlownessEffectInfo = new EffectInfo(2, "Slowness", "Slowness", false);
		public static readonly EffectInfo HasteEffectInfo = new EffectInfo(3, "Haste", "Haste", true);
		public static readonly EffectInfo MiningfatigueEffectInfo = new EffectInfo(4, "MiningFatigue", "Mining Fatigue", false);
		public static readonly EffectInfo StrengthEffectInfo = new EffectInfo(5, "Strength", "Strength", true);
		public static readonly EffectInfo InstanthealthEffectInfo = new EffectInfo(6, "InstantHealth", "Instant Health", true);
		public static readonly EffectInfo InstantdamageEffectInfo = new EffectInfo(7, "InstantDamage", "Instant Damage", false);
		public static readonly EffectInfo JumpboostEffectInfo = new EffectInfo(8, "JumpBoost", "Jump Boost", true);
		public static readonly EffectInfo NauseaEffectInfo = new EffectInfo(9, "Nausea", "Nausea", false);
		public static readonly EffectInfo RegenerationEffectInfo = new EffectInfo(10, "Regeneration", "Regeneration", true);
		public static readonly EffectInfo ResistanceEffectInfo = new EffectInfo(11, "Resistance", "Resistance", true);
		public static readonly EffectInfo FireresistanceEffectInfo = new EffectInfo(12, "FireResistance", "Fire Resistance", true);
		public static readonly EffectInfo WaterbreathingEffectInfo = new EffectInfo(13, "WaterBreathing", "Water Breathing", true);
		public static readonly EffectInfo InvisibilityEffectInfo = new EffectInfo(14, "Invisibility", "Invisibility", true);
		public static readonly EffectInfo BlindnessEffectInfo = new EffectInfo(15, "Blindness", "Blindness", false);
		public static readonly EffectInfo NightvisionEffectInfo = new EffectInfo(16, "NightVision", "Night Vision", true);
		public static readonly EffectInfo HungerEffectInfo = new EffectInfo(17, "Hunger", "Hunger", false);
		public static readonly EffectInfo WeaknessEffectInfo = new EffectInfo(18, "Weakness", "Weakness", false);
		public static readonly EffectInfo PoisonEffectInfo = new EffectInfo(19, "Poison", "Poison", false);
		public static readonly EffectInfo WitherEffectInfo = new EffectInfo(20, "Wither", "Wither", false);
		public static readonly EffectInfo HealthboostEffectInfo = new EffectInfo(21, "HealthBoost", "Health Boost", true);
		public static readonly EffectInfo AbsorptionEffectInfo = new EffectInfo(22, "Absorption", "Absorption", true);
		public static readonly EffectInfo SaturationEffectInfo = new EffectInfo(23, "Saturation", "Saturation", true);
		public static readonly EffectInfo GlowingEffectInfo = new EffectInfo(24, "Glowing", "Glowing", false);
		public static readonly EffectInfo LevitationEffectInfo = new EffectInfo(25, "Levitation", "Levitation", false);
		public static readonly EffectInfo LuckEffectInfo = new EffectInfo(26, "Luck", "Luck", true);
		public static readonly EffectInfo BadluckEffectInfo = new EffectInfo(27, "BadLuck", "Bad Luck", false);
		public static readonly EffectInfo SlowfallingEffectInfo = new EffectInfo(28, "SlowFalling", "Slow Falling", true);
		public static readonly EffectInfo ConduitpowerEffectInfo = new EffectInfo(29, "ConduitPower", "Conduit Power", true);
		public static readonly EffectInfo DolphinsgraceEffectInfo = new EffectInfo(30, "DolphinsGrace", "Dolphin's Grace", true);
		public static readonly EffectInfo BadomenEffectInfo = new EffectInfo(31, "BadOmen", "Bad Omen", false);
		public static readonly EffectInfo HeroofthevillageEffectInfo = new EffectInfo(32, "HeroOfTheVillage", "Hero of the Village", true);
		public static EffectInfo GetEffectInfoById(int id) => id switch
		{
			1 => SpeedEffectInfo,
			2 => SlownessEffectInfo,
			3 => HasteEffectInfo,
			4 => MiningfatigueEffectInfo,
			5 => StrengthEffectInfo,
			6 => InstanthealthEffectInfo,
			7 => InstantdamageEffectInfo,
			8 => JumpboostEffectInfo,
			9 => NauseaEffectInfo,
			10 => RegenerationEffectInfo,
			11 => ResistanceEffectInfo,
			12 => FireresistanceEffectInfo,
			13 => WaterbreathingEffectInfo,
			14 => InvisibilityEffectInfo,
			15 => BlindnessEffectInfo,
			16 => NightvisionEffectInfo,
			17 => HungerEffectInfo,
			18 => WeaknessEffectInfo,
			19 => PoisonEffectInfo,
			20 => WitherEffectInfo,
			21 => HealthboostEffectInfo,
			22 => AbsorptionEffectInfo,
			23 => SaturationEffectInfo,
			24 => GlowingEffectInfo,
			25 => LevitationEffectInfo,
			26 => LuckEffectInfo,
			27 => BadluckEffectInfo,
			28 => SlowfallingEffectInfo,
			29 => ConduitpowerEffectInfo,
			30 => DolphinsgraceEffectInfo,
			31 => BadomenEffectInfo,
			32 => HeroofthevillageEffectInfo,
			_ => throw new ArgumentException($"Biome with id {id} not found!")
		};
	}
	public enum EffectType
	{
		SpeedEffect = 0,
		SlownessEffect = 1,
		HasteEffect = 2,
		MiningfatigueEffect = 3,
		StrengthEffect = 4,
		InstanthealthEffect = 5,
		InstantdamageEffect = 6,
		JumpboostEffect = 7,
		NauseaEffect = 8,
		RegenerationEffect = 9,
		ResistanceEffect = 10,
		FireresistanceEffect = 11,
		WaterbreathingEffect = 12,
		InvisibilityEffect = 13,
		BlindnessEffect = 14,
		NightvisionEffect = 15,
		HungerEffect = 16,
		WeaknessEffect = 17,
		PoisonEffect = 18,
		WitherEffect = 19,
		HealthboostEffect = 20,
		AbsorptionEffect = 21,
		SaturationEffect = 22,
		GlowingEffect = 23,
		LevitationEffect = 24,
		LuckEffect = 25,
		BadluckEffect = 26,
		SlowfallingEffect = 27,
		ConduitpowerEffect = 28,
		DolphinsgraceEffect = 29,
		BadomenEffect = 30,
		HeroofthevillageEffect = 31,
	}
	
	public static class EffectExtensions 
	{
	    public static EffectInfo GetInfo(this EffectType type)
	    {
	        return EffectPalette.GetEffectInfoById((int)type);
	    }
	}
}
