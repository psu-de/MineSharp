using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.WorldEventPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sent when a client is to play a sound or particle effect.
/// </summary>
/// <param name="Event">The event, see below.</param>
/// <param name="Location">The location of the event.</param>
/// <param name="Data">Extra data for certain events, see below.</param>
/// <param name="DisableRelativeVolume">
///     If true, the effect is played from 2 blocks away in the correct direction, ignoring distance-based volume adjustment.
/// </param>
public sealed record WorldEventPacket(EventType Event, Position Location, int Data, bool DisableRelativeVolume) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WorldEvent;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt((int)Event);
        buffer.WritePosition(Location);
        buffer.WriteInt(Data);
        buffer.WriteBool(DisableRelativeVolume);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var eventType = (EventType)buffer.ReadInt();
        var location = buffer.ReadPosition();
        var data = buffer.ReadInt();
        var disableRelativeVolume = buffer.ReadBool();

        return new WorldEventPacket(eventType, location, data, disableRelativeVolume);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    ///     Enum representing the possible events.
    /// </summary>
    public enum EventType
    {
        #region Sound Events
        DispenserDispenses = 1000,
        DispenserFailsToDispense = 1001,
        DispenserShoots = 1002,
        EnderEyeLaunched = 1003,
        FireworkShot = 1004,
        FireExtinguished = 1009,
        /// <summary>
        /// Meaning of Data field:
        /// An ID in the minecraft:item registry, corresponding to a record item.
        /// If the ID doesn't correspond to a record, the packet is ignored.
        /// Any record already being played at the given location is overwritten.
        /// See Data Generators for information on item IDs. 
        /// </summary>
        PlayRecord = 1010,
        StopRecord = 1011,
        GhastWarns = 1015,
        GhastShoots = 1016,
        EnderdragonShoots = 1017,
        BlazeShoots = 1018,
        ZombieAttacksWoodDoor = 1019,
        ZombieAttacksIronDoor = 1020,
        ZombieBreaksWoodDoor = 1021,
        WitherBreaksBlock = 1022,
        WitherSpawned = 1023,
        WitherShoots = 1024,
        BatTakesOff = 1025,
        ZombieInfects = 1026,
        ZombieVillagerConverted = 1027,
        EnderDragonDeath = 1028,
        AnvilDestroyed = 1029,
        AnvilUsed = 1030,
        AnvilLanded = 1031,
        PortalTravel = 1032,
        ChorusFlowerGrown = 1033,
        ChorusFlowerDied = 1034,
        BrewingStandBrewed = 1035,
        IronTrapdoorOpened = 1036,
        IronTrapdoorClosed = 1037,
        EndPortalCreatedInOverworld = 1038,
        PhantomBites = 1039,
        ZombieConvertsToDrowned = 1040,
        HuskConvertsToZombieByDrowning = 1041,
        GrindstoneUsed = 1042,
        BookPageTurned = 1043,
        #endregion

        #region Particle Events
        ComposterComposts = 1500,
        LavaConvertsBlock = 1501,
        RedstoneTorchBurnsOut = 1502,
        EnderEyePlaced = 1503,
        /// <summary>
        /// Meaning of Data field:
        /// See <see cref="Direction"/>
        /// </summary>
        SpawnsSmokeParticles = 2000,
        /// <summary>
        /// Meaning of Data field:
        /// Block state ID (see Chunk Format#Block state registry). 
        /// </summary>
        BlockBreak = 2001,
        /// <summary>
        /// Meaning of Data field:
        /// RGB color as an integer (e.g. 8364543 for #7FA1FF). 
        /// </summary>
        SplashPotion = 2002,
        EyeOfEnderBreak = 2003,
        MobSpawnParticleEffect = 2004,
        /// <summary>
        /// Meaning of Data field:
        /// How many particles to spawn (if set to 0, 15 are spawned). 
        /// </summary>
        BonemealParticles = 2005,
        DragonBreath = 2006,
        /// <summary>
        /// Meaning of Data field:
        /// RGB color as an integer (e.g. 8364543 for #7FA1FF). 
        /// </summary>
        InstantSplashPotion = 2007,
        EnderDragonDestroysBlock = 2008,
        WetSpongeVaporizesInNether = 2009,
        EndGatewaySpawn = 3000,
        EnderdragonGrowl = 3001,
        ElectricSpark = 3002,
        CopperApplyWax = 3003,
        CopperRemoveWax = 3004,
        CopperScrapeOxidation = 3005
        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
