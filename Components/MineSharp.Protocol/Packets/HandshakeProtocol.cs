using MineSharp.Auth;
using MineSharp.Data;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;
using MineSharp.Protocol.Packets.Serverbound.Login;

namespace MineSharp.Protocol.Packets;

internal static class HandshakeProtocol
{
    public static async Task PerformHandshake(MinecraftClient client, GameState next, MinecraftData data)
    {
        if (next is GameState.PLAY or GameState.HANDSHAKING)
        {
            throw new ArgumentException("Next state must either be LOGIN or STATUS.");
        }
        
        var handshake = new HandshakePacket(data.Protocol.Version, client.IP.ToString(), client.Port, next);
        await client.SendPacket(handshake);

        if (next == GameState.STATUS)
        {
            return;
        }
        
        var login = GetLoginPacket(data, client.Session);
        await client.SendPacket(login);
    }
    
    private static LoginStartPacket GetLoginPacket(MinecraftData data, Session session)
    {
        LoginStartPacket.SignatureContainer? signature = null;
        
        if (data.Protocol.Version >= ProtocolVersion.V_1_19 && !data.Features.Supports("useChatSessions")
                                                            && session.Certificate != null)
        {
            signature = new LoginStartPacket.SignatureContainer(
                new DateTimeOffset(session.Certificate.ExpiresAt).ToUnixTimeMilliseconds(),
                session.Certificate.Keys.PublicKey,
                data.Protocol.Version >= ProtocolVersion.V_1_19_2
                    ? session.Certificate.PublicKeySignatureV2
                    : session.Certificate.PublicKeySignature
            );
        }

        return new LoginStartPacket(session.Username, signature, session.OnlineSession ? session.UUID : null);
    }
}
