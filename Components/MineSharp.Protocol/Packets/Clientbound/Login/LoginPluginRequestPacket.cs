using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;
#pragma warning disable CS1591
/// <summary>
///     Login plugin request packet
/// </summary>
public class LoginPluginRequestPacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="channel"></param>
    /// <param name="data"></param>
    public LoginPluginRequestPacket(int messageId, string channel, byte[] data)
    {
        MessageId = messageId;
        Channel = channel;
        Data = data;
    }

    /// <summary>
    ///     The message id
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    ///     The channel identifier
    /// </summary>
    public string Channel { get; set; } // TODO: Identifier

    /// <summary>
    ///     The raw message data
    /// </summary>
    public byte[] Data { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Login_LoginPluginRequest;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteString(Channel);
        buffer.WriteBytes(Data.AsSpan());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var messageId = buffer.ReadVarInt();
        var channel = buffer.ReadString();
        var data = buffer.RestBuffer();

        return new LoginPluginRequestPacket(messageId, channel, data);
    }
}
#pragma warning restore CS1591
