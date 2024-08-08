using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record PlayerSessionPacket(Uuid SessionId, long ExpiresAt, byte[] PublicKey, byte[] KeySignature) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ChatSessionUpdate;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteUuid(SessionId);
        buffer.WriteLong(ExpiresAt);
        buffer.WriteVarInt(PublicKey.Length);
        buffer.WriteBytes(PublicKey);
        buffer.WriteVarInt(KeySignature.Length);
        buffer.WriteBytes(KeySignature);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var sessionId = buffer.ReadUuid();
        var expiresAt = buffer.ReadLong();
        var publicKey = new byte[buffer.ReadVarInt()];
        buffer.ReadBytes(publicKey);
        var keySignature = new byte[buffer.ReadVarInt()];
        buffer.ReadBytes(keySignature);

        return new PlayerSessionPacket(sessionId, expiresAt, publicKey, keySignature);
    }
}
#pragma warning restore CS1591
