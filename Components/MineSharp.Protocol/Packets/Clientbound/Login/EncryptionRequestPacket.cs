using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

public class EncryptionRequestPacket : IPacket
{
    public static int Id => 0x01;

    public string ServerId { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] VerifyToken { get; set; }

    public EncryptionRequestPacket(string serverId, byte[] publicKey, byte[] verifyToken)
    {
        this.ServerId = serverId;
        this.PublicKey = publicKey;
        this.VerifyToken = verifyToken;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteString(ServerId);
        buffer.WriteVarInt(this.PublicKey.Length);
        buffer.WriteBytes(this.PublicKey.AsSpan());
        buffer.WriteVarInt(this.VerifyToken.Length);
        buffer.WriteBytes(this.VerifyToken.AsSpan());
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var serverId = buffer.ReadString();
        Span<byte> publicKey = stackalloc byte[buffer.ReadVarInt()];
        buffer.ReadBytes(publicKey);
        Span<byte> verifyToken = stackalloc byte[buffer.ReadVarInt()];
        buffer.ReadBytes(verifyToken);

        return new EncryptionRequestPacket(serverId, publicKey.ToArray(), verifyToken.ToArray());
    }
}
