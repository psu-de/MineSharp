using MineSharp.Auth;
using MineSharp.Core;
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
        if (!(next is GameState.Status or GameState.Login))
        {
            throw new ArgumentException($"{nameof(next)} must either be {GameState.Status} or {GameState.Login}");
        }

        var handshake = new HandshakePacket(data.Version.Protocol, client.Hostname, client.Port, next);
        await client.ChangeGameState(GameState.Handshaking);
        await client.SendPacket(handshake);

        await client.ChangeGameState(next);
        if (next == GameState.Status)
        {
            return;
        }
    }

    internal static LoginStartPacket GetLoginPacket(MinecraftData data, Session session)
    {
        LoginStartPacket.SignatureContainer? signature = null;

        if (data.Version.Protocol >= ProtocolVersion.V_1_19_3 && session.Certificate != null)
        {
            signature = new(
                new DateTimeOffset(session.Certificate.ExpiresAt).ToUnixTimeMilliseconds(),
                session.Certificate.Keys.PublicKey,
                data.Version.Protocol >= ProtocolVersion.V_1_19_2
                    ? session.Certificate.PublicKeySignatureV2
                    : session.Certificate.PublicKeySignature
            );
        }

        Uuid? uuid = session.OnlineSession || data.Version.Protocol >= ProtocolVersion.V_1_20_2
            ? session.Uuid
            : null;

        return new(session.Username, signature, uuid);
    }
}
