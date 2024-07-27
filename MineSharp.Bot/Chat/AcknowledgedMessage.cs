using MineSharp.Core.Common;
using MineSharp.Protocol.Packets.NetworkTypes;

/*
 * Thanks to https://github.com/MCCTeam/Minecraft-Console-Client
 * Some code has been copied and modified from:
 *  - MinecraftClient/Protocol/Handlers/Protocol18.cs
 */
namespace MineSharp.Bot.Chat;

internal class AcknowledgedMessage
{
    public AcknowledgedMessage(bool pending, Uuid sender, byte[] signature)
    {
        Pending = pending;
        Sender = sender;
        Signature = signature;
    }

    public bool Pending { get; set; }
    public Uuid Sender { get; set; }
    public byte[] Signature { get; set; }

    public ChatMessageItem ToProtocolMessage()
    {
        return new(
            Sender, Signature);
    }
}
