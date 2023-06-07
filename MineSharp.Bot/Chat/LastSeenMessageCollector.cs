/*
 * Thanks to https://github.com/MCCTeam/Minecraft-Console-Client
 * Some code has been copied and modified from:
 *  - MinecraftClient/Protocol/Handlers/Protocol18.cs
 */
namespace MineSharp.Bot.Chat;

internal abstract class LastSeenMessageCollector
{
    protected readonly AcknowledgedMessage?[] AcknowledgedMessages;
    protected AcknowledgedMessage[] LastSeenMessages;

    public int Count { get; protected set; }

    public LastSeenMessageCollector(int capacity)
    {
        this.LastSeenMessages = Array.Empty<AcknowledgedMessage>();
        this.AcknowledgedMessages = new AcknowledgedMessage[capacity];
        this.Count = 0;
    }

    public abstract bool Push(AcknowledgedMessage message);
}
