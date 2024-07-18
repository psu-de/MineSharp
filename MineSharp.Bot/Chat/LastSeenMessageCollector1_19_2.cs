/*
 * Thanks to https://github.com/MCCTeam/Minecraft-Console-Client
 * Some code has been copied and modified from:
 *  - MinecraftClient/Protocol/Handlers/Protocol18.cs
 */

namespace MineSharp.Bot.Chat;

internal class LastSeenMessageCollector1192 : LastSeenMessageCollector
{
    private const int Capacity = 5;

    public LastSeenMessageCollector1192() : base(Capacity)
    { }

    public int PendingAcknowledgements { get; set; }

    public override bool Push(AcknowledgedMessage message)
    {
        var last = message;
        for (var i = 0; i < Count; i++)
        {
            var entry = AcknowledgedMessages[i]!;
            AcknowledgedMessages[i] = last;
            last = entry;

            if (message.Sender != entry.Sender)
            {
                continue;
            }

            last = null;
            break;
        }

        if (last != null && Count < AcknowledgedMessages.Length)
        {
            AcknowledgedMessages[Count++] = last;
        }

        LastSeenMessages = new AcknowledgedMessage[Count];
        Array.Copy(AcknowledgedMessages, LastSeenMessages, Count);

        return false;
    }

    public AcknowledgedMessage[] AcknowledgeMessages()
    {
        PendingAcknowledgements = 0;
        return LastSeenMessages;
    }
}
