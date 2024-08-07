using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Serverbound.Login;
#pragma warning disable CS1591
public class EncryptionResponsePacket : IPacket
{
    public EncryptionResponsePacket(byte[] sharedSecret, byte[]? verifyToken, CryptoContainer? crypto)
    {
        SharedSecret = sharedSecret;
        VerifyToken = verifyToken;
        Crypto = crypto;
    }

    public byte[] SharedSecret { get; set; }
    public byte[]? VerifyToken { get; set; }
    public CryptoContainer? Crypto { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Login_EncryptionBegin;

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


    public class CryptoContainer : ISerializable<CryptoContainer>
    {
        public CryptoContainer(long salt, byte[] messageSignature)
        {
            Salt = salt;
            MessageSignature = messageSignature;
        }

        public long Salt { get; set; }
        public byte[] MessageSignature { get; set; }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteLong(Salt);
            buffer.WriteVarInt(MessageSignature.Length);
            buffer.WriteBytes(MessageSignature.AsSpan());
        }

        public static CryptoContainer Read(PacketBuffer buffer)
        {
            var salt = buffer.ReadLong();
            var length = buffer.ReadVarInt();
            var verifyToken = buffer.ReadBytes(length);

            return new(salt, verifyToken);
        }
    }
}
#pragma warning restore CS1591
