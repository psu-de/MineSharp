using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;
#pragma warning disable CS1591
/// <summary>
/// Login plugin request packet
/// </summary>
public class LoginPluginRequestPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_LoginPluginRequest;

    /// <summary>
    /// The message id
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// The channel identifier
    /// </summary>
    public string Channel { get; set; } // TODO: Identifier

    /// <summary>
    /// The raw message data
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="channel"></param>
    /// <param name="data"></param>
    public LoginPluginRequestPacket(int messageId, string channel, byte[] data)
    {
        this.MessageId = messageId;
        this.Channel   = channel;
        this.Data      = data;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.MessageId);
        buffer.WriteString(this.Channel);
        buffer.WriteBytes(this.Data.AsSpan());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var messageId = buffer.ReadVarInt();
        var channel   = buffer.ReadString();
        var data      = buffer.RestBuffer();

        return new LoginPluginRequestPacket(messageId, channel, data);
    }
}
#pragma warning restore CS1591
