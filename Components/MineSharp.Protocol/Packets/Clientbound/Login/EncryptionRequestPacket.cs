using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;
#pragma warning disable CS1591
/// <summary>
///     Encryption request packet
/// </summary>
public class EncryptionRequestPacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="publicKey"></param>
    /// <param name="verifyToken"></param>
    public EncryptionRequestPacket(string serverId, byte[] publicKey, byte[] verifyToken)
    {
        ServerId = serverId;
        PublicKey = publicKey;
        VerifyToken = verifyToken;
    }

    /// <summary>
    ///     The hashed server id
    /// </summary>
    public string ServerId { get; set; }

    /// <summary>
    ///     The public key of the server
    /// </summary>
    public byte[] PublicKey { get; set; }

    /// <summary>
    ///     Verify token
    /// </summary>
    public byte[] VerifyToken { get; set; }

    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_EncryptionBegin;

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
        Span<byte> publicKey = stackalloc byte[buffer.ReadVarInt()];
        buffer.ReadBytes(publicKey);
        Span<byte> verifyToken = stackalloc byte[buffer.ReadVarInt()];
        buffer.ReadBytes(verifyToken);

        return new EncryptionRequestPacket(serverId, publicKey.ToArray(), verifyToken.ToArray());
    }
}
#pragma warning restore CS1591
