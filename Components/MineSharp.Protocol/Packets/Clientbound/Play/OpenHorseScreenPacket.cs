using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Open Horse Screen packet
/// </summary>
/// <param name="WindowId">The window ID</param>
/// <param name="SlotCount">The number of slots in the horse inventory</param>
/// <param name="EntityId">The entity ID of the horse</param>
public sealed record OpenHorseScreenPacket(byte WindowId, int SlotCount, int EntityId) : IPacketStatic<OpenHorseScreenPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_OpenHorseWindow;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteByte(WindowId);
        buffer.WriteVarInt(SlotCount);
        buffer.WriteInt(EntityId);
    }

    /// <inheritdoc />
    public static OpenHorseScreenPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var windowId = buffer.ReadByte();
        var slotCount = buffer.ReadVarInt();
        var entityId = buffer.ReadInt();

        return new OpenHorseScreenPacket(
            windowId,
            slotCount,
            entityId);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
