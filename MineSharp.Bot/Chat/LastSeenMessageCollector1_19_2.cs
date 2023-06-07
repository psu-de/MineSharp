/*
 * Thanks to https://github.com/MCCTeam/Minecraft-Console-Client
 * Some code has been copied and modified from:
 *  - MinecraftClient/Protocol/Handlers/Protocol18.cs
 */

namespace MineSharp.Bot.Chat;

internal class LastSeenMessageCollector1_19_2 : LastSeenMessageCollector
{
    private const int CAPACITY = 5;

    public int PendingAcknowledgements { get; set; } = 0;

    public LastSeenMessageCollector1_19_2() : base(CAPACITY)
    { }
    
    public override bool Push(AcknowledgedMessage message)
    {
        var last = message;
        for (int i = 0; i < this.Count; i++)
        {
            var entry = this.AcknowledgedMessages[i]!;
            this.AcknowledgedMessages[i] = last;
            last = entry;

            if (message.Sender != entry.Sender)
                continue;

            last = null;
            break;
        }

        if (last != null && this.Count < this.AcknowledgedMessages.Length)
            this.AcknowledgedMessages[this.Count++] = last;

        this.LastSeenMessages = new AcknowledgedMessage[Count];
        Array.Copy(this.AcknowledgedMessages, this.LastSeenMessages, this.Count);
        
        return false;
    }

    public AcknowledgedMessage[] AcknowledgeMessages()
    {
        this.PendingAcknowledgements = 0;
        return this.LastSeenMessages;
    }
}
