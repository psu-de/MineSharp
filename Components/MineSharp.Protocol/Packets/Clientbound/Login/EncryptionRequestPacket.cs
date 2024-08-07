using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Encryption request packet
///     See https://wiki.vg/Protocol#Encryption_Request
/// </summary>
public class EncryptionRequestPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Login_EncryptionBegin;
    
    /// <summary>
    ///     The hashed server id
    /// </summary>
    public required string ServerId { get; init; }

    /// <summary>
    ///     The public key of the server
    /// </summary>
    public required byte[] PublicKey { get; init; }

    /// <summary>
    ///     Verify token
    /// </summary>
    public required byte[] VerifyToken { get; init; }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(ServerId);
        buffer.WriteVarInt(PublicKey.Length);
        buffer.WriteBytes(PublicKey.AsSpan());
        buffer.WriteVarInt(VerifyToken.Length);
        buffer.WriteBytes(VerifyToken.AsSpan());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var serverId = buffer.ReadString();
        var publicKey = new byte[buffer.ReadVarInt()];
        buffer.ReadBytes(publicKey);
        var verifyToken = new byte[buffer.ReadVarInt()];
        buffer.ReadBytes(verifyToken);

        return new EncryptionRequestPacket()
        {
            ServerId    = serverId,
            PublicKey   = publicKey,
            VerifyToken = verifyToken
        };
    }
}
