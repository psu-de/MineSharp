using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

public class LoginPluginRequestPacket : IPacket
{
    public PacketType Type => PacketType.CB_Login_LoginPluginRequest;

    public int MessageId { get; set; }
    public string Channel { get; set; } // TODO: Identifier
    public byte[] Data { get; set; }

    public LoginPluginRequestPacket(int messageId, string channel, byte[] data)
    {
        this.MessageId = messageId;
        this.Channel = channel;
        this.Data = data;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.MessageId);
        buffer.WriteString(this.Channel);
        buffer.WriteBytes(this.Data.AsSpan());
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var messageId = buffer.ReadVarInt();
        var channel = buffer.ReadString();
        var data = buffer.RestBuffer();

        return new LoginPluginRequestPacket(messageId, channel, data);
    }
}
