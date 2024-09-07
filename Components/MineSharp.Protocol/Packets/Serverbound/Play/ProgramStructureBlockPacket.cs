using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Serverbound.Play.ProgramStructureBlockPacket;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
/// <summary>
///     Program Structure Block packet
/// </summary>
/// <param name="Location">Block entity location</param>
/// <param name="Action">An additional action to perform beyond simply saving the given data. See <see cref="StructureBlockAction"/></param>
/// <param name="Mode">One of <see cref="StructureBlockMode"/></param>
/// <param name="Name">Name of the structure</param>
/// <param name="OffsetX">Offset X, between -48 and 48</param>
/// <param name="OffsetY">Offset Y, between -48 and 48</param>
/// <param name="OffsetZ">Offset Z, between -48 and 48</param>
/// <param name="SizeX">Size X, between 0 and 48</param>
/// <param name="SizeY">Size Y, between 0 and 48</param>
/// <param name="SizeZ">Size Z, between 0 and 48</param>
/// <param name="Mirror">One of <see cref="StructureBlockMirror"/></param>
/// <param name="Rotation">One of <see cref="StructureBlockRotation"/></param>
/// <param name="Metadata">Metadata of the structure</param>
/// <param name="Integrity">Integrity, between 0 and 1</param>
/// <param name="Seed">Seed for the structure</param>
/// <param name="Flags">Flags. See <see cref="StructureBlockFlags"/></param>
public sealed record ProgramStructureBlockPacket(
    Position Location,
    StructureBlockAction Action,
    StructureBlockMode Mode,
    string Name,
    sbyte OffsetX,
    sbyte OffsetY,
    sbyte OffsetZ,
    sbyte SizeX,
    sbyte SizeY,
    sbyte SizeZ,
    StructureBlockMirror Mirror,
    StructureBlockRotation Rotation,
    string Metadata,
    float Integrity,
    long Seed,
    StructureBlockFlags Flags) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_UpdateStructureBlock;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WritePosition(Location);
        buffer.WriteVarInt((int)Action);
        buffer.WriteVarInt((int)Mode);
        buffer.WriteString(Name);
        buffer.WriteSByte(OffsetX);
        buffer.WriteSByte(OffsetY);
        buffer.WriteSByte(OffsetZ);
        buffer.WriteSByte(SizeX);
        buffer.WriteSByte(SizeY);
        buffer.WriteSByte(SizeZ);
        buffer.WriteVarInt((int)Mirror);
        buffer.WriteVarInt((int)Rotation);
        buffer.WriteString(Metadata);
        buffer.WriteFloat(Integrity);
        buffer.WriteVarLong(Seed);
        buffer.WriteSByte((sbyte)Flags);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = buffer.ReadPosition();
        var action = (StructureBlockAction)buffer.ReadVarInt();
        var mode = (StructureBlockMode)buffer.ReadVarInt();
        var name = buffer.ReadString();
        var offsetX = buffer.ReadSByte();
        var offsetY = buffer.ReadSByte();
        var offsetZ = buffer.ReadSByte();
        var sizeX = buffer.ReadSByte();
        var sizeY = buffer.ReadSByte();
        var sizeZ = buffer.ReadSByte();
        var mirror = (StructureBlockMirror)buffer.ReadVarInt();
        var rotation = (StructureBlockRotation)buffer.ReadVarInt();
        var metadata = buffer.ReadString();
        var integrity = buffer.ReadFloat();
        var seed = buffer.ReadVarLong();
        var flags = (StructureBlockFlags)buffer.ReadSByte();

        return new ProgramStructureBlockPacket(
            location,
            action,
            mode,
            name,
            offsetX,
            offsetY,
            offsetZ,
            sizeX,
            sizeY,
            sizeZ,
            mirror,
            rotation,
            metadata,
            integrity,
            seed,
            flags);
    }

    /// <summary>
    /// Enum representing the action to perform on the structure block.
    /// </summary>
    public enum StructureBlockAction
    {
        UpdateData = 0,
        SaveStructure = 1,
        LoadStructure = 2,
        DetectSize = 3
    }

    /// <summary>
    /// Enum representing the mode of the structure block.
    /// </summary>
    public enum StructureBlockMode
    {
        Save = 0,
        Load = 1,
        Corner = 2,
        Data = 3
    }

    /// <summary>
    /// Enum representing the mirror type of the structure block.
    /// </summary>
    public enum StructureBlockMirror
    {
        None = 0,
        LeftRight = 1,
        FrontBack = 2
    }

    /// <summary>
    /// Enum representing the rotation type of the structure block.
    /// </summary>
    public enum StructureBlockRotation
    {
        None = 0,
        Clockwise90 = 1,
        Clockwise180 = 2,
        Counterclockwise90 = 3
    }

    /// <summary>
    /// Enum representing the flags for the structure block.
    /// </summary>
    [Flags]
    public enum StructureBlockFlags : sbyte
    {
        IgnoreEntities = 0x01,
        ShowAir = 0x02,
        ShowBoundingBox = 0x04
    }

}
