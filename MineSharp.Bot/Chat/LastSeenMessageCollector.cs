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

    public LastSeenMessageCollector(int capacity)
    {
        LastSeenMessages = Array.Empty<AcknowledgedMessage>();
        AcknowledgedMessages = new AcknowledgedMessage[capacity];
        Count = 0;
    }

    public int Count { get; protected set; }

    public abstract bool Push(AcknowledgedMessage message);
}
