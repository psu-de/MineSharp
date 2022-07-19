////////////////////////////////////////////////////////////
//   Generated Effect Data for Minecraft Version 1.18.1   //
////////////////////////////////////////////////////////////
using MineSharp.Core.Types;
using System.Collections.Generic;
namespace MineSharp.Data.Effects {
	public static class EffectPalette {
		public static Type GetEffectTypeById(int id) => id switch {
			1 => typeof(SpeedEffect),
			2 => typeof(SlownessEffect),
			3 => typeof(HasteEffect),
			4 => typeof(MiningfatigueEffect),
			5 => typeof(StrengthEffect),
			6 => typeof(InstanthealthEffect),
			7 => typeof(InstantdamageEffect),
			8 => typeof(JumpboostEffect),
			9 => typeof(NauseaEffect),
			10 => typeof(RegenerationEffect),
			11 => typeof(ResistanceEffect),
			12 => typeof(FireresistanceEffect),
			13 => typeof(WaterbreathingEffect),
			14 => typeof(InvisibilityEffect),
			15 => typeof(BlindnessEffect),
			16 => typeof(NightvisionEffect),
			17 => typeof(HungerEffect),
			18 => typeof(WeaknessEffect),
			19 => typeof(PoisonEffect),
			20 => typeof(WitherEffect),
			21 => typeof(HealthboostEffect),
			22 => typeof(AbsorptionEffect),
			23 => typeof(SaturationEffect),
			24 => typeof(GlowingEffect),
			25 => typeof(LevitationEffect),
			26 => typeof(LuckEffect),
			27 => typeof(BadluckEffect),
			28 => typeof(SlowfallingEffect),
			29 => typeof(ConduitpowerEffect),
			30 => typeof(DolphinsgraceEffect),
			31 => typeof(BadomenEffect),
			32 => typeof(HeroofthevillageEffect),
			_ => throw new ArgumentException($"Effect with id {id} not found!")
		};
	}
	public class SpeedEffect : Effect {
		
		public const int EffectId = 1;
				public const string EffectName = "Speed";
				public const string EffectDisplayName = "Speed";
		        public const bool EffectIsGood = true;
		
		
		        public SpeedEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public SpeedEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class SlownessEffect : Effect {
		
		public const int EffectId = 2;
				public const string EffectName = "Slowness";
				public const string EffectDisplayName = "Slowness";
		        public const bool EffectIsGood = false;
		
		
		        public SlownessEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public SlownessEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class HasteEffect : Effect {
		
		public const int EffectId = 3;
				public const string EffectName = "Haste";
				public const string EffectDisplayName = "Haste";
		        public const bool EffectIsGood = true;
		
		
		        public HasteEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public HasteEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class MiningfatigueEffect : Effect {
		
		public const int EffectId = 4;
				public const string EffectName = "MiningFatigue";
				public const string EffectDisplayName = "Mining Fatigue";
		        public const bool EffectIsGood = false;
		
		
		        public MiningfatigueEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public MiningfatigueEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class StrengthEffect : Effect {
		
		public const int EffectId = 5;
				public const string EffectName = "Strength";
				public const string EffectDisplayName = "Strength";
		        public const bool EffectIsGood = true;
		
		
		        public StrengthEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public StrengthEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class InstanthealthEffect : Effect {
		
		public const int EffectId = 6;
				public const string EffectName = "InstantHealth";
				public const string EffectDisplayName = "Instant Health";
		        public const bool EffectIsGood = true;
		
		
		        public InstanthealthEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public InstanthealthEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class InstantdamageEffect : Effect {
		
		public const int EffectId = 7;
				public const string EffectName = "InstantDamage";
				public const string EffectDisplayName = "Instant Damage";
		        public const bool EffectIsGood = false;
		
		
		        public InstantdamageEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public InstantdamageEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class JumpboostEffect : Effect {
		
		public const int EffectId = 8;
				public const string EffectName = "JumpBoost";
				public const string EffectDisplayName = "Jump Boost";
		        public const bool EffectIsGood = true;
		
		
		        public JumpboostEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public JumpboostEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class NauseaEffect : Effect {
		
		public const int EffectId = 9;
				public const string EffectName = "Nausea";
				public const string EffectDisplayName = "Nausea";
		        public const bool EffectIsGood = false;
		
		
		        public NauseaEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public NauseaEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class RegenerationEffect : Effect {
		
		public const int EffectId = 10;
				public const string EffectName = "Regeneration";
				public const string EffectDisplayName = "Regeneration";
		        public const bool EffectIsGood = true;
		
		
		        public RegenerationEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public RegenerationEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class ResistanceEffect : Effect {
		
		public const int EffectId = 11;
				public const string EffectName = "Resistance";
				public const string EffectDisplayName = "Resistance";
		        public const bool EffectIsGood = true;
		
		
		        public ResistanceEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public ResistanceEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class FireresistanceEffect : Effect {
		
		public const int EffectId = 12;
				public const string EffectName = "FireResistance";
				public const string EffectDisplayName = "Fire Resistance";
		        public const bool EffectIsGood = true;
		
		
		        public FireresistanceEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public FireresistanceEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class WaterbreathingEffect : Effect {
		
		public const int EffectId = 13;
				public const string EffectName = "WaterBreathing";
				public const string EffectDisplayName = "Water Breathing";
		        public const bool EffectIsGood = true;
		
		
		        public WaterbreathingEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public WaterbreathingEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class InvisibilityEffect : Effect {
		
		public const int EffectId = 14;
				public const string EffectName = "Invisibility";
				public const string EffectDisplayName = "Invisibility";
		        public const bool EffectIsGood = true;
		
		
		        public InvisibilityEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public InvisibilityEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class BlindnessEffect : Effect {
		
		public const int EffectId = 15;
				public const string EffectName = "Blindness";
				public const string EffectDisplayName = "Blindness";
		        public const bool EffectIsGood = false;
		
		
		        public BlindnessEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public BlindnessEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class NightvisionEffect : Effect {
		
		public const int EffectId = 16;
				public const string EffectName = "NightVision";
				public const string EffectDisplayName = "Night Vision";
		        public const bool EffectIsGood = true;
		
		
		        public NightvisionEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public NightvisionEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class HungerEffect : Effect {
		
		public const int EffectId = 17;
				public const string EffectName = "Hunger";
				public const string EffectDisplayName = "Hunger";
		        public const bool EffectIsGood = false;
		
		
		        public HungerEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public HungerEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class WeaknessEffect : Effect {
		
		public const int EffectId = 18;
				public const string EffectName = "Weakness";
				public const string EffectDisplayName = "Weakness";
		        public const bool EffectIsGood = false;
		
		
		        public WeaknessEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public WeaknessEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class PoisonEffect : Effect {
		
		public const int EffectId = 19;
				public const string EffectName = "Poison";
				public const string EffectDisplayName = "Poison";
		        public const bool EffectIsGood = false;
		
		
		        public PoisonEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public PoisonEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class WitherEffect : Effect {
		
		public const int EffectId = 20;
				public const string EffectName = "Wither";
				public const string EffectDisplayName = "Wither";
		        public const bool EffectIsGood = false;
		
		
		        public WitherEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public WitherEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class HealthboostEffect : Effect {
		
		public const int EffectId = 21;
				public const string EffectName = "HealthBoost";
				public const string EffectDisplayName = "Health Boost";
		        public const bool EffectIsGood = true;
		
		
		        public HealthboostEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public HealthboostEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class AbsorptionEffect : Effect {
		
		public const int EffectId = 22;
				public const string EffectName = "Absorption";
				public const string EffectDisplayName = "Absorption";
		        public const bool EffectIsGood = true;
		
		
		        public AbsorptionEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public AbsorptionEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class SaturationEffect : Effect {
		
		public const int EffectId = 23;
				public const string EffectName = "Saturation";
				public const string EffectDisplayName = "Saturation";
		        public const bool EffectIsGood = true;
		
		
		        public SaturationEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public SaturationEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class GlowingEffect : Effect {
		
		public const int EffectId = 24;
				public const string EffectName = "Glowing";
				public const string EffectDisplayName = "Glowing";
		        public const bool EffectIsGood = false;
		
		
		        public GlowingEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public GlowingEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class LevitationEffect : Effect {
		
		public const int EffectId = 25;
				public const string EffectName = "Levitation";
				public const string EffectDisplayName = "Levitation";
		        public const bool EffectIsGood = false;
		
		
		        public LevitationEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public LevitationEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class LuckEffect : Effect {
		
		public const int EffectId = 26;
				public const string EffectName = "Luck";
				public const string EffectDisplayName = "Luck";
		        public const bool EffectIsGood = true;
		
		
		        public LuckEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public LuckEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class BadluckEffect : Effect {
		
		public const int EffectId = 27;
				public const string EffectName = "BadLuck";
				public const string EffectDisplayName = "Bad Luck";
		        public const bool EffectIsGood = false;
		
		
		        public BadluckEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public BadluckEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class SlowfallingEffect : Effect {
		
		public const int EffectId = 28;
				public const string EffectName = "SlowFalling";
				public const string EffectDisplayName = "Slow Falling";
		        public const bool EffectIsGood = true;
		
		
		        public SlowfallingEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public SlowfallingEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class ConduitpowerEffect : Effect {
		
		public const int EffectId = 29;
				public const string EffectName = "ConduitPower";
				public const string EffectDisplayName = "Conduit Power";
		        public const bool EffectIsGood = true;
		
		
		        public ConduitpowerEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public ConduitpowerEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class DolphinsgraceEffect : Effect {
		
		public const int EffectId = 30;
				public const string EffectName = "DolphinsGrace";
				public const string EffectDisplayName = "Dolphin's Grace";
		        public const bool EffectIsGood = true;
		
		
		        public DolphinsgraceEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public DolphinsgraceEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class BadomenEffect : Effect {
		
		public const int EffectId = 31;
				public const string EffectName = "BadOmen";
				public const string EffectDisplayName = "Bad Omen";
		        public const bool EffectIsGood = false;
		
		
		        public BadomenEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public BadomenEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public class HeroofthevillageEffect : Effect {
		
		public const int EffectId = 32;
				public const string EffectName = "HeroOfTheVillage";
				public const string EffectDisplayName = "Hero of the Village";
		        public const bool EffectIsGood = true;
		
		
		        public HeroofthevillageEffect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {} 
				public HeroofthevillageEffect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {}
	}
	public enum EffectType {
		Speed = 1,
		Slowness = 2,
		Haste = 3,
		Miningfatigue = 4,
		Strength = 5,
		Instanthealth = 6,
		Instantdamage = 7,
		Jumpboost = 8,
		Nausea = 9,
		Regeneration = 10,
		Resistance = 11,
		Fireresistance = 12,
		Waterbreathing = 13,
		Invisibility = 14,
		Blindness = 15,
		Nightvision = 16,
		Hunger = 17,
		Weakness = 18,
		Poison = 19,
		Wither = 20,
		Healthboost = 21,
		Absorption = 22,
		Saturation = 23,
		Glowing = 24,
		Levitation = 25,
		Luck = 26,
		Badluck = 27,
		Slowfalling = 28,
		Conduitpower = 29,
		Dolphinsgrace = 30,
		Badomen = 31,
		Heroofthevillage = 32,
	}
}
