using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class ConfirmTeleportPacket : IPacket
{
    public ConfirmTeleportPacket(int teleportId)
    {
        TeleportId = teleportId;
    }

    public int TeleportId { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Play_TeleportConfirm;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(TeleportId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var teleportId = buffer.ReadVarInt();
        return new ConfirmTeleportPacket(teleportId);
    }
}
#pragma warning restore CS1591
