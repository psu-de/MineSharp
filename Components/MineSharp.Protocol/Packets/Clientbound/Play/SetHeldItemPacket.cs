using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SetHeldItemPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_HeldItemSlot;
    
    public sbyte Slot { get; set; }

    public SetHeldItemPacket(sbyte slot)
    {
        this.Slot = slot;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteSByte(this.Slot);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new SetHeldItemPacket(
            buffer.ReadSByte());
    }
}
#pragma warning restore CS1591