using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class PlayerSessionPacket : IPacket
{
    public PlayerSessionPacket(Uuid sessionId, long expiresAt, byte[] publicKey, byte[] keySignature)
    {
        SessionId = sessionId;
        ExpiresAt = expiresAt;
        PublicKey = publicKey;
        KeySignature = keySignature;
    }

    public Uuid SessionId { get; set; }
    public long ExpiresAt { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] KeySignature { get; set; }
    public PacketType Type => PacketType.SB_Play_ChatSessionUpdate;

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

        return new PlayerSessionPacket(
            sessionId, expiresAt, publicKey, keySignature);
    }
}
#pragma warning restore CS1591
