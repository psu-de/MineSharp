using fNbt;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class LoginPacket : IPacket
{
    public LoginPacket(int entityId,
                       bool isHardcore,
                       byte gameMode,
                       sbyte previousGameMode,
                       string[] dimensionNames,
                       NbtCompound? registryCodec,
                       string dimensionType,
                       string dimensionName,
                       long hashedSeed,
                       int maxPlayers,
                       int viewDistance,
                       int simulationDistance,
                       bool reducedDebugInfo,
                       bool enableRespawnScreen,
                       bool isDebug,
                       bool isFlat,
                       bool hasDeathLocation,
                       string? deathDimensionName,
                       Position? deathLocation,
                       int? portalCooldown,
                       bool? doLimitedCrafting)
    {
        EntityId = entityId;
        IsHardcore = isHardcore;
        GameMode = gameMode;
        PreviousGameMode = previousGameMode;
        DimensionNames = dimensionNames;
        RegistryCodec = registryCodec;
        DimensionType = dimensionType;
        DimensionName = dimensionName;
        HashedSeed = hashedSeed;
        MaxPlayers = maxPlayers;
        ViewDistance = viewDistance;
        SimulationDistance = simulationDistance;
        ReducedDebugInfo = reducedDebugInfo;
        EnableRespawnScreen = enableRespawnScreen;
        IsDebug = isDebug;
        IsFlat = isFlat;
        HasDeathLocation = hasDeathLocation;
        DeathDimensionName = deathDimensionName;
        DeathLocation = deathLocation;
        PortalCooldown = portalCooldown;
        DoLimitedCrafting = doLimitedCrafting;
    }

    public int EntityId { get; set; }
    public bool IsHardcore { get; set; }
    public byte GameMode { get; set; }
    public sbyte PreviousGameMode { get; set; }
    public string[] DimensionNames { get; set; }
    public NbtCompound? RegistryCodec { get; set; }
    public string DimensionType { get; set; }
    public string DimensionName { get; set; }
    public long HashedSeed { get; set; }
    public int MaxPlayers { get; set; }
    public int ViewDistance { get; set; }
    public int SimulationDistance { get; set; }
    public bool ReducedDebugInfo { get; set; }
    public bool EnableRespawnScreen { get; set; }
    public bool IsDebug { get; set; }
    public bool IsFlat { get; set; }
    public bool HasDeathLocation { get; set; }
    public string? DeathDimensionName { get; set; }
    public Position? DeathLocation { get; set; }
    public int? PortalCooldown { get; set; }
    public bool? DoLimitedCrafting { get; set; } // since 1.20.2
    public PacketType Type => PacketType.CB_Play_Login;

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

        buffer.WriteVarIntArray(DimensionNames, (buf, val) => buf.WriteString(val));
        if (version.Version.Protocol < ProtocolVersion.V_1_20_2)
        {
            buffer.WriteNbt(RegistryCodec);
            buffer.WriteString(DimensionType);
            buffer.WriteString(DimensionName);
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
            buffer.WriteString(DimensionType);
            buffer.WriteString(DimensionName);
            buffer.WriteLong(HashedSeed);
            buffer.WriteByte(GameMode);
            buffer.WriteSByte(PreviousGameMode);
        }

        buffer.WriteBool(IsDebug);
        buffer.WriteBool(IsFlat);

        buffer.WriteBool(HasDeathLocation);
        if (HasDeathLocation)
        {
            buffer.WriteString(DeathDimensionName!);
            buffer.WriteULong(DeathLocation!.ToULong());
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
        var dimensionNames = buffer.ReadVarIntArray<string>(buf => buf.ReadString());
        var registryCodec = buffer.ReadOptionalNbtCompound();

        string dimensionType;
        if (version.Version.Protocol < ProtocolVersion.V_1_19)
        {
            var dimensionTypeNbt = buffer.ReadNbtCompound();
            dimensionType = dimensionTypeNbt.Get<NbtString>("effects")!.Value;
        }
        else
        {
            dimensionType = buffer.ReadString();
        }

        var dimensionName = buffer.ReadString();
        var hashedSeed = buffer.ReadLong();
        var maxPlayers = buffer.ReadVarInt();
        var viewDistance = buffer.ReadVarInt();
        var simulationDistance = buffer.ReadVarInt();
        var reducedDebugInfo = buffer.ReadBool();
        var enableRespawnScreen = buffer.ReadBool();
        var isDebug = buffer.ReadBool();
        var isFlat = buffer.ReadBool();
        bool? hasDeathLocation = null;
        string? deathDimensionName = null;
        Position? deathLocation = null;

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            hasDeathLocation = buffer.ReadBool();
            if (hasDeathLocation.Value)
            {
                deathDimensionName = buffer.ReadString();
                deathLocation = new(buffer.ReadULong());
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
        var dimensionNames = buffer.ReadVarIntArray<string>(buf => buf.ReadString());
        var maxPlayer = buffer.ReadVarInt();
        var viewDistance = buffer.ReadVarInt();
        var simulationDistance = buffer.ReadVarInt();
        var reducedDebugInfo = buffer.ReadBool();
        var enableRespawnScreen = buffer.ReadBool();
        var doLimitedCrafting = buffer.ReadBool();
        var dimensionType = buffer.ReadString();
        var dimensionName = buffer.ReadString();
        var hashedSeed = buffer.ReadLong();
        var gameMode = buffer.ReadByte();
        var previousGameMode = buffer.ReadSByte();
        var isDebug = buffer.ReadBool();
        var isFlat = buffer.ReadBool();
        var hasDeathLocation = buffer.ReadBool();
        string? deathDimensionName = null;
        Position? deathLocation = null;
        if (hasDeathLocation)
        {
            deathDimensionName = buffer.ReadString();
            deathLocation = new(buffer.ReadULong());
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
#pragma warning restore CS1591
