/////////////////////////////////////////////////////////////////
//   Generated Enchantment Data for Minecraft Version 1.18.1   //
/////////////////////////////////////////////////////////////////
using MineSharp.Core.Types;
using System.Collections.Generic;
namespace MineSharp.Data.Enchantments {
	public static class EnchantmentPalette {
		public static Type GetEnchantmentTypeById(int id) => id switch {
			0 => typeof(Protection),
			1 => typeof(FireProtection),
			2 => typeof(FeatherFalling),
			3 => typeof(BlastProtection),
			4 => typeof(ProjectileProtection),
			5 => typeof(Respiration),
			6 => typeof(AquaAffinity),
			7 => typeof(Thorns),
			8 => typeof(DepthStrider),
			9 => typeof(FrostWalker),
			10 => typeof(BindingCurse),
			11 => typeof(SoulSpeed),
			12 => typeof(Sharpness),
			13 => typeof(Smite),
			14 => typeof(BaneOfArthropods),
			15 => typeof(Knockback),
			16 => typeof(FireAspect),
			17 => typeof(Looting),
			18 => typeof(Sweeping),
			19 => typeof(Efficiency),
			20 => typeof(SilkTouch),
			21 => typeof(Unbreaking),
			22 => typeof(Fortune),
			23 => typeof(Power),
			24 => typeof(Punch),
			25 => typeof(Flame),
			26 => typeof(Infinity),
			27 => typeof(LuckOfTheSea),
			28 => typeof(Lure),
			29 => typeof(Loyalty),
			30 => typeof(Impaling),
			31 => typeof(Riptide),
			32 => typeof(Channeling),
			33 => typeof(Multishot),
			34 => typeof(QuickCharge),
			35 => typeof(Piercing),
			36 => typeof(Mending),
			37 => typeof(VanishingCurse),
			_ => throw new ArgumentException($"Enchantment with id {id} not found!")
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
	public class Protection : Enchantment {
		
		public const int EnchantmentId = 0;
		public const string EnchantmentName = "protection";
		public const string EnchantmentDisplayName = "Protection";
		
		public const int EnchantmentMaxLevel = 4;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(11, -10);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(11, 1);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Protection), typeof(Protection), typeof(Protection) };
		public const int EnchantmentCategory = 0;
		public const int EnchantmentWeight = 10;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Protection () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Protection (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class FireProtection : Enchantment {
		
		public const int EnchantmentId = 1;
		public const string EnchantmentName = "fire_protection";
		public const string EnchantmentDisplayName = "Fire Protection";
		
		public const int EnchantmentMaxLevel = 4;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(8, 2);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(8, 10);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(FireProtection), typeof(FireProtection), typeof(FireProtection) };
		public const int EnchantmentCategory = 0;
		public const int EnchantmentWeight = 5;
		public const bool EnchantmentDiscoverable = true;
		
		
		public FireProtection () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public FireProtection (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class FeatherFalling : Enchantment {
		
		public const int EnchantmentId = 2;
		public const string EnchantmentName = "feather_falling";
		public const string EnchantmentDisplayName = "Feather Falling";
		
		public const int EnchantmentMaxLevel = 4;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(6, -1);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(6, 5);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 1;
		public const int EnchantmentWeight = 5;
		public const bool EnchantmentDiscoverable = true;
		
		
		public FeatherFalling () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public FeatherFalling (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class BlastProtection : Enchantment {
		
		public const int EnchantmentId = 3;
		public const string EnchantmentName = "blast_protection";
		public const string EnchantmentDisplayName = "Blast Protection";
		
		public const int EnchantmentMaxLevel = 4;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(8, -3);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(8, 5);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(BlastProtection), typeof(BlastProtection), typeof(BlastProtection) };
		public const int EnchantmentCategory = 0;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public BlastProtection () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public BlastProtection (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class ProjectileProtection : Enchantment {
		
		public const int EnchantmentId = 4;
		public const string EnchantmentName = "projectile_protection";
		public const string EnchantmentDisplayName = "Projectile Protection";
		
		public const int EnchantmentMaxLevel = 4;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(6, -3);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(6, 3);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(ProjectileProtection), typeof(ProjectileProtection), typeof(ProjectileProtection) };
		public const int EnchantmentCategory = 0;
		public const int EnchantmentWeight = 5;
		public const bool EnchantmentDiscoverable = true;
		
		
		public ProjectileProtection () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public ProjectileProtection (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Respiration : Enchantment {
		
		public const int EnchantmentId = 5;
		public const string EnchantmentName = "respiration";
		public const string EnchantmentDisplayName = "Respiration";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(10, 0);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 30);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 2;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Respiration () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Respiration (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class AquaAffinity : Enchantment {
		
		public const int EnchantmentId = 6;
		public const string EnchantmentName = "aqua_affinity";
		public const string EnchantmentDisplayName = "Aqua Affinity";
		
		public const int EnchantmentMaxLevel = 1;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(0, 1);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 41);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 2;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public AquaAffinity () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public AquaAffinity (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Thorns : Enchantment {
		
		public const int EnchantmentId = 7;
		public const string EnchantmentName = "thorns";
		public const string EnchantmentDisplayName = "Thorns";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(20, -10);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 3;
		public const int EnchantmentWeight = 1;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Thorns () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Thorns (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class DepthStrider : Enchantment {
		
		public const int EnchantmentId = 8;
		public const string EnchantmentName = "depth_strider";
		public const string EnchantmentDisplayName = "Depth Strider";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(10, 0);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 15);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(DepthStrider) };
		public const int EnchantmentCategory = 1;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public DepthStrider () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public DepthStrider (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class FrostWalker : Enchantment {
		
		public const int EnchantmentId = 9;
		public const string EnchantmentName = "frost_walker";
		public const string EnchantmentDisplayName = "Frost Walker";
		
		public const int EnchantmentMaxLevel = 2;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(10, 0);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 15);
		public const bool EnchantmentTreasureOnly = true;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(FrostWalker) };
		public const int EnchantmentCategory = 1;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public FrostWalker () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public FrostWalker (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class BindingCurse : Enchantment {
		
		public const int EnchantmentId = 10;
		public const string EnchantmentName = "binding_curse";
		public const string EnchantmentDisplayName = "Curse of Binding";
		
		public const int EnchantmentMaxLevel = 1;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(0, 25);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = true;
		public const bool EnchantmentCurse = true;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 4;
		public const int EnchantmentWeight = 1;
		public const bool EnchantmentDiscoverable = true;
		
		
		public BindingCurse () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public BindingCurse (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class SoulSpeed : Enchantment {
		
		public const int EnchantmentId = 11;
		public const string EnchantmentName = "soul_speed";
		public const string EnchantmentDisplayName = "Soul Speed";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(10, 0);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 15);
		public const bool EnchantmentTreasureOnly = true;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 1;
		public const int EnchantmentWeight = 1;
		public const bool EnchantmentDiscoverable = false;
		
		
		public SoulSpeed () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public SoulSpeed (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Sharpness : Enchantment {
		
		public const int EnchantmentId = 12;
		public const string EnchantmentName = "sharpness";
		public const string EnchantmentDisplayName = "Sharpness";
		
		public const int EnchantmentMaxLevel = 5;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(11, -10);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(11, 10);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Sharpness), typeof(Sharpness) };
		public const int EnchantmentCategory = 5;
		public const int EnchantmentWeight = 10;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Sharpness () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Sharpness (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Smite : Enchantment {
		
		public const int EnchantmentId = 13;
		public const string EnchantmentName = "smite";
		public const string EnchantmentDisplayName = "Smite";
		
		public const int EnchantmentMaxLevel = 5;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(8, -3);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(8, 17);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Smite), typeof(Smite) };
		public const int EnchantmentCategory = 5;
		public const int EnchantmentWeight = 5;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Smite () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Smite (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class BaneOfArthropods : Enchantment {
		
		public const int EnchantmentId = 14;
		public const string EnchantmentName = "bane_of_arthropods";
		public const string EnchantmentDisplayName = "Bane of Arthropods";
		
		public const int EnchantmentMaxLevel = 5;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(8, -3);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(8, 17);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(BaneOfArthropods), typeof(BaneOfArthropods) };
		public const int EnchantmentCategory = 5;
		public const int EnchantmentWeight = 5;
		public const bool EnchantmentDiscoverable = true;
		
		
		public BaneOfArthropods () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public BaneOfArthropods (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Knockback : Enchantment {
		
		public const int EnchantmentId = 15;
		public const string EnchantmentName = "knockback";
		public const string EnchantmentDisplayName = "Knockback";
		
		public const int EnchantmentMaxLevel = 2;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(20, -15);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 5;
		public const int EnchantmentWeight = 5;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Knockback () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Knockback (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class FireAspect : Enchantment {
		
		public const int EnchantmentId = 16;
		public const string EnchantmentName = "fire_aspect";
		public const string EnchantmentDisplayName = "Fire Aspect";
		
		public const int EnchantmentMaxLevel = 2;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(20, -10);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 5;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public FireAspect () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public FireAspect (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Looting : Enchantment {
		
		public const int EnchantmentId = 17;
		public const string EnchantmentName = "looting";
		public const string EnchantmentDisplayName = "Looting";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(9, 6);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Looting) };
		public const int EnchantmentCategory = 5;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Looting () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Looting (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Sweeping : Enchantment {
		
		public const int EnchantmentId = 18;
		public const string EnchantmentName = "sweeping";
		public const string EnchantmentDisplayName = "Sweeping Edge";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(9, -4);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(9, 11);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 5;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Sweeping () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Sweeping (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Efficiency : Enchantment {
		
		public const int EnchantmentId = 19;
		public const string EnchantmentName = "efficiency";
		public const string EnchantmentDisplayName = "Efficiency";
		
		public const int EnchantmentMaxLevel = 5;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(10, -9);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 6;
		public const int EnchantmentWeight = 10;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Efficiency () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Efficiency (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class SilkTouch : Enchantment {
		
		public const int EnchantmentId = 20;
		public const string EnchantmentName = "silk_touch";
		public const string EnchantmentDisplayName = "Silk Touch";
		
		public const int EnchantmentMaxLevel = 1;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(0, 15);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(SilkTouch), typeof(SilkTouch), typeof(SilkTouch) };
		public const int EnchantmentCategory = 6;
		public const int EnchantmentWeight = 1;
		public const bool EnchantmentDiscoverable = true;
		
		
		public SilkTouch () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public SilkTouch (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Unbreaking : Enchantment {
		
		public const int EnchantmentId = 21;
		public const string EnchantmentName = "unbreaking";
		public const string EnchantmentDisplayName = "Unbreaking";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(8, -3);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 7;
		public const int EnchantmentWeight = 5;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Unbreaking () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Unbreaking (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Fortune : Enchantment {
		
		public const int EnchantmentId = 22;
		public const string EnchantmentName = "fortune";
		public const string EnchantmentDisplayName = "Fortune";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(9, 6);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Fortune) };
		public const int EnchantmentCategory = 6;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Fortune () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Fortune (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Power : Enchantment {
		
		public const int EnchantmentId = 23;
		public const string EnchantmentName = "power";
		public const string EnchantmentDisplayName = "Power";
		
		public const int EnchantmentMaxLevel = 5;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(10, -9);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 6);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 8;
		public const int EnchantmentWeight = 10;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Power () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Power (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Punch : Enchantment {
		
		public const int EnchantmentId = 24;
		public const string EnchantmentName = "punch";
		public const string EnchantmentDisplayName = "Punch";
		
		public const int EnchantmentMaxLevel = 2;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(20, -8);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(20, 17);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 8;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Punch () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Punch (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Flame : Enchantment {
		
		public const int EnchantmentId = 25;
		public const string EnchantmentName = "flame";
		public const string EnchantmentDisplayName = "Flame";
		
		public const int EnchantmentMaxLevel = 1;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(0, 20);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 8;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Flame () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Flame (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Infinity : Enchantment {
		
		public const int EnchantmentId = 26;
		public const string EnchantmentName = "infinity";
		public const string EnchantmentDisplayName = "Infinity";
		
		public const int EnchantmentMaxLevel = 1;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(0, 20);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Infinity) };
		public const int EnchantmentCategory = 8;
		public const int EnchantmentWeight = 1;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Infinity () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Infinity (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class LuckOfTheSea : Enchantment {
		
		public const int EnchantmentId = 27;
		public const string EnchantmentName = "luck_of_the_sea";
		public const string EnchantmentDisplayName = "Luck of the Sea";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(9, 6);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(LuckOfTheSea) };
		public const int EnchantmentCategory = 9;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public LuckOfTheSea () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public LuckOfTheSea (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Lure : Enchantment {
		
		public const int EnchantmentId = 28;
		public const string EnchantmentName = "lure";
		public const string EnchantmentDisplayName = "Lure";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(9, 6);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(10, 51);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 9;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Lure () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Lure (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Loyalty : Enchantment {
		
		public const int EnchantmentId = 29;
		public const string EnchantmentName = "loyalty";
		public const string EnchantmentDisplayName = "Loyalty";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(7, 5);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Loyalty) };
		public const int EnchantmentCategory = 10;
		public const int EnchantmentWeight = 5;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Loyalty () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Loyalty (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Impaling : Enchantment {
		
		public const int EnchantmentId = 30;
		public const string EnchantmentName = "impaling";
		public const string EnchantmentDisplayName = "Impaling";
		
		public const int EnchantmentMaxLevel = 5;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(8, -7);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(8, 13);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 10;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Impaling () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Impaling (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Riptide : Enchantment {
		
		public const int EnchantmentId = 31;
		public const string EnchantmentName = "riptide";
		public const string EnchantmentDisplayName = "Riptide";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(7, 10);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Riptide), typeof(Riptide) };
		public const int EnchantmentCategory = 10;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Riptide () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Riptide (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Channeling : Enchantment {
		
		public const int EnchantmentId = 32;
		public const string EnchantmentName = "channeling";
		public const string EnchantmentDisplayName = "Channeling";
		
		public const int EnchantmentMaxLevel = 1;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(0, 25);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Channeling) };
		public const int EnchantmentCategory = 10;
		public const int EnchantmentWeight = 1;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Channeling () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Channeling (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Multishot : Enchantment {
		
		public const int EnchantmentId = 33;
		public const string EnchantmentName = "multishot";
		public const string EnchantmentDisplayName = "Multishot";
		
		public const int EnchantmentMaxLevel = 1;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(0, 20);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Multishot) };
		public const int EnchantmentCategory = 11;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Multishot () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Multishot (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class QuickCharge : Enchantment {
		
		public const int EnchantmentId = 34;
		public const string EnchantmentName = "quick_charge";
		public const string EnchantmentDisplayName = "Quick Charge";
		
		public const int EnchantmentMaxLevel = 3;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(20, -8);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 11;
		public const int EnchantmentWeight = 5;
		public const bool EnchantmentDiscoverable = true;
		
		
		public QuickCharge () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public QuickCharge (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Piercing : Enchantment {
		
		public const int EnchantmentId = 35;
		public const string EnchantmentName = "piercing";
		public const string EnchantmentDisplayName = "Piercing";
		
		public const int EnchantmentMaxLevel = 4;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(10, -9);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = false;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Piercing) };
		public const int EnchantmentCategory = 11;
		public const int EnchantmentWeight = 10;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Piercing () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Piercing (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class Mending : Enchantment {
		
		public const int EnchantmentId = 36;
		public const string EnchantmentName = "mending";
		public const string EnchantmentDisplayName = "Mending";
		
		public const int EnchantmentMaxLevel = 1;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(25, 0);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(25, 50);
		public const bool EnchantmentTreasureOnly = true;
		public const bool EnchantmentCurse = false;
		public static readonly Type[] EnchantmentExclude = new Type[] { typeof(Mending) };
		public const int EnchantmentCategory = 7;
		public const int EnchantmentWeight = 2;
		public const bool EnchantmentDiscoverable = true;
		
		
		public Mending () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public Mending (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
	}
	public class VanishingCurse : Enchantment {
		
		public const int EnchantmentId = 37;
		public const string EnchantmentName = "vanishing_curse";
		public const string EnchantmentDisplayName = "Curse of Vanishing";
		
		public const int EnchantmentMaxLevel = 1;
		public static readonly EnchantCost EnchantmentMinCost = new EnchantCost(0, 25);
		public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost(0, 50);
		public const bool EnchantmentTreasureOnly = true;
		public const bool EnchantmentCurse = true;
		public static readonly Type[] EnchantmentExclude = new Type[] {  };
		public const int EnchantmentCategory = 12;
		public const int EnchantmentWeight = 1;
		public const bool EnchantmentDiscoverable = true;
		
		
		public VanishingCurse () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
		
		public VanishingCurse (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {}
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
