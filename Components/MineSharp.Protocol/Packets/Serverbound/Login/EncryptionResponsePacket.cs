using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using static MineSharp.Protocol.Packets.Serverbound.Login.EncryptionResponsePacket;

namespace MineSharp.Protocol.Packets.Serverbound.Login;

/// <summary>
///     Encryption response packet
/// </summary>
/// <param name="SharedSecret">The shared secret</param>
/// <param name="VerifyToken">The verify token</param>
/// <param name="Crypto">The crypto container</param>
public sealed record EncryptionResponsePacket(byte[] SharedSecret, byte[]? VerifyToken, CryptoContainer? Crypto) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Login_EncryptionBegin;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(SharedSecret.Length);
        buffer.WriteBytes(SharedSecret);

        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
        {
            var hasVerifyToken = VerifyToken != null;
            buffer.WriteBool(hasVerifyToken);

            if (!hasVerifyToken)
            {
                if (Crypto == null)
                {
                    throw new MineSharpPacketVersionException(nameof(Crypto), version.Version.Protocol);
                }

                Crypto.Write(buffer);
                return;
            }
        }

        if (VerifyToken == null)
        {
            throw new MineSharpPacketVersionException(nameof(VerifyToken), version.Version.Protocol);
        }

        buffer.WriteVarInt(VerifyToken.Length);
        buffer.WriteBytes(VerifyToken);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var sharedSecretLength = buffer.ReadVarInt();
        var sharedSecret = buffer.ReadBytes(sharedSecretLength);
        CryptoContainer? crypto = null;
        byte[]? verifyToken = null;

        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
        {
            var hasVerifyToken = buffer.ReadBool();
            buffer.WriteBool(hasVerifyToken);

            if (!hasVerifyToken)
            {
                crypto = CryptoContainer.Read(buffer);
                return new EncryptionResponsePacket(sharedSecret, verifyToken, crypto);
            }
        }

        var verifyTokenLength = buffer.ReadVarInt();
        verifyToken = buffer.ReadBytes(verifyTokenLength);

        return new EncryptionResponsePacket(sharedSecret, verifyToken, crypto);
    }

    /// <summary>
    ///     Crypto container class
    /// </summary>
    /// <param name="Salt">The salt</param>
    /// <param name="MessageSignature">The message signature</param>
    public sealed record CryptoContainer(long Salt, byte[] MessageSignature) : ISerializable<CryptoContainer>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteLong(Salt);
            buffer.WriteVarInt(MessageSignature.Length);
            buffer.WriteBytes(MessageSignature.AsSpan());
        }

        /// <inheritdoc />
        public static CryptoContainer Read(PacketBuffer buffer)
        {
            var salt = buffer.ReadLong();
            var length = buffer.ReadVarInt();
            var verifyToken = buffer.ReadBytes(length);

            return new CryptoContainer(salt, verifyToken);
        }
    }
}
