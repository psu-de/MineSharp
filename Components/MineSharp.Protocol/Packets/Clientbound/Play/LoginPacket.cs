using fNbt;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class LoginPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_Login;

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
        this.EntityId = entityId;
        this.IsHardcore = isHardcore;
        this.GameMode = gameMode;
        this.PreviousGameMode = previousGameMode;
        this.DimensionNames = dimensionNames;
        this.RegistryCodec = registryCodec;
        this.DimensionType = dimensionType;
        this.DimensionName = dimensionName;
        this.HashedSeed = hashedSeed;
        this.MaxPlayers = maxPlayers;
        this.ViewDistance = viewDistance;
        this.SimulationDistance = simulationDistance;
        this.ReducedDebugInfo = reducedDebugInfo;
        this.EnableRespawnScreen = enableRespawnScreen;     
        this.IsDebug = isDebug;
        this.IsFlat = isFlat;
        this.HasDeathLocation = hasDeathLocation;
        this.DeathDimensionName = deathDimensionName;
        this.DeathLocation = deathLocation;
        this.PortalCooldown = portalCooldown;
        this.DoLimitedCrafting = doLimitedCrafting;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol < ProtocolVersion.V_1_19)
        {
            throw new PacketVersionException($"Cannot write {nameof(LoginPacket)} for versions before 1.19.");
        }
        
        buffer.WriteInt(this.EntityId);
        buffer.WriteBool(this.IsHardcore);
        if (version.Version.Protocol < ProtocolVersion.V_1_20_2)
        {
            buffer.WriteByte(this.GameMode);
            buffer.WriteSByte(this.PreviousGameMode);   
        }
        buffer.WriteVarIntArray(this.DimensionNames, (buf, val) => buf.WriteString(val));
        if (version.Version.Protocol < ProtocolVersion.V_1_20_2)
        {
            buffer.WriteNbt(this.RegistryCodec);
            buffer.WriteString(this.DimensionType);
            buffer.WriteString(this.DimensionName);
            buffer.WriteLong(this.HashedSeed);   
        }
        buffer.WriteVarInt(this.MaxPlayers);
        buffer.WriteVarInt(this.ViewDistance);
        buffer.WriteVarInt(this.SimulationDistance);
        buffer.WriteBool(this.ReducedDebugInfo);
        buffer.WriteBool(this.EnableRespawnScreen);
        if (version.Version.Protocol >= ProtocolVersion.V_1_20_2)
        {
            buffer.WriteBool(this.DoLimitedCrafting!.Value);
            buffer.WriteString(this.DimensionType);
            buffer.WriteString(this.DimensionName);
            buffer.WriteLong(this.HashedSeed);
            buffer.WriteByte(this.GameMode);
            buffer.WriteSByte(this.PreviousGameMode);
        }
        buffer.WriteBool(this.IsDebug);
        buffer.WriteBool(this.IsFlat);
        
        buffer.WriteBool(this.HasDeathLocation);
        if (this.HasDeathLocation)
        {
            buffer.WriteString(this.DeathDimensionName!);
            buffer.WriteULong(this.DeathLocation!.ToULong());
        }

        if (version.Version.Protocol >= ProtocolVersion.V_1_20)
        {
            if (this.PortalCooldown == null)
                throw new ArgumentNullException(nameof(PortalCooldown));
            buffer.WriteVarInt(this.PortalCooldown.Value);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol >= ProtocolVersion.V_1_20_2)
            return ReadV1_20_2(buffer, version);
        
        var entityId = buffer.ReadInt();
        var isHardcore = buffer.ReadBool();
        var gameMode = buffer.ReadByte();
        var previousGameMode = buffer.ReadSByte();
        var dimensionNames = buffer.ReadVarIntArray<string>(buf => buf.ReadString());
        var registryCodec = buffer.ReadNbtCompound();

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
                deathLocation = new Position(buffer.ReadULong());
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
            deathLocation = new Position(buffer.ReadULong());
        }
        var portalCooldown = buffer.ReadVarInt();

        return new LoginPacket(
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
