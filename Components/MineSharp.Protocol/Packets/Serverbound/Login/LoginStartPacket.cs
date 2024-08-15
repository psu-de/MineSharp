using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Login;

public abstract record LoginStartPacket : IPacketStatic<LoginStartPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Login_LoginStart;

    // all versions contain these fields:
    public abstract string Username { get; init; }

    // may only be called from sub class in this class
    private LoginStartPacket()
    {
    }

    public sealed record LoginStartPacketV_1_7_0(string Username) : LoginStartPacket, IPacketVersionSubTypeStatic<LoginStartPacketV_1_7_0, LoginStartPacket>
    {
        /// <inheritdoc />
        public ProtocolVersion FirstVersionUsed => FirstVersionUsedStatic;
        /// <inheritdoc />
        public static ProtocolVersion FirstVersionUsedStatic => ProtocolVersion.V_1_7_0;

        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteString(Username);
        }

        /// <inheritdoc />
        public static new LoginStartPacketV_1_7_0 Read(PacketBuffer buffer, MinecraftData data)
        {
            var username = buffer.ReadString();
            return new(username);
        }

        static IPacket IPacketVersionSubTypeStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public sealed record LoginStartPacketV_1_19_0(
        string Username,
        SignatureContainer? Signature,
        Uuid? PlayerUuid
    ) : LoginStartPacket, IPacketVersionSubTypeStatic<LoginStartPacketV_1_19_0, LoginStartPacket>
    {
        /// <inheritdoc />
        public ProtocolVersion FirstVersionUsed => FirstVersionUsedStatic;
        /// <inheritdoc />
        public static ProtocolVersion FirstVersionUsedStatic => ProtocolVersion.V_1_19_0;

        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteString(Username);

            var hasSignature = Signature != null;
            buffer.WriteBool(hasSignature);
            if (hasSignature)
            {
                Signature!.Write(buffer);
            }

            WriteOptionalUuid(buffer, PlayerUuid);
        }

        /// <inheritdoc />
        public static new LoginStartPacketV_1_19_0 Read(PacketBuffer buffer, MinecraftData data)
        {
            var username = buffer.ReadString();

            SignatureContainer? signature = null;
            var hasSignature = buffer.ReadBool();
            if (hasSignature)
            {
                signature = SignatureContainer.Read(buffer);
            }

            Uuid? playerUuid = ReadOptionalUuid(buffer);
            return new(username, signature, playerUuid);
        }

        static IPacket IPacketVersionSubTypeStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public sealed record LoginStartPacketV_1_19_3(
        string Username,
        Uuid? PlayerUuid
    ) : LoginStartPacket, IPacketVersionSubTypeStatic<LoginStartPacketV_1_19_3, LoginStartPacket>
    {
        /// <inheritdoc />
        public ProtocolVersion FirstVersionUsed => FirstVersionUsedStatic;
        /// <inheritdoc />
        public static ProtocolVersion FirstVersionUsedStatic => ProtocolVersion.V_1_19_3;

        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteString(Username);
            WriteOptionalUuid(buffer, PlayerUuid);
        }

        /// <inheritdoc />
        public static new LoginStartPacketV_1_19_3 Read(PacketBuffer buffer, MinecraftData data)
        {
            var username = buffer.ReadString();
            Uuid? playerUuid = ReadOptionalUuid(buffer);
            return new(username, playerUuid);
        }

        static IPacket IPacketVersionSubTypeStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public sealed record LoginStartPacketV_1_20_2(
        string Username,
        Uuid PlayerUuid
    ) : LoginStartPacket, IPacketVersionSubTypeStatic<LoginStartPacketV_1_20_2, LoginStartPacket>
    {
        /// <inheritdoc />
        public ProtocolVersion FirstVersionUsed => FirstVersionUsedStatic;
        /// <inheritdoc />
        public static ProtocolVersion FirstVersionUsedStatic => ProtocolVersion.V_1_20_2;

        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteString(Username);
            buffer.WriteUuid(PlayerUuid);
        }

        /// <inheritdoc />
        public static new LoginStartPacketV_1_20_2 Read(PacketBuffer buffer, MinecraftData data)
        {
            var username = buffer.ReadString();
            var playerUuid = buffer.ReadUuid();
            return new(username, playerUuid);
        }

        static IPacket IPacketVersionSubTypeStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    private static void WriteOptionalUuid(PacketBuffer buffer, Uuid? uuid)
    {
        var hasUuid = uuid.HasValue;
        buffer.WriteBool(hasUuid);
        if (hasUuid)
        {
            buffer.WriteUuid(uuid!.Value);
        }
    }

    private static Uuid? ReadOptionalUuid(PacketBuffer buffer)
    {
        Uuid? uuid = null;
        var hasUuid = buffer.ReadBool();
        if (hasUuid)
        {
            uuid = buffer.ReadUuid();
        }
        return uuid;
    }

    public static readonly PacketVersionSubTypeLookup<LoginStartPacket> PacketVersionSubTypeLookup = InitializeVersionPackets();

    private static PacketVersionSubTypeLookup<LoginStartPacket> InitializeVersionPackets()
    {
        PacketVersionSubTypeLookup<LoginStartPacket> lookup = new();

        lookup.RegisterVersionPacket<LoginStartPacketV_1_7_0>();
        lookup.RegisterVersionPacket<LoginStartPacketV_1_19_0>();
        lookup.RegisterVersionPacket<LoginStartPacketV_1_19_3>();
        lookup.RegisterVersionPacket<LoginStartPacketV_1_20_2>();

        lookup.Freeze();
        return lookup;
    }

    /// <inheritdoc />
    public abstract void Write(PacketBuffer buffer, MinecraftData data);

    /// <inheritdoc />
    public static LoginStartPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return PacketVersionSubTypeLookup.Read(buffer, data);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }

    public sealed record SignatureContainer(long Timestamp, byte[] PublicKey, byte[] Signature) : ISerializable<SignatureContainer>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteLong(Timestamp);
            buffer.WriteVarInt(PublicKey.Length);
            buffer.WriteBytes(PublicKey);
            buffer.WriteVarInt(Signature.Length);
            buffer.WriteBytes(Signature);
        }

        /// <inheritdoc />
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
