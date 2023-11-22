using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class WindowSetSlotPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_SetSlot;
    
    public sbyte WindowId { get; set; }
    public int StateId { get; set; }
    public Slot Slot { get; set; }


    public WindowSetSlotPacket(sbyte windowId, int stateId, Slot slot)
    {
        this.WindowId = windowId;
        this.StateId = stateId;
        this.Slot = slot;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteSByte(this.WindowId);
        buffer.WriteVarInt(this.StateId);
        buffer.WriteSlot(this.Slot);
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new WindowSetSlotPacket(
            buffer.ReadSByte(),
            buffer.ReadVarInt(),
            buffer.ReadSlot(version));
    }
}
