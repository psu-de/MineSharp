using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

public class DisconnectPacket : IPacket
{
    public static int Id => 0x00;

    public Chat Reason { get; set; }

    public DisconnectPacket(Chat reason)
    {
        this.Reason = reason;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteString(this.Reason.JSON);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        string reason = buffer.ReadString();
        return new DisconnectPacket(new Chat(reason));
    }
}
