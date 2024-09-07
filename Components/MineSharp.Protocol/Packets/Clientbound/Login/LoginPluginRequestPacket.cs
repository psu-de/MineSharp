using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Login plugin request packet
/// </summary>
/// <param name="MessageId">The message id</param>
/// <param name="Channel">The channel identifier</param>
/// <param name="Data">The raw message data</param>
public sealed record LoginPluginRequestPacket(int MessageId, Identifier Channel, byte[] Data) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Login_LoginPluginRequest;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteIdentifier(Channel);
        buffer.WriteBytes(Data.AsSpan());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var messageId = buffer.ReadVarInt();
        var channel = buffer.ReadIdentifier();
        var data = buffer.RestBuffer();

        return new LoginPluginRequestPacket(messageId, channel, data);
    }
}
