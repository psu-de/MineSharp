using MineSharp.Auth;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;
using MineSharp.Protocol.Packets.Serverbound.Login;
using static MineSharp.Protocol.Packets.Serverbound.Login.LoginStartPacket;

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
        var username = session.Username;
        Uuid? uuid = session.OnlineSession || data.Version.Protocol >= ProtocolVersion.V_1_20_2
            ? session.Uuid
            : null;

        if (data.Version.Protocol >= ProtocolVersion.V_1_20_2)
        {
            return new LoginStartPacketV_1_20_2(username, uuid!.Value);
        }
        else if (data.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            return new LoginStartPacketV_1_19_3(username, uuid);
        }
        else if (data.Version.Protocol >= ProtocolVersion.V_1_19_0)
        {
            var signature = session.Certificate == null ? null :
                new SignatureContainer(
                    new DateTimeOffset(session.Certificate.ExpiresAt).ToUnixTimeMilliseconds(),
                    session.Certificate.Keys.PublicKey,
                    data.Version.Protocol >= ProtocolVersion.V_1_19_1
                        ? session.Certificate.PublicKeySignatureV2
                        : session.Certificate.PublicKeySignature
                );
            return new LoginStartPacketV_1_19_0(username, signature, uuid);
        }

        return new LoginStartPacketV_1_7_0(username);
    }
}
