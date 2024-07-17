using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Serverbound.Login;
#pragma warning disable CS1591
public class LoginStartPacket : IPacket
{
    public PacketType Type => PacketType.SB_Login_LoginStart;

    public string              Username   { get; set; }
    public SignatureContainer? Signature  { get; set; }
    public UUID?               PlayerUuid { get; set; }


    /// <summary>
    /// Constructor for versions before 1.19
    /// </summary>
    /// <param name="username"></param>
    public LoginStartPacket(string username)
    {
        this.Username = username;
    }

    /// <summary>
    /// Constructor for versions >= 1.19.3
    /// </summary>
    /// <param name="username"></param>
    /// <param name="playerUuid"></param>
    public LoginStartPacket(string username, UUID? playerUuid)
    {
        this.Username   = username;
        this.PlayerUuid = playerUuid;
    }


    /// <summary>
    /// Constructor for version 1.19-1.19.2
    /// </summary>
    /// <param name="username"></param>
    /// <param name="signature"></param>
    /// <param name="playerUuid"></param>
    public LoginStartPacket(string username, SignatureContainer? signature, UUID? playerUuid = null)
    {
        this.Username   = username;
        this.Signature  = signature;
        this.PlayerUuid = playerUuid;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Username);

        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
        {
            bool hasSignature = this.Signature != null;
            buffer.WriteBool(hasSignature);
            this.Signature?.Write(buffer);
        }

        if (version.Version.Protocol < ProtocolVersion.V_1_19_2)
        {
            return;
        }

        if (version.Version.Protocol < ProtocolVersion.V_1_20_2)
        {
            buffer.WriteBool(this.PlayerUuid.HasValue);
            if (this.PlayerUuid.HasValue)
            {
                buffer.WriteUuid(this.PlayerUuid!.Value);
            }

            return;
        }

        if (!this.PlayerUuid.HasValue)
            throw new MineSharpPacketVersionException(nameof(this.PlayerUuid), version.Version.Protocol);

        buffer.WriteUuid(this.PlayerUuid.Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var                 username   = buffer.ReadString();
        SignatureContainer? signature  = null;
        UUID?               playerUuid = null;

        if (version.Version.Protocol is >= ProtocolVersion.V_1_19 and <= ProtocolVersion.V_1_19_2)
        {
            signature = SignatureContainer.Read(buffer);
        }

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_2)
        {
            playerUuid = buffer.ReadUuid();
        }

        return new LoginStartPacket(username, signature, playerUuid);
    }

    public class SignatureContainer : ISerializable<SignatureContainer>
    {
        public long   Timestamp { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] Signature { get; set; }

        public SignatureContainer(long timestamp, byte[] publicKey, byte[] signature)
        {
            this.Timestamp = timestamp;
            this.PublicKey = publicKey;
            this.Signature = signature;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteLong(this.Timestamp);
            buffer.WriteVarInt(this.PublicKey.Length);
            buffer.WriteBytes(this.PublicKey.AsSpan());
            buffer.WriteVarInt(this.Signature.Length);
            buffer.WriteBytes(this.Signature.AsSpan());
        }

        public static SignatureContainer Read(PacketBuffer buffer)
        {
            var        timestamp = buffer.ReadLong();
            Span<byte> publicKey = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(publicKey);
            Span<byte> signature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(signature);

            return new SignatureContainer(timestamp, publicKey.ToArray(), signature.ToArray());
        }
    }
}
#pragma warning restore CS1591
