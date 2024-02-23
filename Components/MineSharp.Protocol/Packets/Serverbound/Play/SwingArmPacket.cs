using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class SwingArmPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_ArmAnimation;

    public PlayerHand Hand { get; set; }

    public SwingArmPacket(PlayerHand hand)
    {
        this.Hand = hand;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)this.Hand);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new SwingArmPacket(
            (PlayerHand)buffer.ReadVarInt());
    }
}
#pragma warning restore CS1591
