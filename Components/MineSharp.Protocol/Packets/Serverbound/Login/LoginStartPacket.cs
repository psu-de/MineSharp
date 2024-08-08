using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Serverbound.Login;
#pragma warning disable CS1591
public sealed record LoginStartPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Login_LoginStart;

    // Here is no non-argument constructor allowed
    // Do not use
#pragma warning disable CS8618
    private LoginStartPacket()
#pragma warning restore CS8618
    {
    }

    /// <summary>
    ///     Constructor for versions before 1.19
    /// </summary>
    /// <param name="username"></param>
    public LoginStartPacket(string username)
    {
        Username = username;
    }

    /// <summary>
    ///     Constructor for versions >= 1.19.3
    /// </summary>
    /// <param name="username"></param>
    /// <param name="playerUuid"></param>
    public LoginStartPacket(string username, Uuid? playerUuid)
    {
        Username = username;
        PlayerUuid = playerUuid;
    }

    /// <summary>
    ///     Constructor for version 1.19-1.19.2
    /// </summary>
    /// <param name="username"></param>
    /// <param name="signature"></param>
    /// <param name="playerUuid"></param>
    public LoginStartPacket(string username, SignatureContainer? signature, Uuid? playerUuid = null)
    {
        Username = username;
        Signature = signature;
        PlayerUuid = playerUuid;
    }

    public string Username { get; init; }
    public SignatureContainer? Signature { get; init; }
    public Uuid? PlayerUuid { get; init; }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(Username);

        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
        {
            var hasSignature = Signature != null;
            buffer.WriteBool(hasSignature);
            Signature?.Write(buffer);
        }

        if (version.Version.Protocol < ProtocolVersion.V_1_19_2)
        {
            return;
        }

        if (version.Version.Protocol < ProtocolVersion.V_1_20_2)
        {
            buffer.WriteBool(PlayerUuid.HasValue);
            if (PlayerUuid.HasValue)
            {
                buffer.WriteUuid(PlayerUuid!.Value);
            }

            return;
        }

        if (!PlayerUuid.HasValue)
        {
            throw new MineSharpPacketVersionException(nameof(PlayerUuid), version.Version.Protocol);
        }

        buffer.WriteUuid(PlayerUuid.Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var username = buffer.ReadString();
        SignatureContainer? signature = null;
        Uuid? playerUuid = null;

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

    public sealed record SignatureContainer(long Timestamp, byte[] PublicKey, byte[] Signature) : ISerializable<SignatureContainer>
    {
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteLong(Timestamp);
            buffer.WriteVarInt(PublicKey.Length);
            buffer.WriteBytes(PublicKey.AsSpan());
            buffer.WriteVarInt(Signature.Length);
            buffer.WriteBytes(Signature.AsSpan());
        }

        public static SignatureContainer Read(PacketBuffer buffer)
        {
            var timestamp = buffer.ReadLong();
            var publicKey = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(publicKey);
            var signature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(signature);

            return new(timestamp, publicKey, signature);
        }
    }
}
#pragma warning restore CS1591
