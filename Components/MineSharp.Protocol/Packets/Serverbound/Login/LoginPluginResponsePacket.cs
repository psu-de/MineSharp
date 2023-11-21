using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Login;

public class LoginPluginResponsePacket : IPacket
{
    public PacketType Type => PacketType.SB_Login_LoginPluginResponse;

    public int MessageId { get; set; }
    public byte[]? Data { get; set; }

    public LoginPluginResponsePacket(int messageId, byte[]? data)
    {
        this.MessageId = messageId;
        this.Data = data;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.MessageId);
        buffer.WriteBool(this.Data != null);

        if (this.Data != null)
        {
            buffer.WriteBytes(this.Data);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var messageId = buffer.ReadVarInt();
        var hasData = buffer.ReadBool();
        var data = hasData switch {
            true => buffer.RestBuffer(),
            false => null
        };

        return new LoginPluginResponsePacket(messageId, data);
    }
}
