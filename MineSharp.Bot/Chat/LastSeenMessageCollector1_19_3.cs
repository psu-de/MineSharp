/*
 * Thanks to https://github.com/MCCTeam/Minecraft-Console-Client
 * Some code has been copied and modified from:
 *  - MinecraftClient/Protocol/Handlers/Protocol18.cs
 */

namespace MineSharp.Bot.Chat;

internal class LastSeenMessageCollector1_19_3 : LastSeenMessageCollector
{
    private const int CAPACITY = 20;

    private AcknowledgedMessage? _lastEntry = null;
    private int _index;

    public LastSeenMessageCollector1_19_3() : base(CAPACITY)
    {
        this._index = 0;
    }

    public override bool Push(AcknowledgedMessage message)
    {
        if (this._lastEntry != null && message.Signature.SequenceEqual(this._lastEntry.Signature))
        {
            return false;
        }

        this._lastEntry = message;

        var current = this._index;
        this._index = (current + 1) % this.AcknowledgedMessages.Length;
        this.Count++;

        this.AcknowledgedMessages[current] = message;

        return true;
    }

    public byte[] Collect(out int count, out AcknowledgedMessage[] acknowledgedMessages)
    {
        count = this.ResetCount();
        var bitset = new byte[3];
        var acknowledgedMessagesList = new List<AcknowledgedMessage>();

        for (int i = 0; i < this.AcknowledgedMessages.Length; i++)
        {
            int j = (this._index + i) % this.AcknowledgedMessages.Length;
            AcknowledgedMessage? message = this.AcknowledgedMessages[j];
            
            if (message == null)
                continue;

            bitset[i / 8] |= (byte)(1 << i % 8);
            acknowledgedMessagesList.Add(message);

            message.Pending = false;
            this.AcknowledgedMessages[j] = message;
        }
        
        acknowledgedMessages = acknowledgedMessagesList.ToArray();
        return bitset;
    }

    public int ResetCount()
    {
        var count = this.Count;
        this.Count = 0;
        return count;
    }
}
