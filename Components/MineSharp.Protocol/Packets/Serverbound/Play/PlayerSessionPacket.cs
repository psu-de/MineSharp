using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class PlayerSessionPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_ChatSessionUpdate;
    
    public UUID SessionId { get; set; }
    public long ExpiresAt { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] KeySignature { get; set; }

    public PlayerSessionPacket(UUID sessionId, long expiresAt, byte[] publicKey, byte[] keySignature)
    {
        this.SessionId = sessionId;
        this.ExpiresAt = expiresAt;
        this.PublicKey = publicKey;
        this.KeySignature = keySignature;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteUuid(this.SessionId);
        buffer.WriteLong(this.ExpiresAt);
        buffer.WriteVarInt(this.PublicKey.Length);
        buffer.WriteBytes(this.PublicKey);
        buffer.WriteVarInt(this.KeySignature.Length);
        buffer.WriteBytes(this.KeySignature);
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
