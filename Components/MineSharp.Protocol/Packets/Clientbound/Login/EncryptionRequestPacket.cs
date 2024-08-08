using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Encryption request packet
///     See https://wiki.vg/Protocol#Encryption_Request
/// </summary>
/// <param name="ServerId">The hashed server id</param>
/// <param name="PublicKey">The public key of the server</param>
/// <param name="VerifyToken">Verify token</param>
public sealed record EncryptionRequestPacket(string ServerId, byte[] PublicKey, byte[] VerifyToken) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Login_EncryptionBegin;

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

        return new EncryptionRequestPacket(serverId, publicKey, verifyToken);
    }
}
