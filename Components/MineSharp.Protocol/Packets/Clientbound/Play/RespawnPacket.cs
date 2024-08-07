using fNbt;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record RespawnPacket(
    string Dimension,
    string DimensionName,
    long HashedSeed,
    sbyte GameMode,
    byte PreviousGameMode,
    bool IsDebug,
    bool IsFlat,
    bool CopyMetadata,
    bool? HasDeathLocation,
    string? DeathDimensionName,
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
