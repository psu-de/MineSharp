using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Serverbound.Handshaking;

public class HandshakePacket : IPacket
{
    public static int Id => 0x00;

    public int ProtocolVersion { get; set; }
    public string Host { get; set; }
    public ushort Port { get; set; }
    public GameState NextState { get; set; }

    public HandshakePacket(int protocolVersion, string host, ushort port, GameState nextState)
    {
        this.ProtocolVersion = protocolVersion;
        this.Host = host;
        this.Port = port;
        this.NextState = nextState;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.ProtocolVersion);
        buffer.WriteString(this.Host);
        buffer.WriteUShort(this.Port);
        buffer.WriteVarInt((int)this.NextState);
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var protocolVersion = buffer.ReadVarInt();
        var host = buffer.ReadString();
        var port = buffer.ReadUShort();
        var nextState = (GameState)buffer.ReadVarInt();

        return new HandshakePacket(protocolVersion, host, port, nextState);
    }
}
