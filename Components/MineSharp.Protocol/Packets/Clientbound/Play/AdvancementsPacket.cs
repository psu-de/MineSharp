using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class AdvancementsPacket : Packet {

        public bool ResetClear { get; private set; }
        public Dictionary<Identifier, Advancement> AdvancementMapping { get; private set; }
        public Identifier[]? Identifiers { get; private set; }
        public Dictionary<Identifier, (Identifier, (bool, long?))[]> Progress { get; private set; }

        public AdvancementsPacket() { }

        public override void Read(PacketBuffer buffer) {
            this.ResetClear = buffer.ReadBoolean();

            int nAdvancements = buffer.ReadVarInt();
            AdvancementMapping = new Dictionary<Identifier, Advancement>();
            for (int i = 0; i < nAdvancements; i++) AdvancementMapping.Add(buffer.ReadIdentifier(), ReadAdvancement(buffer));

            this.Identifiers = buffer.ReadIdentifierArray();

            int nProgress = buffer.ReadVarInt();
            Progress = new Dictionary<Identifier, (Identifier, (bool, long?))[]>();
            for (int i = 0; i < nProgress; i++) Progress.Add(buffer.ReadIdentifier(), ReadProgress(buffer));
        }


        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }


        private Advancement ReadAdvancement(PacketBuffer buffer) {
            bool hasParent = buffer.ReadBoolean();
            Identifier? parent = null;
            if (hasParent) parent = buffer.ReadIdentifier();
            bool hasDisplay = buffer.ReadBoolean();
            Advancement.Display? displayData = null;
            if (hasDisplay) displayData = ReadDisplay(buffer);
            Identifier[] criteria = buffer.ReadIdentifierArray();
            int len = buffer.ReadVarInt();
            string[][] requirements = new string[len][];
            for (int i = 0; i < len; i++) requirements[i] = buffer.ReadStringArray();
            return new Advancement(hasParent, parent, hasDisplay, displayData, criteria, requirements);
        }

        private Advancement.Display ReadDisplay(PacketBuffer buffer) {
            Chat title = buffer.ReadChat();
            Chat description = buffer.ReadChat();
            Slot icon = buffer.ReadSlot();
            Advancement.Display.FrameType type = (Advancement.Display.FrameType)buffer.ReadVarInt();
            int flags = buffer.ReadInt();
            Identifier? background = null;
            if ((flags & 0x01) == 0x01) background = buffer.ReadIdentifier();
            float x = buffer.ReadFloat();
            float y = buffer.ReadFloat();
            return new Advancement.Display(title, description, icon, type, flags, background, x, y);
        }


        private (Identifier, (bool, long?))[] ReadProgress(PacketBuffer buffer) {
            int length = buffer.ReadVarInt();
            (Identifier, (bool, long?))[] array = new (Identifier, (bool, long?))[length];
            for (int i = 0; i < length; i++) {
                Identifier identifier = buffer.ReadIdentifier();
                bool achived = buffer.ReadBoolean();
                long? date = achived ? buffer.ReadLong() : null;
                array[i] = (identifier, (achived, date));
            }
            return array;
        }
    }
}