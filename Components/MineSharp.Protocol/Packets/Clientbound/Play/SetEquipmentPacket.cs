using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;
using static MineSharp.Protocol.Packets.Clientbound.Play.SetEquipmentPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Set Equipment packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="Equipment">The equipment list</param>
public sealed record SetEquipmentPacket(int EntityId, EquipmentEntry[] Equipment) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityEquipment;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        for (int i = 0; i < Equipment.Length; i++)
        {
            var entry = Equipment[i];
            var isLastEntry = i == Equipment.Length - 1;
            entry.Write(buffer, version, isLastEntry);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var equipment = new List<EquipmentEntry>();
        while (true)
        {
            var slot = buffer.Peek();
            // wiki.vg says: "has the top bit set if another entry follows, and otherwise unset if this is the last item in the array"
            var isLast = (slot & 0x80) == 0;
            equipment.Add(EquipmentEntry.Read(buffer, version));
            if (isLast)
            {
                break;
            }
        }

        return new SetEquipmentPacket(entityId, equipment.ToArray());
    }

    /// <summary>
    ///     Equipment entry
    /// </summary>
    /// <param name="SlotId">The equipment slot</param>
    /// <param name="Item">The item in the slot</param>
    public sealed record EquipmentEntry(EquipmentSlot SlotId, Item? Item) : ISerializableWithMinecraftData<EquipmentEntry>
    {
        /// <inheritdoc />
        [Obsolete("Use the Write method with the isLastEntry parameter instead")]
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            Write(buffer, data, true);
        }

        /// <summary>
        /// Writes the equipment entry to the packet buffer.
        /// This method is required because the slotId needs to be changed when its not the last entry
        /// </summary>
        /// <param name="buffer">The packet buffer to write to.</param>
        /// <param name="data">The Minecraft data.</param>
        /// <param name="isLastEntry">Indicates if this is the last entry in the equipment list.</param>
        public void Write(PacketBuffer buffer, MinecraftData data, bool isLastEntry)
        {
            var slotId = (byte)SlotId;
            if (!isLastEntry)
            {
                // wiki.vg says: "has the top bit set if another entry follows, and otherwise unset if this is the last item in the array"
                slotId |= 0x80; // Set the top bit if another entry follows
            }
            buffer.WriteByte(slotId);
            buffer.WriteOptionalItem(Item);
        }

        /// <inheritdoc />
        public static EquipmentEntry Read(PacketBuffer buffer, MinecraftData data)
        {
            var slotId = (EquipmentSlot)(buffer.ReadByte() & 0x7F); // Unset the top bit
            var item = buffer.ReadOptionalItem(data);
            return new EquipmentEntry(slotId, item);
        }
    }

    /// <summary>
    ///     Equipment slot enumeration
    /// </summary>
    public enum EquipmentSlot : byte
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        MainHand = 0,
        OffHand = 1,
        Boots = 2,
        Leggings = 3,
        Chestplate = 4,
        Helmet = 5
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
