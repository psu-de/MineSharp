using MineSharp.Chat;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

public class DisconnectPacket : IPacket
{
    public PacketType Type => PacketType.CB_Configuration_Disconnect;

    public ChatComponent Reason { get; set; }

    public DisconnectPacket(ChatComponent reason)
    {
        this.Reason = reason;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Reason.Json);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        string reason = buffer.ReadString();
        return new DisconnectPacket(new ChatComponent(reason, version));
    }
}
