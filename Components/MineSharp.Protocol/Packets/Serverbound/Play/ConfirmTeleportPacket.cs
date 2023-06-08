using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class ConfirmTeleportPacket : IPacket
{
    public static int Id => 0x00;
    
    public int TeleportId { get; set; }

    public ConfirmTeleportPacket(int teleportId)
    {
        this.TeleportId = teleportId;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.TeleportId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var teleportId = buffer.ReadVarInt();
        return new ConfirmTeleportPacket(teleportId);
    }
}
