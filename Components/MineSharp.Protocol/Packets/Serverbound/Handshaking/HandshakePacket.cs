using MineSharp.Core.Common;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Handshaking;

public class HandshakePacket : IPacket
{
    public PacketType Type => PacketType.SB_Handshake_SetProtocol;

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

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.ProtocolVersion);
        buffer.WriteString(this.Host);
        buffer.WriteUShort(this.Port);
        buffer.WriteVarInt((int)this.NextState);
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var protocolVersion = buffer.ReadVarInt();
        var host = buffer.ReadString();
        var port = buffer.ReadUShort();
        var nextState = (GameState)buffer.ReadVarInt();

        return new HandshakePacket(protocolVersion, host, port, nextState);
    }
}
