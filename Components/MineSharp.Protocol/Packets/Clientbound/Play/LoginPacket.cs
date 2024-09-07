using fNbt;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
/// Represents a login packet.
/// </summary>
/// <param name="EntityId">The entity ID.</param>
/// <param name="IsHardcore">Indicates if the game is in hardcore mode.</param>
/// <param name="GameMode">The current game mode.</param>
/// <param name="PreviousGameMode">The previous game mode.</param>
/// <param name="DimensionNames">The names of the dimensions.</param>
/// <param name="RegistryCodec">The registry codec.</param>
/// <param name="DimensionType">The type of the dimension.</param>
/// <param name="DimensionName">The name of the dimension.</param>
/// <param name="HashedSeed">The hashed seed.</param>
/// <param name="MaxPlayers">The maximum number of players.</param>
/// <param name="ViewDistance">The view distance.</param>
/// <param name="SimulationDistance">The simulation distance.</param>
/// <param name="ReducedDebugInfo">Indicates if reduced debug info is enabled.</param>
/// <param name="EnableRespawnScreen">Indicates if the respawn screen is enabled.</param>
/// <param name="IsDebug">Indicates if the game is in debug mode.</param>
/// <param name="IsFlat">Indicates if the world is flat.</param>
/// <param name="HasDeathLocation">Indicates if there is a death location.</param>
/// <param name="DeathDimensionName">The name of the death dimension.</param>
/// <param name="DeathLocation">The death location.</param>
/// <param name="PortalCooldown">The portal cooldown.</param>
/// <param name="DoLimitedCrafting">Indicates if limited crafting is enabled.</param>
public sealed record LoginPacket(
    int EntityId,
    bool IsHardcore,
    byte GameMode,
    sbyte PreviousGameMode,
    Identifier[] DimensionNames,
    NbtCompound? RegistryCodec,
    Identifier DimensionType,
    Identifier DimensionName,
    long HashedSeed,
    int MaxPlayers,
    int ViewDistance,
    int SimulationDistance,
    bool ReducedDebugInfo,
    bool EnableRespawnScreen,
    bool IsDebug,
    bool IsFlat,
    bool HasDeathLocation,
    Identifier? DeathDimensionName,
    Position? DeathLocation,
    int? PortalCooldown,
    bool? DoLimitedCrafting) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Login;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol < ProtocolVersion.V_1_19)
        {
            throw new NotSupportedException(
                $"{nameof(LoginPacket)}.Write() is not supported for versions before 1.19.");
        }

        buffer.WriteInt(EntityId);
        buffer.WriteBool(IsHardcore);
        if (version.Version.Protocol < ProtocolVersion.V_1_20_2)
        {
            buffer.WriteByte(GameMode);
            buffer.WriteSByte(PreviousGameMode);
        }

        buffer.WriteVarIntArray(DimensionNames, (buf, val) => buf.WriteIdentifier(val));
        if (version.Version.Protocol < ProtocolVersion.V_1_20_2)
        {
            buffer.WriteOptionalNbt(RegistryCodec);
            buffer.WriteIdentifier(DimensionType);
            buffer.WriteIdentifier(DimensionName);
            buffer.WriteLong(HashedSeed);
        }

        buffer.WriteVarInt(MaxPlayers);
        buffer.WriteVarInt(ViewDistance);
        buffer.WriteVarInt(SimulationDistance);
        buffer.WriteBool(ReducedDebugInfo);
        buffer.WriteBool(EnableRespawnScreen);
        if (version.Version.Protocol >= ProtocolVersion.V_1_20_2)
        {
            buffer.WriteBool(DoLimitedCrafting!.Value);
            buffer.WriteIdentifier(DimensionType);
            buffer.WriteIdentifier(DimensionName);
            buffer.WriteLong(HashedSeed);
            buffer.WriteByte(GameMode);
            buffer.WriteSByte(PreviousGameMode);
        }

        buffer.WriteBool(IsDebug);
        buffer.WriteBool(IsFlat);

        buffer.WriteBool(HasDeathLocation);
        if (HasDeathLocation)
        {
            buffer.WriteIdentifier(DeathDimensionName ?? throw new InvalidOperationException($"{nameof(DeathDimensionName)} must not be null if {nameof(HasDeathLocation)} is true."));
            buffer.WritePosition(DeathLocation ?? throw new InvalidOperationException($"{nameof(DeathLocation)} must not be null if {nameof(HasDeathLocation)} is true."));
        }

        if (version.Version.Protocol >= ProtocolVersion.V_1_20)
        {
            if (PortalCooldown == null)
            {
                throw new MineSharpPacketVersionException(nameof(PortalCooldown), version.Version.Protocol);
            }

            buffer.WriteVarInt(PortalCooldown.Value);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol >= ProtocolVersion.V_1_20_2)
        {
            return ReadV1_20_2(buffer, version);
        }

        var entityId = buffer.ReadInt();
        var isHardcore = buffer.ReadBool();
        var gameMode = buffer.ReadByte();
        var previousGameMode = buffer.ReadSByte();
        var dimensionNames = buffer.ReadVarIntArray(buf => buf.ReadIdentifier());
        var registryCodec = buffer.ReadOptionalNbtCompound();
        registryCodec = registryCodec?.NormalizeRegistryDataTopLevelIdentifiers();

        Identifier dimensionType;
        if (version.Version.Protocol < ProtocolVersion.V_1_19)
        {
            var dimensionTypeNbt = buffer.ReadNbtCompound();
            dimensionType = Identifier.Parse(dimensionTypeNbt.Get<NbtString>("effects")!.Value);
        }
        else
        {
            dimensionType = buffer.ReadIdentifier();
        }

        var dimensionName = buffer.ReadIdentifier();
        var hashedSeed = buffer.ReadLong();
        var maxPlayers = buffer.ReadVarInt();
        var viewDistance = buffer.ReadVarInt();
        var simulationDistance = buffer.ReadVarInt();
        var reducedDebugInfo = buffer.ReadBool();
        var enableRespawnScreen = buffer.ReadBool();
        var isDebug = buffer.ReadBool();
        var isFlat = buffer.ReadBool();
        bool? hasDeathLocation = null;
        Identifier? deathDimensionName = null;
        Position? deathLocation = null;

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            hasDeathLocation = buffer.ReadBool();
            if (hasDeathLocation.Value)
            {
                deathDimensionName = buffer.ReadIdentifier();
                deathLocation = buffer.ReadPosition();
            }
        }

        int? portalCooldown = null;
        if (version.Version.Protocol >= ProtocolVersion.V_1_20)
        {
            portalCooldown = buffer.ReadVarInt();
        }

        return new LoginPacket(
            entityId,
            isHardcore,
            gameMode,
            previousGameMode,
            dimensionNames,
            registryCodec,
            dimensionType,
            dimensionName,
            hashedSeed,
            maxPlayers,
            viewDistance,
            simulationDistance,
            reducedDebugInfo,
            enableRespawnScreen,
            isDebug,
            isFlat,
            hasDeathLocation ?? false,
            deathDimensionName,
            deathLocation,
            portalCooldown,
            false);
    }

    private static LoginPacket ReadV1_20_2(PacketBuffer buffer, MinecraftData data)
    {
        var entityId = buffer.ReadInt();
        var isHardcode = buffer.ReadBool();
        var dimensionNames = buffer.ReadVarIntArray(buf => buf.ReadIdentifier());
        var maxPlayer = buffer.ReadVarInt();
        var viewDistance = buffer.ReadVarInt();
        var simulationDistance = buffer.ReadVarInt();
        var reducedDebugInfo = buffer.ReadBool();
        var enableRespawnScreen = buffer.ReadBool();
        var doLimitedCrafting = buffer.ReadBool();
        var dimensionType = buffer.ReadIdentifier();
        var dimensionName = buffer.ReadIdentifier();
        var hashedSeed = buffer.ReadLong();
        var gameMode = buffer.ReadByte();
        var previousGameMode = buffer.ReadSByte();
        var isDebug = buffer.ReadBool();
        var isFlat = buffer.ReadBool();
        var hasDeathLocation = buffer.ReadBool();
        Identifier? deathDimensionName = null;
        Position? deathLocation = null;
        if (hasDeathLocation)
        {
            deathDimensionName = buffer.ReadIdentifier();
            deathLocation = buffer.ReadPosition();
        }

        var portalCooldown = buffer.ReadVarInt();

        return new(
            entityId,
            isHardcode,
            gameMode,
            previousGameMode,
            dimensionNames,
            null,
            dimensionType,
            dimensionName,
            hashedSeed,
            maxPlayer,
            viewDistance,
            simulationDistance,
            reducedDebugInfo,
            enableRespawnScreen,
            isDebug,
            isFlat,
            hasDeathLocation,
            deathDimensionName,
            deathLocation,
            portalCooldown,
            doLimitedCrafting);
    }
}
