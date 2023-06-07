using MineSharp.Data.Providers;
using MineSharp.Protocol.Packets;
using System.Diagnostics;

namespace MineSharp.Protocol.Extensions;

public static class ProtocolDataExtensions
{
    public static int GetPacketId(this ProtocolDataProvider data, string name, GameState state, PacketFlow flow)
    {
        return state switch {
            GameState.HANDSHAKING => flow == PacketFlow.Clientbound ? data.GetClientHandshakePacketId(name) : data.GetServerHandshakePacketId(name),
            GameState.LOGIN => flow == PacketFlow.Clientbound ? data.GetClientLoginPacketId(name) : data.GetServerLoginPacketId(name),
            GameState.STATUS => flow == PacketFlow.Clientbound ? data.GetClientStatusPacketId(name) : data.GetServerStatusPacketId(name),
            GameState.PLAY => flow == PacketFlow.Clientbound ? data.GetClientPlayPacketId(name) : data.GetServerPlayPacketId(name),
            _ => throw new UnreachableException()
        };
    }
    
    
    public static string GetPacketName(this ProtocolDataProvider data, int id, GameState state, PacketFlow flow)
    {
        return state switch {
            GameState.HANDSHAKING => flow == PacketFlow.Clientbound ? data.GetClientHandshakePacketName(id) : data.GetServerHandshakePacketName(id),
            GameState.LOGIN => flow == PacketFlow.Clientbound ? data.GetClientLoginPacketName(id) : data.GetServerLoginPacketName(id),
            GameState.STATUS => flow == PacketFlow.Clientbound ? data.GetClientStatusPacketName(id) : data.GetServerStatusPacketName(id),
            GameState.PLAY => flow == PacketFlow.Clientbound ? data.GetClientPlayPacketName(id) : data.GetServerPlayPacketName(id),
            _ => throw new UnreachableException()
        };
    }
}
