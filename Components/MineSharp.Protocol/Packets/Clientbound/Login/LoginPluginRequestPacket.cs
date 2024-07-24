using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Login plugin request packet
///     See https://wiki.vg/Protocol#Login_Plugin_Request
/// </summary>
public class LoginPluginRequestPacket : IPacket
{
    /// <summary>
    ///     The message id
    /// </summary>
    public required int MessageId { get; init; }

    /// <summary>
    ///     The channel identifier
    /// </summary>
    public required string Channel { get; init; }

    /// <summary>
    ///     The raw message data
    /// </summary>
    public required byte[] Data { get; init; }
    
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_LoginPluginRequest;

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

        return new LoginPluginRequestPacket() { MessageId = messageId, Channel = channel, Data = data };
    }
}
