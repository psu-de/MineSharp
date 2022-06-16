

///////////////////////////////////////////////////////////////
//  Generated Enchantment Data for Minecraft Version 1.18.1  //
///////////////////////////////////////////////////////////////

using MineSharp.Core.Types;

namespace MineSharp.Data.Enchantments {

	
    public static class EnchantmentPalette {

	        public static Type[] AllEnchantments = new Type[] {  typeof(ProtectionEnchantment),  typeof(FireProtectionEnchantment),  typeof(FeatherFallingEnchantment),  typeof(BlastProtectionEnchantment),  typeof(ProjectileProtectionEnchantment),  typeof(RespirationEnchantment),  typeof(AquaAffinityEnchantment),  typeof(ThornsEnchantment),  typeof(DepthStriderEnchantment),  typeof(FrostWalkerEnchantment),  typeof(BindingCurseEnchantment),  typeof(SoulSpeedEnchantment),  typeof(SharpnessEnchantment),  typeof(SmiteEnchantment),  typeof(BaneOfArthropodsEnchantment),  typeof(KnockbackEnchantment),  typeof(FireAspectEnchantment),  typeof(LootingEnchantment),  typeof(SweepingEnchantment),  typeof(EfficiencyEnchantment),  typeof(SilkTouchEnchantment),  typeof(UnbreakingEnchantment),  typeof(FortuneEnchantment),  typeof(PowerEnchantment),  typeof(PunchEnchantment),  typeof(FlameEnchantment),  typeof(InfinityEnchantment),  typeof(LuckOfTheSeaEnchantment),  typeof(LureEnchantment),  typeof(LoyaltyEnchantment),  typeof(ImpalingEnchantment),  typeof(RiptideEnchantment),  typeof(ChannelingEnchantment),  typeof(MultishotEnchantment),  typeof(QuickChargeEnchantment),  typeof(PiercingEnchantment),  typeof(MendingEnchantment),  typeof(VanishingCurseEnchantment),  };


        public static Type GetEnchantmentTypeById(int id) => id switch {
        				0 => typeof(ProtectionEnchantment),
							1 => typeof(FireProtectionEnchantment),
							2 => typeof(FeatherFallingEnchantment),
							3 => typeof(BlastProtectionEnchantment),
							4 => typeof(ProjectileProtectionEnchantment),
							5 => typeof(RespirationEnchantment),
							6 => typeof(AquaAffinityEnchantment),
							7 => typeof(ThornsEnchantment),
							8 => typeof(DepthStriderEnchantment),
							9 => typeof(FrostWalkerEnchantment),
							10 => typeof(BindingCurseEnchantment),
							11 => typeof(SoulSpeedEnchantment),
							12 => typeof(SharpnessEnchantment),
							13 => typeof(SmiteEnchantment),
							14 => typeof(BaneOfArthropodsEnchantment),
							15 => typeof(KnockbackEnchantment),
							16 => typeof(FireAspectEnchantment),
							17 => typeof(LootingEnchantment),
							18 => typeof(SweepingEnchantment),
							19 => typeof(EfficiencyEnchantment),
							20 => typeof(SilkTouchEnchantment),
							21 => typeof(UnbreakingEnchantment),
							22 => typeof(FortuneEnchantment),
							23 => typeof(PowerEnchantment),
							24 => typeof(PunchEnchantment),
							25 => typeof(FlameEnchantment),
							26 => typeof(InfinityEnchantment),
							27 => typeof(LuckOfTheSeaEnchantment),
							28 => typeof(LureEnchantment),
							29 => typeof(LoyaltyEnchantment),
							30 => typeof(ImpalingEnchantment),
							31 => typeof(RiptideEnchantment),
							32 => typeof(ChannelingEnchantment),
							33 => typeof(MultishotEnchantment),
							34 => typeof(QuickChargeEnchantment),
							35 => typeof(PiercingEnchantment),
							36 => typeof(MendingEnchantment),
							37 => typeof(VanishingCurseEnchantment),
						_ => throw new ArgumentException("Effect with id " + id + " not found!")

        };
	}

			public enum EnchantmentCategory {
						Armor = 0,
								ArmorFeet = 1,
								ArmorHead = 2,
								ArmorChest = 3,
								Wearable = 4,
								Weapon = 5,
								Digger = 6,
								Breakable = 7,
								Bow = 8,
								FishingRod = 9,
								Trident = 10,
								Crossbow = 11,
								Vanishable = 12,
				
	}

	public class ProtectionEnchantment : Enchantment {
		public const int EffectId = 0;
		public const string EffectName = "protection";
		public const string EffectDisplayName = "Protection";
		
		public const int EffectMaxLevel = 4;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(11, -10);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(11, 1);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(FireProtectionEnchantment),  typeof(BlastProtectionEnchantment),  typeof(ProjectileProtectionEnchantment),  };
        public const int EffectCategory = 0;
        public const int EffectWeight = 10;
        public const bool EffectDiscoverable = true;

		public ProtectionEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public ProtectionEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class FireProtectionEnchantment : Enchantment {
		public const int EffectId = 1;
		public const string EffectName = "fire_protection";
		public const string EffectDisplayName = "Fire Protection";
		
		public const int EffectMaxLevel = 4;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(8, 2);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(8, 10);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(ProtectionEnchantment),  typeof(BlastProtectionEnchantment),  typeof(ProjectileProtectionEnchantment),  };
        public const int EffectCategory = 0;
        public const int EffectWeight = 5;
        public const bool EffectDiscoverable = true;

		public FireProtectionEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public FireProtectionEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class FeatherFallingEnchantment : Enchantment {
		public const int EffectId = 2;
		public const string EffectName = "feather_falling";
		public const string EffectDisplayName = "Feather Falling";
		
		public const int EffectMaxLevel = 4;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(6, -1);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(6, 5);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 1;
        public const int EffectWeight = 5;
        public const bool EffectDiscoverable = true;

		public FeatherFallingEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public FeatherFallingEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class BlastProtectionEnchantment : Enchantment {
		public const int EffectId = 3;
		public const string EffectName = "blast_protection";
		public const string EffectDisplayName = "Blast Protection";
		
		public const int EffectMaxLevel = 4;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(8, -3);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(8, 5);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(ProtectionEnchantment),  typeof(FireProtectionEnchantment),  typeof(ProjectileProtectionEnchantment),  };
        public const int EffectCategory = 0;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public BlastProtectionEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public BlastProtectionEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class ProjectileProtectionEnchantment : Enchantment {
		public const int EffectId = 4;
		public const string EffectName = "projectile_protection";
		public const string EffectDisplayName = "Projectile Protection";
		
		public const int EffectMaxLevel = 4;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(6, -3);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(6, 3);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(ProtectionEnchantment),  typeof(FireProtectionEnchantment),  typeof(BlastProtectionEnchantment),  };
        public const int EffectCategory = 0;
        public const int EffectWeight = 5;
        public const bool EffectDiscoverable = true;

		public ProjectileProtectionEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public ProjectileProtectionEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class RespirationEnchantment : Enchantment {
		public const int EffectId = 5;
		public const string EffectName = "respiration";
		public const string EffectDisplayName = "Respiration";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(10, 0);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 30);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 2;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public RespirationEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public RespirationEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class AquaAffinityEnchantment : Enchantment {
		public const int EffectId = 6;
		public const string EffectName = "aqua_affinity";
		public const string EffectDisplayName = "Aqua Affinity";
		
		public const int EffectMaxLevel = 1;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(0, 1);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 41);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 2;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public AquaAffinityEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public AquaAffinityEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class ThornsEnchantment : Enchantment {
		public const int EffectId = 7;
		public const string EffectName = "thorns";
		public const string EffectDisplayName = "Thorns";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(20, -10);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 3;
        public const int EffectWeight = 1;
        public const bool EffectDiscoverable = true;

		public ThornsEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public ThornsEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class DepthStriderEnchantment : Enchantment {
		public const int EffectId = 8;
		public const string EffectName = "depth_strider";
		public const string EffectDisplayName = "Depth Strider";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(10, 0);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 15);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(FrostWalkerEnchantment),  };
        public const int EffectCategory = 1;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public DepthStriderEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public DepthStriderEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class FrostWalkerEnchantment : Enchantment {
		public const int EffectId = 9;
		public const string EffectName = "frost_walker";
		public const string EffectDisplayName = "Frost Walker";
		
		public const int EffectMaxLevel = 2;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(10, 0);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 15);
        public const bool EffectTreasureOnly = true;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(DepthStriderEnchantment),  };
        public const int EffectCategory = 1;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public FrostWalkerEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public FrostWalkerEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class BindingCurseEnchantment : Enchantment {
		public const int EffectId = 10;
		public const string EffectName = "binding_curse";
		public const string EffectDisplayName = "Curse of Binding";
		
		public const int EffectMaxLevel = 1;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(0, 25);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = true;
        public const bool EffectCurse = true;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 4;
        public const int EffectWeight = 1;
        public const bool EffectDiscoverable = true;

		public BindingCurseEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public BindingCurseEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class SoulSpeedEnchantment : Enchantment {
		public const int EffectId = 11;
		public const string EffectName = "soul_speed";
		public const string EffectDisplayName = "Soul Speed";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(10, 0);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 15);
        public const bool EffectTreasureOnly = true;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 1;
        public const int EffectWeight = 1;
        public const bool EffectDiscoverable = false;

		public SoulSpeedEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public SoulSpeedEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class SharpnessEnchantment : Enchantment {
		public const int EffectId = 12;
		public const string EffectName = "sharpness";
		public const string EffectDisplayName = "Sharpness";
		
		public const int EffectMaxLevel = 5;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(11, -10);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(11, 10);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(SmiteEnchantment),  typeof(BaneOfArthropodsEnchantment),  };
        public const int EffectCategory = 5;
        public const int EffectWeight = 10;
        public const bool EffectDiscoverable = true;

		public SharpnessEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public SharpnessEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class SmiteEnchantment : Enchantment {
		public const int EffectId = 13;
		public const string EffectName = "smite";
		public const string EffectDisplayName = "Smite";
		
		public const int EffectMaxLevel = 5;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(8, -3);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(8, 17);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(SharpnessEnchantment),  typeof(BaneOfArthropodsEnchantment),  };
        public const int EffectCategory = 5;
        public const int EffectWeight = 5;
        public const bool EffectDiscoverable = true;

		public SmiteEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public SmiteEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class BaneOfArthropodsEnchantment : Enchantment {
		public const int EffectId = 14;
		public const string EffectName = "bane_of_arthropods";
		public const string EffectDisplayName = "Bane of Arthropods";
		
		public const int EffectMaxLevel = 5;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(8, -3);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(8, 17);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(SharpnessEnchantment),  typeof(SmiteEnchantment),  };
        public const int EffectCategory = 5;
        public const int EffectWeight = 5;
        public const bool EffectDiscoverable = true;

		public BaneOfArthropodsEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public BaneOfArthropodsEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class KnockbackEnchantment : Enchantment {
		public const int EffectId = 15;
		public const string EffectName = "knockback";
		public const string EffectDisplayName = "Knockback";
		
		public const int EffectMaxLevel = 2;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(20, -15);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 5;
        public const int EffectWeight = 5;
        public const bool EffectDiscoverable = true;

		public KnockbackEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public KnockbackEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class FireAspectEnchantment : Enchantment {
		public const int EffectId = 16;
		public const string EffectName = "fire_aspect";
		public const string EffectDisplayName = "Fire Aspect";
		
		public const int EffectMaxLevel = 2;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(20, -10);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 5;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public FireAspectEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public FireAspectEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class LootingEnchantment : Enchantment {
		public const int EffectId = 17;
		public const string EffectName = "looting";
		public const string EffectDisplayName = "Looting";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(9, 6);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(SilkTouchEnchantment),  };
        public const int EffectCategory = 5;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public LootingEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public LootingEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class SweepingEnchantment : Enchantment {
		public const int EffectId = 18;
		public const string EffectName = "sweeping";
		public const string EffectDisplayName = "Sweeping Edge";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(9, -4);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(9, 11);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 5;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public SweepingEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public SweepingEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class EfficiencyEnchantment : Enchantment {
		public const int EffectId = 19;
		public const string EffectName = "efficiency";
		public const string EffectDisplayName = "Efficiency";
		
		public const int EffectMaxLevel = 5;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(10, -9);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 6;
        public const int EffectWeight = 10;
        public const bool EffectDiscoverable = true;

		public EfficiencyEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public EfficiencyEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class SilkTouchEnchantment : Enchantment {
		public const int EffectId = 20;
		public const string EffectName = "silk_touch";
		public const string EffectDisplayName = "Silk Touch";
		
		public const int EffectMaxLevel = 1;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(0, 15);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(LootingEnchantment),  typeof(FortuneEnchantment),  typeof(LuckOfTheSeaEnchantment),  };
        public const int EffectCategory = 6;
        public const int EffectWeight = 1;
        public const bool EffectDiscoverable = true;

		public SilkTouchEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public SilkTouchEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class UnbreakingEnchantment : Enchantment {
		public const int EffectId = 21;
		public const string EffectName = "unbreaking";
		public const string EffectDisplayName = "Unbreaking";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(8, -3);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 7;
        public const int EffectWeight = 5;
        public const bool EffectDiscoverable = true;

		public UnbreakingEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public UnbreakingEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class FortuneEnchantment : Enchantment {
		public const int EffectId = 22;
		public const string EffectName = "fortune";
		public const string EffectDisplayName = "Fortune";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(9, 6);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(SilkTouchEnchantment),  };
        public const int EffectCategory = 6;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public FortuneEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public FortuneEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class PowerEnchantment : Enchantment {
		public const int EffectId = 23;
		public const string EffectName = "power";
		public const string EffectDisplayName = "Power";
		
		public const int EffectMaxLevel = 5;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(10, -9);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 6);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 8;
        public const int EffectWeight = 10;
        public const bool EffectDiscoverable = true;

		public PowerEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public PowerEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class PunchEnchantment : Enchantment {
		public const int EffectId = 24;
		public const string EffectName = "punch";
		public const string EffectDisplayName = "Punch";
		
		public const int EffectMaxLevel = 2;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(20, -8);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(20, 17);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 8;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public PunchEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public PunchEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class FlameEnchantment : Enchantment {
		public const int EffectId = 25;
		public const string EffectName = "flame";
		public const string EffectDisplayName = "Flame";
		
		public const int EffectMaxLevel = 1;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(0, 20);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 8;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public FlameEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public FlameEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class InfinityEnchantment : Enchantment {
		public const int EffectId = 26;
		public const string EffectName = "infinity";
		public const string EffectDisplayName = "Infinity";
		
		public const int EffectMaxLevel = 1;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(0, 20);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(MendingEnchantment),  };
        public const int EffectCategory = 8;
        public const int EffectWeight = 1;
        public const bool EffectDiscoverable = true;

		public InfinityEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public InfinityEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class LuckOfTheSeaEnchantment : Enchantment {
		public const int EffectId = 27;
		public const string EffectName = "luck_of_the_sea";
		public const string EffectDisplayName = "Luck of the Sea";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(9, 6);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(SilkTouchEnchantment),  };
        public const int EffectCategory = 9;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public LuckOfTheSeaEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public LuckOfTheSeaEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class LureEnchantment : Enchantment {
		public const int EffectId = 28;
		public const string EffectName = "lure";
		public const string EffectDisplayName = "Lure";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(9, 6);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(10, 51);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 9;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public LureEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public LureEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class LoyaltyEnchantment : Enchantment {
		public const int EffectId = 29;
		public const string EffectName = "loyalty";
		public const string EffectDisplayName = "Loyalty";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(7, 5);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(RiptideEnchantment),  };
        public const int EffectCategory = 10;
        public const int EffectWeight = 5;
        public const bool EffectDiscoverable = true;

		public LoyaltyEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public LoyaltyEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class ImpalingEnchantment : Enchantment {
		public const int EffectId = 30;
		public const string EffectName = "impaling";
		public const string EffectDisplayName = "Impaling";
		
		public const int EffectMaxLevel = 5;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(8, -7);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(8, 13);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 10;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public ImpalingEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public ImpalingEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class RiptideEnchantment : Enchantment {
		public const int EffectId = 31;
		public const string EffectName = "riptide";
		public const string EffectDisplayName = "Riptide";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(7, 10);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(LoyaltyEnchantment),  typeof(ChannelingEnchantment),  };
        public const int EffectCategory = 10;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public RiptideEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public RiptideEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class ChannelingEnchantment : Enchantment {
		public const int EffectId = 32;
		public const string EffectName = "channeling";
		public const string EffectDisplayName = "Channeling";
		
		public const int EffectMaxLevel = 1;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(0, 25);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(RiptideEnchantment),  };
        public const int EffectCategory = 10;
        public const int EffectWeight = 1;
        public const bool EffectDiscoverable = true;

		public ChannelingEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public ChannelingEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class MultishotEnchantment : Enchantment {
		public const int EffectId = 33;
		public const string EffectName = "multishot";
		public const string EffectDisplayName = "Multishot";
		
		public const int EffectMaxLevel = 1;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(0, 20);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(PiercingEnchantment),  };
        public const int EffectCategory = 11;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public MultishotEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public MultishotEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class QuickChargeEnchantment : Enchantment {
		public const int EffectId = 34;
		public const string EffectName = "quick_charge";
		public const string EffectDisplayName = "Quick Charge";
		
		public const int EffectMaxLevel = 3;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(20, -8);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 11;
        public const int EffectWeight = 5;
        public const bool EffectDiscoverable = true;

		public QuickChargeEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public QuickChargeEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class PiercingEnchantment : Enchantment {
		public const int EffectId = 35;
		public const string EffectName = "piercing";
		public const string EffectDisplayName = "Piercing";
		
		public const int EffectMaxLevel = 4;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(10, -9);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = false;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(MultishotEnchantment),  };
        public const int EffectCategory = 11;
        public const int EffectWeight = 10;
        public const bool EffectDiscoverable = true;

		public PiercingEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public PiercingEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class MendingEnchantment : Enchantment {
		public const int EffectId = 36;
		public const string EffectName = "mending";
		public const string EffectDisplayName = "Mending";
		
		public const int EffectMaxLevel = 1;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(25, 0);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(25, 50);
        public const bool EffectTreasureOnly = true;
        public const bool EffectCurse = false;
        public static readonly Type[] EffectExclude = new Type[] {  typeof(InfinityEnchantment),  };
        public const int EffectCategory = 7;
        public const int EffectWeight = 2;
        public const bool EffectDiscoverable = true;

		public MendingEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public MendingEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	public class VanishingCurseEnchantment : Enchantment {
		public const int EffectId = 37;
		public const string EffectName = "vanishing_curse";
		public const string EffectDisplayName = "Curse of Vanishing";
		
		public const int EffectMaxLevel = 1;
        public static readonly EnchantCost EffectMinCost = new EnchantCost(0, 25);
        public static readonly EnchantCost EffectMaxCost = new EnchantCost(0, 50);
        public const bool EffectTreasureOnly = true;
        public const bool EffectCurse = true;
        public static readonly Type[] EffectExclude = new Type[] {  };
        public const int EffectCategory = 12;
        public const int EffectWeight = 1;
        public const bool EffectDiscoverable = true;

		public VanishingCurseEnchantment () : base(EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}

		public VanishingCurseEnchantment (int level) : base(level, EffectId, EffectName, EffectDisplayName, EffectMaxLevel, EffectMinCost, EffectMaxCost, EffectTreasureOnly, EffectCurse, EffectExclude, EffectCategory, EffectWeight, EffectDiscoverable) {}
	}
	
	public enum EnchantmentType {
					Protection = 0,
					FireProtection = 1,
					FeatherFalling = 2,
					BlastProtection = 3,
					ProjectileProtection = 4,
					Respiration = 5,
					AquaAffinity = 6,
					Thorns = 7,
					DepthStrider = 8,
					FrostWalker = 9,
					BindingCurse = 10,
					SoulSpeed = 11,
					Sharpness = 12,
					Smite = 13,
					BaneOfArthropods = 14,
					Knockback = 15,
					FireAspect = 16,
					Looting = 17,
					Sweeping = 18,
					Efficiency = 19,
					SilkTouch = 20,
					Unbreaking = 21,
					Fortune = 22,
					Power = 23,
					Punch = 24,
					Flame = 25,
					Infinity = 26,
					LuckOfTheSea = 27,
					Lure = 28,
					Loyalty = 29,
					Impaling = 30,
					Riptide = 31,
					Channeling = 32,
					Multishot = 33,
					QuickCharge = 34,
					Piercing = 35,
					Mending = 36,
					VanishingCurse = 37,
			}
}

