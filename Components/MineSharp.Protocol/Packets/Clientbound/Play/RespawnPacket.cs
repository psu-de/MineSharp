using fNbt;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class RespawnPacket : IPacket
{
    public static int Id => 0x41;
    
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

    public RespawnPacket(string dimension, string dimensionName, long hashedSeed, sbyte gameMode, byte previousGameMode, bool isDebug, bool isFlat, bool copyMetadata, bool? hasDeathLocation, string? deathDimensionName, Position? deathLocation)
    {
        this.Dimension = dimension;
        this.DimensionName = dimensionName;
        this.HashedSeed = hashedSeed;
        this.GameMode = gameMode;
        this.PreviousGameMode = previousGameMode;
        this.IsDebug = isDebug;
        this.IsFlat = isFlat;
        this.CopyMetadata = copyMetadata;
        this.HasDeathLocation = hasDeathLocation;
        this.DeathDimensionName = deathDimensionName;
        this.DeathLocation = deathLocation;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        if (version.Protocol.Version <= ProtocolVersion.V_1_19)
        {
            throw new PacketVersionException($"Cannot write {nameof(RespawnPacket)} for versions before 1.19.");
        }
        
        buffer.WriteString(this.Dimension);
        buffer.WriteString(this.DimensionName);
        buffer.WriteLong(this.HashedSeed);
        buffer.WriteSByte(this.GameMode);
        buffer.WriteByte(this.PreviousGameMode);
        buffer.WriteBool(this.IsDebug);
        buffer.WriteBool(this.IsFlat);
        buffer.WriteBool(this.CopyMetadata);
        
        if (HasDeathLocation.HasValue)
            buffer.WriteBool(this.HasDeathLocation.Value);
        
        if (!this.HasDeathLocation ?? false)
            return;
        
        buffer.WriteString(this.DeathDimensionName!);
        buffer.WriteULong(this.DeathLocation!.ToULong());
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        string dimension;

        if (version.Protocol.Version <= ProtocolVersion.V_1_19)
        {
            var dimensionNbt = buffer.ReadNbt();
            dimension = dimensionNbt.Get<NbtString>("effects")!.Value;
        }
        else
            dimension = buffer.ReadString();
        
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
        if (version.Protocol.Version >= ProtocolVersion.V_1_19)
        {
            hasDeathLocation = buffer.ReadBool();
            if (hasDeathLocation ?? false)
            {
                deathDimensionName = buffer.ReadString();
                deathLocation = new Position(buffer.ReadULong());
            }
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
            deathLocation);
    }
}
