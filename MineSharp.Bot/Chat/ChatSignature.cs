/*
 * Thanks to https://github.com/MCCTeam/Minecraft-Console-Client
 * Some code has been copied and modified from:
 *  - MinecraftClient/Protocol/Handlers/Protocol18.cs
 */

using System.Security.Cryptography;
using System.Text;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MineSharp.Bot.Chat;

internal static class ChatSignature
{
    public static byte[] SignChat1_19_3(MinecraftData data, RSA rsa, string message, Uuid playerUuid, Uuid chatSession,
                                        int messageCount,
                                        long salt, DateTimeOffset timestamp, AcknowledgedMessage[] acknowledged)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);

        var signData = new PacketBuffer(data.Version.Protocol);
        signData.WriteInt(1);
        signData.WriteUuid(playerUuid);
        signData.WriteUuid(chatSession);
        signData.WriteInt(messageCount);
        signData.WriteLong(salt);
        signData.WriteLong(timestamp.ToUnixTimeSeconds());
        signData.WriteInt(messageBytes.Length);
        signData.WriteBytes(messageBytes);
        signData.WriteInt(acknowledged.Length);
        foreach (var ack in acknowledged)
        {
            signData.WriteBytes(ack.Signature);
        }

        return SignChatData(rsa, signData.GetBuffer());
    }

    public static byte[] SignChat1_19_2(RSA rsa, string message, Uuid playerUuid, DateTimeOffset timestamp, long salt,
                                        AcknowledgedMessage[] messageList, byte[]? precedingSignature)
    {
        var hashData = new PacketBuffer(0);
        hashData.WriteLong(salt);
        hashData.WriteLong(timestamp.ToUnixTimeSeconds());
        hashData.WriteBytes(Encoding.UTF8.GetBytes(message));
        hashData.WriteByte(70);

        foreach (var ack in messageList)
        {
            hashData.WriteByte(70);
            hashData.WriteUuid(ack.Sender);
            hashData.WriteBytes(ack.Signature);
        }

        var hash = SHA256.HashData(hashData.GetBuffer());

        var signData = new PacketBuffer(0);
        if (precedingSignature != null)
        {
            signData.WriteBytes(precedingSignature);
        }

        signData.WriteUuid(playerUuid);
        signData.WriteBytes(hash);

        return SignChatData(rsa, signData.GetBuffer());
    }

    public static byte[] SignChat1_19_1(RSA rsa, string message, Uuid uuid, DateTimeOffset timestamp, long salt)
    {
        var json = new JObject(new JProperty("text", message)).ToString(Formatting.None);
        var signData = new PacketBuffer(0);

        signData.WriteLong(salt);
        signData.WriteUuid(uuid);
        signData.WriteLong(timestamp.ToUnixTimeSeconds());
        signData.WriteBytes(Encoding.UTF8.GetBytes(json));

        return SignChatData(rsa, signData.GetBuffer());
    }

    public static long GenerateSalt()
    {
        return ((long)RandomNumberGenerator.GetInt32(int.MaxValue) << 32) |
            (uint)RandomNumberGenerator.GetInt32(int.MaxValue);
    }

    private static byte[] SignChatData(RSA rsa, byte[] data)
    {
        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}
