/*
 * Thanks to https://github.com/MCCTeam/Minecraft-Console-Client
 * Some code has been copied and modified from:
 *  - MinecraftClient/Protocol/Handlers/Protocol18.cs
 */

namespace MineSharp.Bot.Chat;

internal class LastSeenMessageCollector1193 : LastSeenMessageCollector
{
    private const int Capacity = 20;
    private int index;

    private AcknowledgedMessage? lastEntry;

    public LastSeenMessageCollector1193() : base(Capacity)
    {
        index = 0;
    }

    public override bool Push(AcknowledgedMessage message)
    {
        if (lastEntry != null && message.Signature.SequenceEqual(lastEntry.Signature))
        {
            return false;
        }

        lastEntry = message;

        var current = index;
        index = (current + 1) % AcknowledgedMessages.Length;
        Count++;

        AcknowledgedMessages[current] = message;

        return true;
    }

    public byte[] Collect(out int count, out AcknowledgedMessage[] acknowledgedMessages)
    {
        count = ResetCount();
        var bitset = new byte[3];
        var acknowledgedMessagesList = new List<AcknowledgedMessage>();

        for (var i = 0; i < AcknowledgedMessages.Length; i++)
        {
            var j = (index + i) % AcknowledgedMessages.Length;
            var message = AcknowledgedMessages[j];

            if (message == null)
            {
                continue;
            }

            bitset[i / 8] |= (byte)(1 << (i % 8));
            acknowledgedMessagesList.Add(message);

            message.Pending = false;
            AcknowledgedMessages[j] = message;
        }

        acknowledgedMessages = acknowledgedMessagesList.ToArray();
        return bitset;
    }

    public int ResetCount()
    {
        var count = Count;
        Count = 0;
        return count;
    }
}
