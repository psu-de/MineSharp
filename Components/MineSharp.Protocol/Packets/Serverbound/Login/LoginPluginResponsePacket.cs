using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Login;
#pragma warning disable CS1591
public class LoginPluginResponsePacket : IPacket
{
    public LoginPluginResponsePacket(int messageId, byte[]? data)
    {
        MessageId = messageId;
        Data = data;
    }

    public int MessageId { get; set; }
    public byte[]? Data { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Login_LoginPluginResponse;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteBool(Data != null);

        if (Data != null)
        {
            buffer.WriteBytes(Data);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var messageId = buffer.ReadVarInt();
        var hasData = buffer.ReadBool();
        var data = hasData switch
        {
            true => buffer.RestBuffer(),
            false => null
        };

        return new LoginPluginResponsePacket(messageId, data);
    }
}
#pragma warning restore CS1591
