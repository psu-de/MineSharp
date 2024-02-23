using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;
#pragma warning disable CS1591
/// <summary>
/// Encryption request packet
/// </summary>
public class EncryptionRequestPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_EncryptionBegin;

    /// <summary>
    /// The hashed server id
    /// </summary>
    public string ServerId { get; set; }

    /// <summary>
    /// The public key of the server
    /// </summary>
    public byte[] PublicKey { get; set; }

    /// <summary>
    /// Verify token
    /// </summary>
    public byte[] VerifyToken { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="publicKey"></param>
    /// <param name="verifyToken"></param>
    public EncryptionRequestPacket(string serverId, byte[] publicKey, byte[] verifyToken)
    {
        this.ServerId    = serverId;
        this.PublicKey   = publicKey;
        this.VerifyToken = verifyToken;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(ServerId);
        buffer.WriteVarInt(this.PublicKey.Length);
        buffer.WriteBytes(this.PublicKey.AsSpan());
        buffer.WriteVarInt(this.VerifyToken.Length);
        buffer.WriteBytes(this.VerifyToken.AsSpan());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var        serverId  = buffer.ReadString();
        Span<byte> publicKey = stackalloc byte[buffer.ReadVarInt()];
        buffer.ReadBytes(publicKey);
        Span<byte> verifyToken = stackalloc byte[buffer.ReadVarInt()];
        buffer.ReadBytes(verifyToken);

        return new EncryptionRequestPacket(serverId, publicKey.ToArray(), verifyToken.ToArray());
    }
}
#pragma warning restore CS1591
