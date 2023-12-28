using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Serverbound.Login;
#pragma warning disable CS1591
public class EncryptionResponsePacket : IPacket
{
    public PacketType Type => PacketType.SB_Login_EncryptionBegin;
    
    public byte[] SharedSecret { get; set; }
    public byte[]? VerifyToken { get; set; }
    public CryptoContainer? Crypto { get; set; }

    public EncryptionResponsePacket(byte[] sharedSecret, byte[]? verifyToken, CryptoContainer? crypto)
    {
        this.SharedSecret = sharedSecret;
        this.VerifyToken = verifyToken;
        this.Crypto = crypto;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.SharedSecret.Length);
        buffer.WriteBytes(this.SharedSecret);

        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
        {
            bool hasVerifyToken = this.VerifyToken != null;
            buffer.WriteBool(hasVerifyToken);
            
            if (!hasVerifyToken)
            {
                if (this.Crypto == null)
                {
                    throw new PacketVersionException(
                        $"{nameof(EncryptionResponsePacket)} expect to have Crypto or VerifyToken set for version 1.19-1.19.2");
                }
                
                this.Crypto.Write(buffer);
                return;
            }
        }

        if (this.VerifyToken == null)
        {
            throw new PacketVersionException(
                $"{nameof(EncryptionResponsePacket)} expects to have VerifyToken set.");
        }
        
        buffer.WriteVarInt(this.VerifyToken.Length);
        buffer.WriteBytes(this.VerifyToken);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var sharedSecretLength = buffer.ReadVarInt();
        var sharedSecret = buffer.ReadBytes(sharedSecretLength);
        CryptoContainer? crypto = null;
        byte[]? verifyToken = null;
        
        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
        {
            bool hasVerifyToken = buffer.ReadBool();
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
        public long Salt { get; set; }
        public byte[] MessageSignature { get; set; }

        public CryptoContainer(long salt, byte[] messageSignature)
        {
            this.Salt = salt;
            this.MessageSignature = messageSignature;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteLong(this.Salt);
            buffer.WriteVarInt(this.MessageSignature.Length);
            buffer.WriteBytes(this.MessageSignature.AsSpan());
        }

        public static CryptoContainer Read(PacketBuffer buffer)
        {
            var salt = buffer.ReadLong();
            var length = buffer.ReadVarInt();
            var verifyToken = buffer.ReadBytes(length);

            return new CryptoContainer(salt, verifyToken);
        }
    }
}
#pragma warning restore CS1591