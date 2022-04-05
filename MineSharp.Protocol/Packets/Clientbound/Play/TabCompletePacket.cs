using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class TabCompletePacket : Packet {

        public int ID { get; private set; }
        public int Start { get; private set; }
        public int Length { get; private set; }
        public (string, bool, Chat?)[] Matches { get; private set; }

        public TabCompletePacket() { }

        public TabCompletePacket(int iD, int start, int length, (string, bool, Chat?)[] matches) {
            ID = iD;
            Start = start;
            Length = length;
            Matches = matches;
        }

        public override void Read(PacketBuffer buffer) {
            this.ID = buffer.ReadVarInt();
            this.Start = buffer.ReadVarInt();
            this.Length = buffer.ReadVarInt();

            int count = buffer.ReadVarInt();
            (string, bool, Chat?)[] data = new (string, bool, Chat?)[count];
            for (int i = 0; i < count; i++) {
                string match = buffer.ReadString();
                bool hasTooltip = buffer.ReadBoolean();
                Chat? tooltip = null;
                if (hasTooltip) tooltip = buffer.ReadChat();
                data[i] = (match, hasTooltip, tooltip);
            }
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }
    }
}
