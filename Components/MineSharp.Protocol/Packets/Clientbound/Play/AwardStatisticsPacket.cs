using System.Collections.Frozen;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Award statistics packet sent as a response to Client Command.
/// </summary>
/// <param name="Count">Number of elements in the statistics array.</param>
/// <param name="Statistics">Array of statistics.</param>
public sealed record AwardStatisticsPacket(int Count, AwardStatisticsPacket.Statistic[] Statistics) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Statistics;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(Count);
        foreach (var statistic in Statistics)
        {
            buffer.WriteVarInt((int)statistic.CategoryId);
            buffer.WriteVarInt((int)statistic.StatisticId);
            buffer.WriteVarInt(statistic.Value);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var count = buffer.ReadVarInt();
        var statistics = new Statistic[count];
        for (int i = 0; i < count; i++)
        {
            var categoryId = (CategoryId)buffer.ReadVarInt();
            var statisticId = (StatisticId)buffer.ReadVarInt();
            var value = buffer.ReadVarInt();
            statistics[i] = new Statistic(categoryId, statisticId, value);
        }

        return new AwardStatisticsPacket(count, statistics);
    }

    /// <summary>
    ///     Represents a single statistic entry.
    /// </summary>
    /// <param name="CategoryId">The category ID of the statistic.</param>
    /// <param name="StatisticId">The statistic ID.</param>
    /// <param name="Value">The value of the statistic.</param>
    public sealed record Statistic(CategoryId CategoryId, StatisticId StatisticId, int Value);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    ///     Enum representing different types of statistic IDs.
    /// </summary>
    public enum StatisticId
    {
        LeaveGame = 0,
        PlayOneMinute = 1,
        TimeSinceDeath = 2,
        TimeSinceRest = 3,
        SneakTime = 4,
        WalkOneCm = 5,
        CrouchOneCm = 6,
        SprintOneCm = 7,
        WalkOnWaterOneCm = 8,
        FallOneCm = 9,
        ClimbOneCm = 10,
        FlyOneCm = 11,
        WalkUnderWaterOneCm = 12,
        MinecartOneCm = 13,
        BoatOneCm = 14,
        PigOneCm = 15,
        HorseOneCm = 16,
        AviateOneCm = 17,
        SwimOneCm = 18,
        StriderOneCm = 19,
        Jump = 20,
        Drop = 21,
        DamageDealt = 22,
        DamageDealtAbsorbed = 23,
        DamageDealtResisted = 24,
        DamageTaken = 25,
        DamageBlockedByShield = 26,
        DamageAbsorbed = 27,
        DamageResisted = 28,
        Deaths = 29,
        MobKills = 30,
        AnimalsBred = 31,
        PlayerKills = 32,
        FishCaught = 33,
        TalkedToVillager = 34,
        TradedWithVillager = 35,
        EatCakeSlice = 36,
        FillCauldron = 37,
        UseCauldron = 38,
        CleanArmor = 39,
        CleanBanner = 40,
        CleanShulkerBox = 41,
        InteractWithBrewingStand = 42,
        InteractWithBeacon = 43,
        InspectDropper = 44,
        InspectHopper = 45,
        InspectDispenser = 46,
        PlayNoteblock = 47,
        TuneNoteblock = 48,
        PotFlower = 49,
        TriggerTrappedChest = 50,
        OpenEnderchest = 51,
        EnchantItem = 52,
        PlayRecord = 53,
        InteractWithFurnace = 54,
        InteractWithCraftingTable = 55,
        OpenChest = 56,
        SleepInBed = 57,
        OpenShulkerBox = 58,
        OpenBarrel = 59,
        InteractWithBlastFurnace = 60,
        InteractWithSmoker = 61,
        InteractWithLectern = 62,
        InteractWithCampfire = 63,
        InteractWithCartographyTable = 64,
        InteractWithLoom = 65,
        InteractWithStonecutter = 66,
        BellRing = 67,
        RaidTrigger = 68,
        RaidWin = 69,
        InteractWithAnvil = 70,
        InteractWithGrindstone = 71,
        TargetHit = 72,
        InteractWithSmithingTable = 73
    }

    /// <summary>
    ///     Enum representing different categories of statistics.
    /// </summary>
    public enum CategoryId
    {
        General = 0,
        Blocks = 1,
        Items = 2,
        Mobs = 3
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    ///     Enum representing different units for statistics.
    /// </summary>
    public enum Unit
    {
        /// <summary>
        ///     No unit.
        ///     Just a normal number (formatted with 0 decimal places).
        /// </summary>
        None,
        /// <summary>
        ///     Unit representing damage.
        ///     Value is 10 times the normal amount.
        /// </summary>
        Damage,
        /// <summary>
        ///     Unit representing distance.
        ///     A distance in centimeters (hundredths of blocks).
        /// </summary>
        Distance,
        /// <summary>
        ///     Unit representing time.
        ///     A time span in ticks
        /// </summary>
        Time
    }

    // TODO: Is this data from a registry? Hardcoded for now.
    /// <summary>
    ///     Lookup table for statistic ID to name and unit.
    /// </summary>
    public static readonly FrozenDictionary<StatisticId, (string Name, Unit Unit)> StatisticLookup = new Dictionary<StatisticId, (string Name, Unit Unit)>()
    {
        { StatisticId.LeaveGame, ("minecraft.leave_game", Unit.None) },
        { StatisticId.PlayOneMinute, ("minecraft.play_one_minute", Unit.Time) },
        { StatisticId.TimeSinceDeath, ("minecraft.time_since_death", Unit.Time) },
        { StatisticId.TimeSinceRest, ("minecraft.time_since_rest", Unit.Time) },
        { StatisticId.SneakTime, ("minecraft.sneak_time", Unit.Time) },
        { StatisticId.WalkOneCm, ("minecraft.walk_one_cm", Unit.Distance) },
        { StatisticId.CrouchOneCm, ("minecraft.crouch_one_cm", Unit.Distance) },
        { StatisticId.SprintOneCm, ("minecraft.sprint_one_cm", Unit.Distance) },
        { StatisticId.WalkOnWaterOneCm, ("minecraft.walk_on_water_one_cm", Unit.Distance) },
        { StatisticId.FallOneCm, ("minecraft.fall_one_cm", Unit.Distance) },
        { StatisticId.ClimbOneCm, ("minecraft.climb_one_cm", Unit.Distance) },
        { StatisticId.FlyOneCm, ("minecraft.fly_one_cm", Unit.Distance) },
        { StatisticId.WalkUnderWaterOneCm, ("minecraft.walk_under_water_one_cm", Unit.Distance) },
        { StatisticId.MinecartOneCm, ("minecraft.minecart_one_cm", Unit.Distance) },
        { StatisticId.BoatOneCm, ("minecraft.boat_one_cm", Unit.Distance) },
        { StatisticId.PigOneCm, ("minecraft.pig_one_cm", Unit.Distance) },
        { StatisticId.HorseOneCm, ("minecraft.horse_one_cm", Unit.Distance) },
        { StatisticId.AviateOneCm, ("minecraft.aviate_one_cm", Unit.Distance) },
        { StatisticId.SwimOneCm, ("minecraft.swim_one_cm", Unit.Distance) },
        { StatisticId.StriderOneCm, ("minecraft.strider_one_cm", Unit.Distance) },
        { StatisticId.Jump, ("minecraft.jump", Unit.None) },
        { StatisticId.Drop, ("minecraft.drop", Unit.None) },
        { StatisticId.DamageDealt, ("minecraft.damage_dealt", Unit.Damage) },
        { StatisticId.DamageDealtAbsorbed, ("minecraft.damage_dealt_absorbed", Unit.Damage) },
        { StatisticId.DamageDealtResisted, ("minecraft.damage_dealt_resisted", Unit.Damage) },
        { StatisticId.DamageTaken, ("minecraft.damage_taken", Unit.Damage) },
        { StatisticId.DamageBlockedByShield, ("minecraft.damage_blocked_by_shield", Unit.Damage) },
        { StatisticId.DamageAbsorbed, ("minecraft.damage_absorbed", Unit.Damage) },
        { StatisticId.DamageResisted, ("minecraft.damage_resisted", Unit.Damage) },
        { StatisticId.Deaths, ("minecraft.deaths", Unit.None) },
        { StatisticId.MobKills, ("minecraft.mob_kills", Unit.None) },
        { StatisticId.AnimalsBred, ("minecraft.animals_bred", Unit.None) },
        { StatisticId.PlayerKills, ("minecraft.player_kills", Unit.None) },
        { StatisticId.FishCaught, ("minecraft.fish_caught", Unit.None) },
        { StatisticId.TalkedToVillager, ("minecraft.talked_to_villager", Unit.None) },
        { StatisticId.TradedWithVillager, ("minecraft.traded_with_villager", Unit.None) },
        { StatisticId.EatCakeSlice, ("minecraft.eat_cake_slice", Unit.None) },
        { StatisticId.FillCauldron, ("minecraft.fill_cauldron", Unit.None) },
        { StatisticId.UseCauldron, ("minecraft.use_cauldron", Unit.None) },
        { StatisticId.CleanArmor, ("minecraft.clean_armor", Unit.None) },
        { StatisticId.CleanBanner, ("minecraft.clean_banner", Unit.None) },
        { StatisticId.CleanShulkerBox, ("minecraft.clean_shulker_box", Unit.None) },
        { StatisticId.InteractWithBrewingStand, ("minecraft.interact_with_brewingstand", Unit.None) },
        { StatisticId.InteractWithBeacon, ("minecraft.interact_with_beacon", Unit.None) },
        { StatisticId.InspectDropper, ("minecraft.inspect_dropper", Unit.None) },
        { StatisticId.InspectHopper, ("minecraft.inspect_hopper", Unit.None) },
        { StatisticId.InspectDispenser, ("minecraft.inspect_dispenser", Unit.None) },
        { StatisticId.PlayNoteblock, ("minecraft.play_noteblock", Unit.None) },
        { StatisticId.TuneNoteblock, ("minecraft.tune_noteblock", Unit.None) },
        { StatisticId.PotFlower, ("minecraft.pot_flower", Unit.None) },
        { StatisticId.TriggerTrappedChest, ("minecraft.trigger_trapped_chest", Unit.None) },
        { StatisticId.OpenEnderchest, ("minecraft.open_enderchest", Unit.None) },
        { StatisticId.EnchantItem, ("minecraft.enchant_item", Unit.None) },
        { StatisticId.PlayRecord, ("minecraft.play_record", Unit.None) },
        { StatisticId.InteractWithFurnace, ("minecraft.interact_with_furnace", Unit.None) },
        { StatisticId.InteractWithCraftingTable, ("minecraft.interact_with_crafting_table", Unit.None) },
        { StatisticId.OpenChest, ("minecraft.open_chest", Unit.None) },
        { StatisticId.SleepInBed, ("minecraft.sleep_in_bed", Unit.None) },
        { StatisticId.OpenShulkerBox, ("minecraft.open_shulker_box", Unit.None) },
        { StatisticId.OpenBarrel, ("minecraft.open_barrel", Unit.None) },
        { StatisticId.InteractWithBlastFurnace, ("minecraft.interact_with_blast_furnace", Unit.None) },
        { StatisticId.InteractWithSmoker, ("minecraft.interact_with_smoker", Unit.None) },
        { StatisticId.InteractWithLectern, ("minecraft.interact_with_lectern", Unit.None) },
        { StatisticId.InteractWithCampfire, ("minecraft.interact_with_campfire", Unit.None) },
        { StatisticId.InteractWithCartographyTable, ("minecraft.interact_with_cartography_table", Unit.None) },
        { StatisticId.InteractWithLoom, ("minecraft.interact_with_loom", Unit.None) },
        { StatisticId.InteractWithStonecutter, ("minecraft.interact_with_stonecutter", Unit.None) },
        { StatisticId.BellRing, ("minecraft.bell_ring", Unit.None) },
        { StatisticId.RaidTrigger, ("minecraft.raid_trigger", Unit.None) },
        { StatisticId.RaidWin, ("minecraft.raid_win", Unit.None) },
        { StatisticId.InteractWithAnvil, ("minecraft.interact_with_anvil", Unit.None) },
        { StatisticId.InteractWithGrindstone, ("minecraft.interact_with_grindstone", Unit.None) },
        { StatisticId.TargetHit, ("minecraft.target_hit", Unit.None) },
        { StatisticId.InteractWithSmithingTable, ("minecraft.interact_with_smithing_table", Unit.None) }
    }.ToFrozenDictionary();
}
