using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class SetHeldItemPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_HeldItemSlot;
    
    public int Slot { get; set; }

    public SetHeldItemPacket(int slot)
    {
        this.Slot = slot;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.Slot);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new SetHeldItemPacket(
            buffer.ReadVarInt());
    }
}
