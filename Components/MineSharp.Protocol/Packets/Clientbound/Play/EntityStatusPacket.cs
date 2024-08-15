using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Entity status packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="Status">The status byte</param>
public sealed record EntityStatusPacket(int EntityId, byte Status) : IPacketStatic<EntityStatusPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityStatus;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteByte(Status);
    }

    /// <inheritdoc />
    public static EntityStatusPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new EntityStatusPacket(
            buffer.ReadInt(),
            buffer.ReadByte());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}

