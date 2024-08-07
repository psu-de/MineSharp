using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class SwingArmPacket : IPacket
{
    public SwingArmPacket(PlayerHand hand)
    {
        Hand = hand;
    }

    public PlayerHand Hand { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Play_ArmAnimation;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)Hand);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new SwingArmPacket(
            (PlayerHand)buffer.ReadVarInt());
    }
}
#pragma warning restore CS1591
