using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class TagsPacket : Packet {

        public (Identifier, (Identifier, int[])[])[]? Data { get; private set; }

        public TagsPacket() { }

        public TagsPacket((Identifier, (Identifier, int[])[])[] data) {
            this.Data = data;
        }

        public override void Read(PacketBuffer buffer) {
            int length = buffer.ReadVarInt();
            (Identifier, (Identifier, int[])[])[] data = new (Identifier, (Identifier, int[])[])[length];

            for (int i = 0; i < length; i++) {
                Identifier tagType = buffer.ReadIdentifier();
                int length2 = buffer.ReadVarInt();
                (Identifier, int[])[] element = new (Identifier, int[])[length2];

                for (int j = 0; j < length2; j++) {
                    Identifier tagName = buffer.ReadIdentifier();
                    int count = buffer.ReadVarInt();
                    int[] entries = new int[count];
                    for (int k = 0; k < count; k++) entries[k] = buffer.ReadVarInt();
                    element[j] = (tagName, entries);
                }
                data[i] = (tagType, element);
            }
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotSupportedException();
        }
    }
}