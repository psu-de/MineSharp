using MineSharp.Auth;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;
using MineSharp.Protocol.Packets.Serverbound.Login;

namespace MineSharp.Protocol.Packets;

internal static class HandshakeProtocol
{
    public static async Task PerformHandshake(MinecraftClient client, GameState next, MinecraftData data)
    {
        if (next is GameState.Play or GameState.Handshaking)
        {
            throw new ArgumentException("Next state must either be Login or Status.");
        }

        var handshake = new HandshakePacket(data.Version.Protocol, client.Hostname, client.Port, next);
        await client.SendPacket(handshake);

        if (next == GameState.Status)
        {
            return;
        }

        var login = GetLoginPacket(data, client.Session);
        await client.SendPacket(login);
    }

    private static LoginStartPacket GetLoginPacket(MinecraftData data, Session session)
    {
        LoginStartPacket.SignatureContainer? signature = null;

        if (data.Version.Protocol >= ProtocolVersion.V_1_19_3 && session.Certificate != null)
        {
            signature = new LoginStartPacket.SignatureContainer(
                new DateTimeOffset(session.Certificate.ExpiresAt).ToUnixTimeMilliseconds(),
                session.Certificate.Keys.PublicKey,
                data.Version.Protocol >= ProtocolVersion.V_1_19_2
                    ? session.Certificate.PublicKeySignatureV2
                    : session.Certificate.PublicKeySignature
            );
        }

        UUID? uuid = session.OnlineSession || data.Version.Protocol >= ProtocolVersion.V_1_20_2
            ? session.UUID
            : null;

        return new LoginStartPacket(session.Username, signature, uuid);
    }
}
