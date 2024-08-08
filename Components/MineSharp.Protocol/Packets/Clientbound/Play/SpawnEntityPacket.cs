using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record SpawnEntityPacket(
    int EntityId,
    Uuid ObjectUuid,
    int EntityType,
    double X,
    double Y,
    double Z,
    sbyte Pitch,
    sbyte Yaw,
    sbyte HeadPitch,
    int ObjectData,
    short VelocityX,
    short VelocityY,
    short VelocityZ
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SpawnEntity;

    /// <summary>
    /// Writes the packet data to the buffer.
    /// </summary>
    /// <param name="buffer">The buffer to write to.</param>
    /// <param name="version">The Minecraft version.</param>
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteUuid(ObjectUuid);
        buffer.WriteVarInt(EntityType);
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteSByte(Pitch);
        buffer.WriteSByte(Yaw);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            buffer.WriteSByte(HeadPitch);
            buffer.WriteVarInt(ObjectData);
        }
        else
        {
            buffer.WriteInt(ObjectData);
        }

        buffer.WriteShort(VelocityX);
        buffer.WriteShort(VelocityY);
        buffer.WriteShort(VelocityZ);
    }

    /// <summary>
    /// Reads the packet data from the buffer.
    /// </summary>
    /// <param name="buffer">The buffer to read from.</param>
    /// <param name="version">The Minecraft version.</param>
    /// <returns>A new instance of <see cref="SpawnEntityPacket"/>.</returns>
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var objectUuid = buffer.ReadUuid();
        var type = buffer.ReadVarInt();
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var pitch = buffer.ReadSByte();
        var yaw = buffer.ReadSByte();
        sbyte headPitch = 0;
        var objectData = 0;
        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            headPitch = buffer.ReadSByte();
            objectData = buffer.ReadVarInt();
        }
        else
        {
            objectData = buffer.ReadInt();
        }

        var velocityX = buffer.ReadShort();
        var velocityY = buffer.ReadShort();
        var velocityZ = buffer.ReadShort();

        return new SpawnEntityPacket(entityId, objectUuid, type, x, y, z, pitch, yaw, headPitch, objectData, velocityX, velocityY, velocityZ);
    }
}
#pragma warning restore CS1591
