using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

public class LoginPluginRequestPacket : IPacket
{
    public static int Id => 0x04;

    public int MessageId { get; set; }
    public string Channel { get; set; } // TODO: Identifier
    public byte[] Data { get; set; }

    public LoginPluginRequestPacket(int messageId, string channel, byte[] data)
    {
        this.MessageId = messageId;
        this.Channel = channel;
        this.Data = data;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.MessageId);
        buffer.WriteString(this.Channel);
        buffer.WriteBytes(this.Data.AsSpan());
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var messageId = buffer.ReadVarInt();
        var channel = buffer.ReadString();
        var data = buffer.RestBuffer();

        return new LoginPluginRequestPacket(messageId, channel, data);
    }
}
