using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class ConfirmTeleportPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_TeleportConfirm;
    
    public int TeleportId { get; set; }

    public ConfirmTeleportPacket(int teleportId)
    {
        this.TeleportId = teleportId;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.TeleportId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var teleportId = buffer.ReadVarInt();
        return new ConfirmTeleportPacket(teleportId);
    }
}
