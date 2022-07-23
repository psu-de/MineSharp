//////////////////////////////////////////////////////////
//   Generated Entity Data for Minecraft Version 1.19   //
//////////////////////////////////////////////////////////
using MineSharp.Core.Types;
using System.Collections.Generic;
namespace MineSharp.Data.Entities {
	public static class EntityPalette {
		public static Type GetEntityTypeById(int id) => id switch {
			0 => typeof(Allay),
			1 => typeof(AreaEffectCloud),
			2 => typeof(ArmorStand),
			3 => typeof(Arrow),
			4 => typeof(Axolotl),
			5 => typeof(Bat),
			6 => typeof(Bee),
			7 => typeof(Blaze),
			8 => typeof(Boat),
			9 => typeof(ChestBoat),
			10 => typeof(Cat),
			11 => typeof(CaveSpider),
			12 => typeof(Chicken),
			13 => typeof(Cod),
			14 => typeof(Cow),
			15 => typeof(Creeper),
			16 => typeof(Dolphin),
			17 => typeof(Donkey),
			18 => typeof(DragonFireball),
			19 => typeof(Drowned),
			20 => typeof(ElderGuardian),
			21 => typeof(EndCrystal),
			22 => typeof(EnderDragon),
			23 => typeof(Enderman),
			24 => typeof(Endermite),
			25 => typeof(Evoker),
			26 => typeof(EvokerFangs),
			27 => typeof(ExperienceOrb),
			28 => typeof(EyeOfEnder),
			29 => typeof(FallingBlock),
			30 => typeof(FireworkRocket),
			31 => typeof(Fox),
			32 => typeof(Frog),
			33 => typeof(Ghast),
			34 => typeof(Giant),
			35 => typeof(GlowItemFrame),
			36 => typeof(GlowSquid),
			37 => typeof(Goat),
			38 => typeof(Guardian),
			39 => typeof(Hoglin),
			40 => typeof(Horse),
			41 => typeof(Husk),
			42 => typeof(Illusioner),
			43 => typeof(IronGolem),
			44 => typeof(Item),
			45 => typeof(ItemFrame),
			46 => typeof(Fireball),
			47 => typeof(LeashKnot),
			48 => typeof(LightningBolt),
			49 => typeof(Llama),
			50 => typeof(LlamaSpit),
			51 => typeof(MagmaCube),
			52 => typeof(Marker),
			53 => typeof(Minecart),
			54 => typeof(ChestMinecart),
			55 => typeof(CommandBlockMinecart),
			56 => typeof(FurnaceMinecart),
			57 => typeof(HopperMinecart),
			58 => typeof(SpawnerMinecart),
			59 => typeof(TntMinecart),
			60 => typeof(Mule),
			61 => typeof(Mooshroom),
			62 => typeof(Ocelot),
			63 => typeof(Painting),
			64 => typeof(Panda),
			65 => typeof(Parrot),
			66 => typeof(Phantom),
			67 => typeof(Pig),
			68 => typeof(Piglin),
			69 => typeof(PiglinBrute),
			70 => typeof(Pillager),
			71 => typeof(PolarBear),
			72 => typeof(Tnt),
			73 => typeof(Pufferfish),
			74 => typeof(Rabbit),
			75 => typeof(Ravager),
			76 => typeof(Salmon),
			77 => typeof(Sheep),
			78 => typeof(Shulker),
			79 => typeof(ShulkerBullet),
			80 => typeof(Silverfish),
			81 => typeof(Skeleton),
			82 => typeof(SkeletonHorse),
			83 => typeof(Slime),
			84 => typeof(SmallFireball),
			85 => typeof(SnowGolem),
			86 => typeof(Snowball),
			87 => typeof(SpectralArrow),
			88 => typeof(Spider),
			89 => typeof(Squid),
			90 => typeof(Stray),
			91 => typeof(Strider),
			92 => typeof(Tadpole),
			93 => typeof(Egg),
			94 => typeof(EnderPearl),
			95 => typeof(ExperienceBottle),
			96 => typeof(Potion),
			97 => typeof(Trident),
			98 => typeof(TraderLlama),
			99 => typeof(TropicalFish),
			100 => typeof(Turtle),
			101 => typeof(Vex),
			102 => typeof(Villager),
			103 => typeof(Vindicator),
			104 => typeof(WanderingTrader),
			105 => typeof(Warden),
			106 => typeof(Witch),
			107 => typeof(Wither),
			108 => typeof(WitherSkeleton),
			109 => typeof(WitherSkull),
			110 => typeof(Wolf),
			111 => typeof(Zoglin),
			112 => typeof(Zombie),
			113 => typeof(ZombieHorse),
			114 => typeof(ZombieVillager),
			115 => typeof(ZombifiedPiglin),
			116 => typeof(Player),
			117 => typeof(FishingBobber),
			_ => throw new ArgumentException($"Entity with id {id} not found!")
		};
	}
	public enum EntityCategory {
		PassiveMobs = 0,
		UNKNOWN = 1,
		Immobile = 2,
		Projectiles = 3,
		HostileMobs = 4,
		Vehicles = 5,
	}
	public class Allay : Entity {
		public const int EntityId = 0;
		public const string EntityName = " allay";
		public const string EntityDisplayName = "Allay";
		
		public const float EntityWidth = 0.35F;
		public const float EntityHeight = 0.6F;
		public const int EntityCategory = 0;
		
		
		public Allay (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class AreaEffectCloud : Entity {
		public const int EntityId = 1;
		public const string EntityName = " area_effect_cloud";
		public const string EntityDisplayName = "Area Effect Cloud";
		
		public const float EntityWidth = 6F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 1;
		
		
		public AreaEffectCloud (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ArmorStand : Entity {
		public const int EntityId = 2;
		public const string EntityName = " armor_stand";
		public const string EntityDisplayName = "Armor Stand";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 1.975F;
		public const int EntityCategory = 2;
		
		
		public ArmorStand (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Arrow : Entity {
		public const int EntityId = 3;
		public const string EntityName = " arrow";
		public const string EntityDisplayName = "Arrow";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 3;
		
		
		public Arrow (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Axolotl : Entity {
		public const int EntityId = 4;
		public const string EntityName = " axolotl";
		public const string EntityDisplayName = "Axolotl";
		
		public const float EntityWidth = 0.75F;
		public const float EntityHeight = 0.42F;
		public const int EntityCategory = 0;
		
		
		public Axolotl (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Bat : Entity {
		public const int EntityId = 5;
		public const string EntityName = " bat";
		public const string EntityDisplayName = "Bat";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.9F;
		public const int EntityCategory = 0;
		
		
		public Bat (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Bee : Entity {
		public const int EntityId = 6;
		public const string EntityName = " bee";
		public const string EntityDisplayName = "Bee";
		
		public const float EntityWidth = 0.7F;
		public const float EntityHeight = 0.6F;
		public const int EntityCategory = 0;
		
		
		public Bee (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Blaze : Entity {
		public const int EntityId = 7;
		public const string EntityName = " blaze";
		public const string EntityDisplayName = "Blaze";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.8F;
		public const int EntityCategory = 4;
		
		
		public Blaze (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Boat : Entity {
		public const int EntityId = 8;
		public const string EntityName = " boat";
		public const string EntityDisplayName = "Boat";
		
		public const float EntityWidth = 1.375F;
		public const float EntityHeight = 0.5625F;
		public const int EntityCategory = 5;
		
		
		public Boat (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ChestBoat : Entity {
		public const int EntityId = 9;
		public const string EntityName = " chest_boat";
		public const string EntityDisplayName = "Boat with Chest";
		
		public const float EntityWidth = 1.375F;
		public const float EntityHeight = 0.5625F;
		public const int EntityCategory = 5;
		
		
		public ChestBoat (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Cat : Entity {
		public const int EntityId = 10;
		public const string EntityName = " cat";
		public const string EntityDisplayName = "Cat";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 0;
		
		
		public Cat (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class CaveSpider : Entity {
		public const int EntityId = 11;
		public const string EntityName = " cave_spider";
		public const string EntityDisplayName = "Cave Spider";
		
		public const float EntityWidth = 0.7F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 4;
		
		
		public CaveSpider (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Chicken : Entity {
		public const int EntityId = 12;
		public const string EntityName = " chicken";
		public const string EntityDisplayName = "Chicken";
		
		public const float EntityWidth = 0.4F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 0;
		
		
		public Chicken (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Cod : Entity {
		public const int EntityId = 13;
		public const string EntityName = " cod";
		public const string EntityDisplayName = "Cod";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.3F;
		public const int EntityCategory = 0;
		
		
		public Cod (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Cow : Entity {
		public const int EntityId = 14;
		public const string EntityName = " cow";
		public const string EntityDisplayName = "Cow";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 1.4F;
		public const int EntityCategory = 0;
		
		
		public Cow (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Creeper : Entity {
		public const int EntityId = 15;
		public const string EntityName = " creeper";
		public const string EntityDisplayName = "Creeper";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.7F;
		public const int EntityCategory = 4;
		
		
		public Creeper (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Dolphin : Entity {
		public const int EntityId = 16;
		public const string EntityName = " dolphin";
		public const string EntityDisplayName = "Dolphin";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 0.6F;
		public const int EntityCategory = 0;
		
		
		public Dolphin (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Donkey : Entity {
		public const int EntityId = 17;
		public const string EntityName = " donkey";
		public const string EntityDisplayName = "Donkey";
		
		public const float EntityWidth = 1.3964844F;
		public const float EntityHeight = 1.5F;
		public const int EntityCategory = 0;
		
		
		public Donkey (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class DragonFireball : Entity {
		public const int EntityId = 18;
		public const string EntityName = " dragon_fireball";
		public const string EntityDisplayName = "Dragon Fireball";
		
		public const float EntityWidth = 1F;
		public const float EntityHeight = 1F;
		public const int EntityCategory = 3;
		
		
		public DragonFireball (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Drowned : Entity {
		public const int EntityId = 19;
		public const string EntityName = " drowned";
		public const string EntityDisplayName = "Drowned";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public Drowned (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ElderGuardian : Entity {
		public const int EntityId = 20;
		public const string EntityName = " elder_guardian";
		public const string EntityDisplayName = "Elder Guardian";
		
		public const float EntityWidth = 1.9975F;
		public const float EntityHeight = 1.9975F;
		public const int EntityCategory = 4;
		
		
		public ElderGuardian (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class EndCrystal : Entity {
		public const int EntityId = 21;
		public const string EntityName = " end_crystal";
		public const string EntityDisplayName = "End Crystal";
		
		public const float EntityWidth = 2F;
		public const float EntityHeight = 2F;
		public const int EntityCategory = 2;
		
		
		public EndCrystal (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class EnderDragon : Entity {
		public const int EntityId = 22;
		public const string EntityName = " ender_dragon";
		public const string EntityDisplayName = "Ender Dragon";
		
		public const float EntityWidth = 16F;
		public const float EntityHeight = 8F;
		public const int EntityCategory = 4;
		
		
		public EnderDragon (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Enderman : Entity {
		public const int EntityId = 23;
		public const string EntityName = " enderman";
		public const string EntityDisplayName = "Enderman";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 2.9F;
		public const int EntityCategory = 4;
		
		
		public Enderman (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Endermite : Entity {
		public const int EntityId = 24;
		public const string EntityName = " endermite";
		public const string EntityDisplayName = "Endermite";
		
		public const float EntityWidth = 0.4F;
		public const float EntityHeight = 0.3F;
		public const int EntityCategory = 4;
		
		
		public Endermite (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Evoker : Entity {
		public const int EntityId = 25;
		public const string EntityName = " evoker";
		public const string EntityDisplayName = "Evoker";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public Evoker (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class EvokerFangs : Entity {
		public const int EntityId = 26;
		public const string EntityName = " evoker_fangs";
		public const string EntityDisplayName = "Evoker Fangs";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.8F;
		public const int EntityCategory = 4;
		
		
		public EvokerFangs (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ExperienceOrb : Entity {
		public const int EntityId = 27;
		public const string EntityName = " experience_orb";
		public const string EntityDisplayName = "Experience Orb";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 1;
		
		
		public ExperienceOrb (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class EyeOfEnder : Entity {
		public const int EntityId = 28;
		public const string EntityName = " eye_of_ender";
		public const string EntityDisplayName = "Eye of Ender";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 1;
		
		
		public EyeOfEnder (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class FallingBlock : Entity {
		public const int EntityId = 29;
		public const string EntityName = " falling_block";
		public const string EntityDisplayName = "Falling Block";
		
		public const float EntityWidth = 0.98F;
		public const float EntityHeight = 0.98F;
		public const int EntityCategory = 1;
		
		
		public FallingBlock (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class FireworkRocket : Entity {
		public const int EntityId = 30;
		public const string EntityName = " firework_rocket";
		public const string EntityDisplayName = "Firework Rocket";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 3;
		
		
		public FireworkRocket (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Fox : Entity {
		public const int EntityId = 31;
		public const string EntityName = " fox";
		public const string EntityDisplayName = "Fox";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 0;
		
		
		public Fox (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Frog : Entity {
		public const int EntityId = 32;
		public const string EntityName = " frog";
		public const string EntityDisplayName = "Frog";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 0;
		
		
		public Frog (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Ghast : Entity {
		public const int EntityId = 33;
		public const string EntityName = " ghast";
		public const string EntityDisplayName = "Ghast";
		
		public const float EntityWidth = 4F;
		public const float EntityHeight = 4F;
		public const int EntityCategory = 4;
		
		
		public Ghast (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Giant : Entity {
		public const int EntityId = 34;
		public const string EntityName = " giant";
		public const string EntityDisplayName = "Giant";
		
		public const float EntityWidth = 3.6F;
		public const float EntityHeight = 12F;
		public const int EntityCategory = 4;
		
		
		public Giant (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class GlowItemFrame : Entity {
		public const int EntityId = 35;
		public const string EntityName = " glow_item_frame";
		public const string EntityDisplayName = "Glow Item Frame";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 2;
		
		
		public GlowItemFrame (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class GlowSquid : Entity {
		public const int EntityId = 36;
		public const string EntityName = " glow_squid";
		public const string EntityDisplayName = "Glow Squid";
		
		public const float EntityWidth = 0.8F;
		public const float EntityHeight = 0.8F;
		public const int EntityCategory = 0;
		
		
		public GlowSquid (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Goat : Entity {
		public const int EntityId = 37;
		public const string EntityName = " goat";
		public const string EntityDisplayName = "Goat";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 1.3F;
		public const int EntityCategory = 0;
		
		
		public Goat (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Guardian : Entity {
		public const int EntityId = 38;
		public const string EntityName = " guardian";
		public const string EntityDisplayName = "Guardian";
		
		public const float EntityWidth = 0.85F;
		public const float EntityHeight = 0.85F;
		public const int EntityCategory = 4;
		
		
		public Guardian (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Hoglin : Entity {
		public const int EntityId = 39;
		public const string EntityName = " hoglin";
		public const string EntityDisplayName = "Hoglin";
		
		public const float EntityWidth = 1.3964844F;
		public const float EntityHeight = 1.4F;
		public const int EntityCategory = 4;
		
		
		public Hoglin (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Horse : Entity {
		public const int EntityId = 40;
		public const string EntityName = " horse";
		public const string EntityDisplayName = "Horse";
		
		public const float EntityWidth = 1.3964844F;
		public const float EntityHeight = 1.6F;
		public const int EntityCategory = 0;
		
		
		public Horse (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Husk : Entity {
		public const int EntityId = 41;
		public const string EntityName = " husk";
		public const string EntityDisplayName = "Husk";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public Husk (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Illusioner : Entity {
		public const int EntityId = 42;
		public const string EntityName = " illusioner";
		public const string EntityDisplayName = "Illusioner";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public Illusioner (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class IronGolem : Entity {
		public const int EntityId = 43;
		public const string EntityName = " iron_golem";
		public const string EntityDisplayName = "Iron Golem";
		
		public const float EntityWidth = 1.4F;
		public const float EntityHeight = 2.7F;
		public const int EntityCategory = 0;
		
		
		public IronGolem (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Item : Entity {
		public const int EntityId = 44;
		public const string EntityName = " item";
		public const string EntityDisplayName = "Item";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 1;
		
		
		public Item (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ItemFrame : Entity {
		public const int EntityId = 45;
		public const string EntityName = " item_frame";
		public const string EntityDisplayName = "Item Frame";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 2;
		
		
		public ItemFrame (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Fireball : Entity {
		public const int EntityId = 46;
		public const string EntityName = " fireball";
		public const string EntityDisplayName = "Fireball";
		
		public const float EntityWidth = 1F;
		public const float EntityHeight = 1F;
		public const int EntityCategory = 3;
		
		
		public Fireball (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class LeashKnot : Entity {
		public const int EntityId = 47;
		public const string EntityName = " leash_knot";
		public const string EntityDisplayName = "Leash Knot";
		
		public const float EntityWidth = 0.375F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 2;
		
		
		public LeashKnot (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class LightningBolt : Entity {
		public const int EntityId = 48;
		public const string EntityName = " lightning_bolt";
		public const string EntityDisplayName = "Lightning Bolt";
		
		public const float EntityWidth = 0F;
		public const float EntityHeight = 0F;
		public const int EntityCategory = 1;
		
		
		public LightningBolt (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Llama : Entity {
		public const int EntityId = 49;
		public const string EntityName = " llama";
		public const string EntityDisplayName = "Llama";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 1.87F;
		public const int EntityCategory = 0;
		
		
		public Llama (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class LlamaSpit : Entity {
		public const int EntityId = 50;
		public const string EntityName = " llama_spit";
		public const string EntityDisplayName = "Llama Spit";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 3;
		
		
		public LlamaSpit (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class MagmaCube : Entity {
		public const int EntityId = 51;
		public const string EntityName = " magma_cube";
		public const string EntityDisplayName = "Magma Cube";
		
		public const float EntityWidth = 2.04F;
		public const float EntityHeight = 2.04F;
		public const int EntityCategory = 4;
		
		
		public MagmaCube (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Marker : Entity {
		public const int EntityId = 52;
		public const string EntityName = " marker";
		public const string EntityDisplayName = "Marker";
		
		public const float EntityWidth = 0F;
		public const float EntityHeight = 0F;
		public const int EntityCategory = 1;
		
		
		public Marker (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Minecart : Entity {
		public const int EntityId = 53;
		public const string EntityName = " minecart";
		public const string EntityDisplayName = "Minecart";
		
		public const float EntityWidth = 0.98F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 5;
		
		
		public Minecart (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ChestMinecart : Entity {
		public const int EntityId = 54;
		public const string EntityName = " chest_minecart";
		public const string EntityDisplayName = "Minecart with Chest";
		
		public const float EntityWidth = 0.98F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 5;
		
		
		public ChestMinecart (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class CommandBlockMinecart : Entity {
		public const int EntityId = 55;
		public const string EntityName = " command_block_minecart";
		public const string EntityDisplayName = "Minecart with Command Block";
		
		public const float EntityWidth = 0.98F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 5;
		
		
		public CommandBlockMinecart (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class FurnaceMinecart : Entity {
		public const int EntityId = 56;
		public const string EntityName = " furnace_minecart";
		public const string EntityDisplayName = "Minecart with Furnace";
		
		public const float EntityWidth = 0.98F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 5;
		
		
		public FurnaceMinecart (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class HopperMinecart : Entity {
		public const int EntityId = 57;
		public const string EntityName = " hopper_minecart";
		public const string EntityDisplayName = "Minecart with Hopper";
		
		public const float EntityWidth = 0.98F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 5;
		
		
		public HopperMinecart (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class SpawnerMinecart : Entity {
		public const int EntityId = 58;
		public const string EntityName = " spawner_minecart";
		public const string EntityDisplayName = "Minecart with Spawner";
		
		public const float EntityWidth = 0.98F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 5;
		
		
		public SpawnerMinecart (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class TntMinecart : Entity {
		public const int EntityId = 59;
		public const string EntityName = " tnt_minecart";
		public const string EntityDisplayName = "Minecart with TNT";
		
		public const float EntityWidth = 0.98F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 5;
		
		
		public TntMinecart (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Mule : Entity {
		public const int EntityId = 60;
		public const string EntityName = " mule";
		public const string EntityDisplayName = "Mule";
		
		public const float EntityWidth = 1.3964844F;
		public const float EntityHeight = 1.6F;
		public const int EntityCategory = 0;
		
		
		public Mule (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Mooshroom : Entity {
		public const int EntityId = 61;
		public const string EntityName = " mooshroom";
		public const string EntityDisplayName = "Mooshroom";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 1.4F;
		public const int EntityCategory = 0;
		
		
		public Mooshroom (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Ocelot : Entity {
		public const int EntityId = 62;
		public const string EntityName = " ocelot";
		public const string EntityDisplayName = "Ocelot";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 0;
		
		
		public Ocelot (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Painting : Entity {
		public const int EntityId = 63;
		public const string EntityName = " painting";
		public const string EntityDisplayName = "Painting";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 2;
		
		
		public Painting (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Panda : Entity {
		public const int EntityId = 64;
		public const string EntityName = " panda";
		public const string EntityDisplayName = "Panda";
		
		public const float EntityWidth = 1.3F;
		public const float EntityHeight = 1.25F;
		public const int EntityCategory = 0;
		
		
		public Panda (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Parrot : Entity {
		public const int EntityId = 65;
		public const string EntityName = " parrot";
		public const string EntityDisplayName = "Parrot";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.9F;
		public const int EntityCategory = 0;
		
		
		public Parrot (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Phantom : Entity {
		public const int EntityId = 66;
		public const string EntityName = " phantom";
		public const string EntityDisplayName = "Phantom";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 4;
		
		
		public Phantom (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Pig : Entity {
		public const int EntityId = 67;
		public const string EntityName = " pig";
		public const string EntityDisplayName = "Pig";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 0.9F;
		public const int EntityCategory = 0;
		
		
		public Pig (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Piglin : Entity {
		public const int EntityId = 68;
		public const string EntityName = " piglin";
		public const string EntityDisplayName = "Piglin";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public Piglin (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class PiglinBrute : Entity {
		public const int EntityId = 69;
		public const string EntityName = " piglin_brute";
		public const string EntityDisplayName = "Piglin Brute";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public PiglinBrute (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Pillager : Entity {
		public const int EntityId = 70;
		public const string EntityName = " pillager";
		public const string EntityDisplayName = "Pillager";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public Pillager (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class PolarBear : Entity {
		public const int EntityId = 71;
		public const string EntityName = " polar_bear";
		public const string EntityDisplayName = "Polar Bear";
		
		public const float EntityWidth = 1.4F;
		public const float EntityHeight = 1.4F;
		public const int EntityCategory = 0;
		
		
		public PolarBear (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Tnt : Entity {
		public const int EntityId = 72;
		public const string EntityName = " tnt";
		public const string EntityDisplayName = "Primed TNT";
		
		public const float EntityWidth = 0.98F;
		public const float EntityHeight = 0.98F;
		public const int EntityCategory = 1;
		
		
		public Tnt (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Pufferfish : Entity {
		public const int EntityId = 73;
		public const string EntityName = " pufferfish";
		public const string EntityDisplayName = "Pufferfish";
		
		public const float EntityWidth = 0.7F;
		public const float EntityHeight = 0.7F;
		public const int EntityCategory = 0;
		
		
		public Pufferfish (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Rabbit : Entity {
		public const int EntityId = 74;
		public const string EntityName = " rabbit";
		public const string EntityDisplayName = "Rabbit";
		
		public const float EntityWidth = 0.4F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 0;
		
		
		public Rabbit (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Ravager : Entity {
		public const int EntityId = 75;
		public const string EntityName = " ravager";
		public const string EntityDisplayName = "Ravager";
		
		public const float EntityWidth = 1.95F;
		public const float EntityHeight = 2.2F;
		public const int EntityCategory = 4;
		
		
		public Ravager (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Salmon : Entity {
		public const int EntityId = 76;
		public const string EntityName = " salmon";
		public const string EntityDisplayName = "Salmon";
		
		public const float EntityWidth = 0.7F;
		public const float EntityHeight = 0.4F;
		public const int EntityCategory = 0;
		
		
		public Salmon (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Sheep : Entity {
		public const int EntityId = 77;
		public const string EntityName = " sheep";
		public const string EntityDisplayName = "Sheep";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 1.3F;
		public const int EntityCategory = 0;
		
		
		public Sheep (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Shulker : Entity {
		public const int EntityId = 78;
		public const string EntityName = " shulker";
		public const string EntityDisplayName = "Shulker";
		
		public const float EntityWidth = 1F;
		public const float EntityHeight = 1F;
		public const int EntityCategory = 4;
		
		
		public Shulker (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ShulkerBullet : Entity {
		public const int EntityId = 79;
		public const string EntityName = " shulker_bullet";
		public const string EntityDisplayName = "Shulker Bullet";
		
		public const float EntityWidth = 0.3125F;
		public const float EntityHeight = 0.3125F;
		public const int EntityCategory = 3;
		
		
		public ShulkerBullet (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Silverfish : Entity {
		public const int EntityId = 80;
		public const string EntityName = " silverfish";
		public const string EntityDisplayName = "Silverfish";
		
		public const float EntityWidth = 0.4F;
		public const float EntityHeight = 0.3F;
		public const int EntityCategory = 4;
		
		
		public Silverfish (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Skeleton : Entity {
		public const int EntityId = 81;
		public const string EntityName = " skeleton";
		public const string EntityDisplayName = "Skeleton";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.99F;
		public const int EntityCategory = 4;
		
		
		public Skeleton (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class SkeletonHorse : Entity {
		public const int EntityId = 82;
		public const string EntityName = " skeleton_horse";
		public const string EntityDisplayName = "Skeleton Horse";
		
		public const float EntityWidth = 1.3964844F;
		public const float EntityHeight = 1.6F;
		public const int EntityCategory = 4;
		
		
		public SkeletonHorse (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Slime : Entity {
		public const int EntityId = 83;
		public const string EntityName = " slime";
		public const string EntityDisplayName = "Slime";
		
		public const float EntityWidth = 2.04F;
		public const float EntityHeight = 2.04F;
		public const int EntityCategory = 4;
		
		
		public Slime (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class SmallFireball : Entity {
		public const int EntityId = 84;
		public const string EntityName = " small_fireball";
		public const string EntityDisplayName = "Small Fireball";
		
		public const float EntityWidth = 0.3125F;
		public const float EntityHeight = 0.3125F;
		public const int EntityCategory = 3;
		
		
		public SmallFireball (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class SnowGolem : Entity {
		public const int EntityId = 85;
		public const string EntityName = " snow_golem";
		public const string EntityDisplayName = "Snow Golem";
		
		public const float EntityWidth = 0.7F;
		public const float EntityHeight = 1.9F;
		public const int EntityCategory = 0;
		
		
		public SnowGolem (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Snowball : Entity {
		public const int EntityId = 86;
		public const string EntityName = " snowball";
		public const string EntityDisplayName = "Snowball";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 3;
		
		
		public Snowball (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class SpectralArrow : Entity {
		public const int EntityId = 87;
		public const string EntityName = " spectral_arrow";
		public const string EntityDisplayName = "Spectral Arrow";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 3;
		
		
		public SpectralArrow (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Spider : Entity {
		public const int EntityId = 88;
		public const string EntityName = " spider";
		public const string EntityDisplayName = "Spider";
		
		public const float EntityWidth = 1.4F;
		public const float EntityHeight = 0.9F;
		public const int EntityCategory = 4;
		
		
		public Spider (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Squid : Entity {
		public const int EntityId = 89;
		public const string EntityName = " squid";
		public const string EntityDisplayName = "Squid";
		
		public const float EntityWidth = 0.8F;
		public const float EntityHeight = 0.8F;
		public const int EntityCategory = 0;
		
		
		public Squid (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Stray : Entity {
		public const int EntityId = 90;
		public const string EntityName = " stray";
		public const string EntityDisplayName = "Stray";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.99F;
		public const int EntityCategory = 4;
		
		
		public Stray (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Strider : Entity {
		public const int EntityId = 91;
		public const string EntityName = " strider";
		public const string EntityDisplayName = "Strider";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 1.7F;
		public const int EntityCategory = 0;
		
		
		public Strider (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Tadpole : Entity {
		public const int EntityId = 92;
		public const string EntityName = " tadpole";
		public const string EntityDisplayName = "Tadpole";
		
		public const float EntityWidth = 0.4F;
		public const float EntityHeight = 0.3F;
		public const int EntityCategory = 0;
		
		
		public Tadpole (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Egg : Entity {
		public const int EntityId = 93;
		public const string EntityName = " egg";
		public const string EntityDisplayName = "Thrown Egg";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 3;
		
		
		public Egg (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class EnderPearl : Entity {
		public const int EntityId = 94;
		public const string EntityName = " ender_pearl";
		public const string EntityDisplayName = "Thrown Ender Pearl";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 3;
		
		
		public EnderPearl (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ExperienceBottle : Entity {
		public const int EntityId = 95;
		public const string EntityName = " experience_bottle";
		public const string EntityDisplayName = "Thrown Bottle o' Enchanting";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 3;
		
		
		public ExperienceBottle (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Potion : Entity {
		public const int EntityId = 96;
		public const string EntityName = " potion";
		public const string EntityDisplayName = "Potion";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 3;
		
		
		public Potion (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Trident : Entity {
		public const int EntityId = 97;
		public const string EntityName = " trident";
		public const string EntityDisplayName = "Trident";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.5F;
		public const int EntityCategory = 3;
		
		
		public Trident (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class TraderLlama : Entity {
		public const int EntityId = 98;
		public const string EntityName = " trader_llama";
		public const string EntityDisplayName = "Trader Llama";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 1.87F;
		public const int EntityCategory = 0;
		
		
		public TraderLlama (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class TropicalFish : Entity {
		public const int EntityId = 99;
		public const string EntityName = " tropical_fish";
		public const string EntityDisplayName = "Tropical Fish";
		
		public const float EntityWidth = 0.5F;
		public const float EntityHeight = 0.4F;
		public const int EntityCategory = 0;
		
		
		public TropicalFish (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Turtle : Entity {
		public const int EntityId = 100;
		public const string EntityName = " turtle";
		public const string EntityDisplayName = "Turtle";
		
		public const float EntityWidth = 1.2F;
		public const float EntityHeight = 0.4F;
		public const int EntityCategory = 0;
		
		
		public Turtle (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Vex : Entity {
		public const int EntityId = 101;
		public const string EntityName = " vex";
		public const string EntityDisplayName = "Vex";
		
		public const float EntityWidth = 0.4F;
		public const float EntityHeight = 0.8F;
		public const int EntityCategory = 4;
		
		
		public Vex (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Villager : Entity {
		public const int EntityId = 102;
		public const string EntityName = " villager";
		public const string EntityDisplayName = "Villager";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 0;
		
		
		public Villager (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Vindicator : Entity {
		public const int EntityId = 103;
		public const string EntityName = " vindicator";
		public const string EntityDisplayName = "Vindicator";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public Vindicator (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class WanderingTrader : Entity {
		public const int EntityId = 104;
		public const string EntityName = " wandering_trader";
		public const string EntityDisplayName = "Wandering Trader";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 0;
		
		
		public WanderingTrader (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Warden : Entity {
		public const int EntityId = 105;
		public const string EntityName = " warden";
		public const string EntityDisplayName = "Warden";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 2.9F;
		public const int EntityCategory = 4;
		
		
		public Warden (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Witch : Entity {
		public const int EntityId = 106;
		public const string EntityName = " witch";
		public const string EntityDisplayName = "Witch";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public Witch (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Wither : Entity {
		public const int EntityId = 107;
		public const string EntityName = " wither";
		public const string EntityDisplayName = "Wither";
		
		public const float EntityWidth = 0.9F;
		public const float EntityHeight = 3.5F;
		public const int EntityCategory = 4;
		
		
		public Wither (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class WitherSkeleton : Entity {
		public const int EntityId = 108;
		public const string EntityName = " wither_skeleton";
		public const string EntityDisplayName = "Wither Skeleton";
		
		public const float EntityWidth = 0.7F;
		public const float EntityHeight = 2.4F;
		public const int EntityCategory = 4;
		
		
		public WitherSkeleton (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class WitherSkull : Entity {
		public const int EntityId = 109;
		public const string EntityName = " wither_skull";
		public const string EntityDisplayName = "Wither Skull";
		
		public const float EntityWidth = 0.3125F;
		public const float EntityHeight = 0.3125F;
		public const int EntityCategory = 3;
		
		
		public WitherSkull (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Wolf : Entity {
		public const int EntityId = 110;
		public const string EntityName = " wolf";
		public const string EntityDisplayName = "Wolf";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 0.85F;
		public const int EntityCategory = 0;
		
		
		public Wolf (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Zoglin : Entity {
		public const int EntityId = 111;
		public const string EntityName = " zoglin";
		public const string EntityDisplayName = "Zoglin";
		
		public const float EntityWidth = 1.3964844F;
		public const float EntityHeight = 1.4F;
		public const int EntityCategory = 4;
		
		
		public Zoglin (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Zombie : Entity {
		public const int EntityId = 112;
		public const string EntityName = " zombie";
		public const string EntityDisplayName = "Zombie";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public Zombie (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ZombieHorse : Entity {
		public const int EntityId = 113;
		public const string EntityName = " zombie_horse";
		public const string EntityDisplayName = "Zombie Horse";
		
		public const float EntityWidth = 1.3964844F;
		public const float EntityHeight = 1.6F;
		public const int EntityCategory = 4;
		
		
		public ZombieHorse (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ZombieVillager : Entity {
		public const int EntityId = 114;
		public const string EntityName = " zombie_villager";
		public const string EntityDisplayName = "Zombie Villager";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public ZombieVillager (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class ZombifiedPiglin : Entity {
		public const int EntityId = 115;
		public const string EntityName = " zombified_piglin";
		public const string EntityDisplayName = "Zombified Piglin";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.95F;
		public const int EntityCategory = 4;
		
		
		public ZombifiedPiglin (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class Player : Entity {
		public const int EntityId = 116;
		public const string EntityName = " player";
		public const string EntityDisplayName = "Player";
		
		public const float EntityWidth = 0.6F;
		public const float EntityHeight = 1.8F;
		public const int EntityCategory = 1;
		
		
		public Player (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public class FishingBobber : Entity {
		public const int EntityId = 117;
		public const string EntityName = " fishing_bobber";
		public const string EntityDisplayName = "Fishing Bobber";
		
		public const float EntityWidth = 0.25F;
		public const float EntityHeight = 0.25F;
		public const int EntityCategory = 3;
		
		
		public FishingBobber (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {}
	}
	public enum EntityType {
		Allay = 0,
		AreaEffectCloud = 1,
		ArmorStand = 2,
		Arrow = 3,
		Axolotl = 4,
		Bat = 5,
		Bee = 6,
		Blaze = 7,
		Boat = 8,
		ChestBoat = 9,
		Cat = 10,
		CaveSpider = 11,
		Chicken = 12,
		Cod = 13,
		Cow = 14,
		Creeper = 15,
		Dolphin = 16,
		Donkey = 17,
		DragonFireball = 18,
		Drowned = 19,
		ElderGuardian = 20,
		EndCrystal = 21,
		EnderDragon = 22,
		Enderman = 23,
		Endermite = 24,
		Evoker = 25,
		EvokerFangs = 26,
		ExperienceOrb = 27,
		EyeOfEnder = 28,
		FallingBlock = 29,
		FireworkRocket = 30,
		Fox = 31,
		Frog = 32,
		Ghast = 33,
		Giant = 34,
		GlowItemFrame = 35,
		GlowSquid = 36,
		Goat = 37,
		Guardian = 38,
		Hoglin = 39,
		Horse = 40,
		Husk = 41,
		Illusioner = 42,
		IronGolem = 43,
		Item = 44,
		ItemFrame = 45,
		Fireball = 46,
		LeashKnot = 47,
		LightningBolt = 48,
		Llama = 49,
		LlamaSpit = 50,
		MagmaCube = 51,
		Marker = 52,
		Minecart = 53,
		ChestMinecart = 54,
		CommandBlockMinecart = 55,
		FurnaceMinecart = 56,
		HopperMinecart = 57,
		SpawnerMinecart = 58,
		TntMinecart = 59,
		Mule = 60,
		Mooshroom = 61,
		Ocelot = 62,
		Painting = 63,
		Panda = 64,
		Parrot = 65,
		Phantom = 66,
		Pig = 67,
		Piglin = 68,
		PiglinBrute = 69,
		Pillager = 70,
		PolarBear = 71,
		Tnt = 72,
		Pufferfish = 73,
		Rabbit = 74,
		Ravager = 75,
		Salmon = 76,
		Sheep = 77,
		Shulker = 78,
		ShulkerBullet = 79,
		Silverfish = 80,
		Skeleton = 81,
		SkeletonHorse = 82,
		Slime = 83,
		SmallFireball = 84,
		SnowGolem = 85,
		Snowball = 86,
		SpectralArrow = 87,
		Spider = 88,
		Squid = 89,
		Stray = 90,
		Strider = 91,
		Tadpole = 92,
		Egg = 93,
		EnderPearl = 94,
		ExperienceBottle = 95,
		Potion = 96,
		Trident = 97,
		TraderLlama = 98,
		TropicalFish = 99,
		Turtle = 100,
		Vex = 101,
		Villager = 102,
		Vindicator = 103,
		WanderingTrader = 104,
		Warden = 105,
		Witch = 106,
		Wither = 107,
		WitherSkeleton = 108,
		WitherSkull = 109,
		Wolf = 110,
		Zoglin = 111,
		Zombie = 112,
		ZombieHorse = 113,
		ZombieVillager = 114,
		ZombifiedPiglin = 115,
		Player = 116,
		FishingBobber = 117,
	}
}
