using fNbt;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class RespawnPacket : IPacket
{
    public RespawnPacket(string dimension, string dimensionName, long hashedSeed, sbyte gameMode, byte previousGameMode,
                         bool isDebug,
                         bool isFlat, bool copyMetadata, bool? hasDeathLocation, string? deathDimensionName,
                         Position? deathLocation,
                         int? portalCooldown)
    {
        Dimension = dimension;
        DimensionName = dimensionName;
        HashedSeed = hashedSeed;
        GameMode = gameMode;
        PreviousGameMode = previousGameMode;
        IsDebug = isDebug;
        IsFlat = isFlat;
        CopyMetadata = copyMetadata;
        HasDeathLocation = hasDeathLocation;
        DeathDimensionName = deathDimensionName;
        DeathLocation = deathLocation;
        PortalCooldown = portalCooldown;
    }

    public string Dimension { get; set; }
    public string DimensionName { get; set; }
    public long HashedSeed { get; set; }
    public sbyte GameMode { get; set; }
    public byte PreviousGameMode { get; set; }
    public bool IsDebug { get; set; }
    public bool IsFlat { get; set; }
    public bool CopyMetadata { get; set; }
    public bool? HasDeathLocation { get; set; }
    public string? DeathDimensionName { get; set; }
    public Position? DeathLocation { get; set; }
    public int? PortalCooldown { get; set; }
    public PacketType Type => PacketType.CB_Play_Respawn;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol <= ProtocolVersion.V_1_19)
        {
            throw new NotSupportedException(
                $"{nameof(RespawnPacket)}.Write() is not supported for versions before 1.19.");
        }

        buffer.WriteString(Dimension);
        buffer.WriteString(DimensionName);
        buffer.WriteLong(HashedSeed);
        buffer.WriteSByte(GameMode);
        buffer.WriteByte(PreviousGameMode);
        buffer.WriteBool(IsDebug);
        buffer.WriteBool(IsFlat);
        buffer.WriteBool(CopyMetadata);

        if (HasDeathLocation.HasValue)
        {
            buffer.WriteBool(HasDeathLocation.Value);
        }

        if (!HasDeathLocation ?? false)
        {
            return;
        }

        buffer.WriteString(DeathDimensionName!);
        buffer.WriteULong(DeathLocation!.ToULong());

        if (version.Version.Protocol >= ProtocolVersion.V_1_20)
        {
            buffer.WriteVarInt(PortalCooldown!.Value);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        string dimension;

        if (version.Version.Protocol <= ProtocolVersion.V_1_19)
        {
            var dimensionNbt = buffer.ReadNbtCompound();
            dimension = dimensionNbt.Get<NbtString>("effects")!.Value;
        }
        else
        {
            dimension = buffer.ReadString();
        }

        var dimensionName = buffer.ReadString();
        var hashedSeed = buffer.ReadLong();
        var gameMode = buffer.ReadSByte();
        var previousGameMode = buffer.ReadByte();
        var isDebug = buffer.ReadBool();
        var isFlat = buffer.ReadBool();
        var copyMetadata = buffer.ReadBool();

        bool? hasDeathLocation = null;
        string? deathDimensionName = null;
        Position? deathLocation = null;
        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            hasDeathLocation = buffer.ReadBool();
            if (hasDeathLocation ?? false)
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

        return new RespawnPacket(
            dimension,
            dimensionName,
            hashedSeed,
            gameMode,
            previousGameMode,
            isDebug,
            isFlat,
            copyMetadata,
            hasDeathLocation,
            deathDimensionName,
            deathLocation,
            portalCooldown);
    }
}
#pragma warning restore CS1591
