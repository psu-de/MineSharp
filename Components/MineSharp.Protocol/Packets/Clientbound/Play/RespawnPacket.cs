using fNbt;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record RespawnPacket(
    Identifier DimensionType,
    Identifier DimensionName,
    long HashedSeed,
    sbyte GameMode,
    byte PreviousGameMode,
    bool IsDebug,
    bool IsFlat,
    bool CopyMetadata,
    bool? HasDeathLocation,
    Identifier? DeathDimensionName,
    Position? DeathLocation,
    int? PortalCooldown
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Respawn;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol <= ProtocolVersion.V_1_19)
        {
            throw new NotSupportedException(
                $"{nameof(RespawnPacket)}.Write() is not supported for versions before 1.19.");
        }

        buffer.WriteIdentifier(DimensionType);
        buffer.WriteIdentifier(DimensionName);
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

        buffer.WriteIdentifier(DeathDimensionName!);
        buffer.WritePosition(DeathLocation!);

        if (version.Version.Protocol >= ProtocolVersion.V_1_20)
        {
            buffer.WriteVarInt(PortalCooldown!.Value);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        Identifier dimensionType;

        if (version.Version.Protocol <= ProtocolVersion.V_1_19)
        {
            var dimensionNbt = buffer.ReadNbtCompound();
            dimensionType = Identifier.Parse(dimensionNbt.Get<NbtString>("effects")!.Value);
        }
        else
        {
            dimensionType = buffer.ReadIdentifier();
        }

        var dimensionName = buffer.ReadIdentifier();
        var hashedSeed = buffer.ReadLong();
        var gameMode = buffer.ReadSByte();
        var previousGameMode = buffer.ReadByte();
        var isDebug = buffer.ReadBool();
        var isFlat = buffer.ReadBool();
        var copyMetadata = buffer.ReadBool();

        bool? hasDeathLocation = null;
        Identifier? deathDimensionName = null;
        Position? deathLocation = null;
        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            hasDeathLocation = buffer.ReadBool();
            if (hasDeathLocation ?? false)
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

        return new RespawnPacket(
            dimensionType,
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
