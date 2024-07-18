using MineSharp.Core.Common;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Handshaking;

#pragma warning disable CS1591

public class HandshakePacket : IPacket
{
    public HandshakePacket(int protocolVersion, string host, ushort port, GameState nextState)
    {
        ProtocolVersion = protocolVersion;
        Host = host;
        Port = port;
        NextState = nextState;
    }

    public int ProtocolVersion { get; set; }
    public string Host { get; set; }
    public ushort Port { get; set; }
    public GameState NextState { get; set; }
    public PacketType Type => PacketType.SB_Handshake_SetProtocol;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(ProtocolVersion);
        buffer.WriteString(Host);
        buffer.WriteUShort(Port);
        buffer.WriteVarInt((int)NextState);
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

#pragma warning restore CS1591
