using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class WindowSetSlotPacket : IPacket
{
    public WindowSetSlotPacket(sbyte windowId, int stateId, Slot slot)
    {
        WindowId = windowId;
        StateId = stateId;
        Slot = slot;
    }

    public sbyte WindowId { get; set; }
    public int StateId { get; set; }
    public Slot Slot { get; set; }
    public PacketType Type => PacketType.CB_Play_SetSlot;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteSByte(WindowId);
        buffer.WriteVarInt(StateId);
        buffer.WriteSlot(Slot);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new WindowSetSlotPacket(
            buffer.ReadSByte(),
            buffer.ReadVarInt(),
            buffer.ReadSlot(version));
    }
}
#pragma warning restore CS1591
