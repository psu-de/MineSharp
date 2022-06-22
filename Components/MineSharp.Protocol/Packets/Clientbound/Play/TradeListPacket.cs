using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class TradeListPacket : Packet {

        public int WindowId { get; private set; }
        public VillagerTrade[] Trades { get; private set; }
        public VillagerLevel VillagerLevel { get; private set; }
        public int Experience { get; private set; }
        public bool IsRegularVillager { get; private set; }
        public bool CanRestock { get; private set; }

        public TradeListPacket() { }

        public TradeListPacket(int windowId, VillagerTrade[] trades, VillagerLevel villagerLevel, int experience, bool isRegularVillager, bool canRestock) {
            WindowId = windowId;
            Trades = trades;
            VillagerLevel = villagerLevel;
            Experience = experience;
            IsRegularVillager = isRegularVillager;
            CanRestock = canRestock;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadVarInt();
            byte size = buffer.ReadByte();
            Trades = new VillagerTrade[size];
            for (int i = 0; i < size; i++) Trades[i] = ReadTrade(buffer);
            this.VillagerLevel = (VillagerLevel)buffer.ReadVarInt();
            this.Experience = buffer.ReadVarInt();
            this.IsRegularVillager = buffer.ReadBoolean();
            this.CanRestock = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }

        private VillagerTrade ReadTrade(PacketBuffer buffer) {
            Slot in1 = buffer.ReadSlot();
            Slot ou = buffer.ReadSlot();
            bool hasSec = buffer.ReadBoolean();
            Slot? in2 = null;
            if (hasSec) in2 = buffer.ReadSlot();
            bool disabled = buffer.ReadBoolean();
            int uses = buffer.ReadInt();
            int maxUses = buffer.ReadInt();
            int xp = buffer.ReadInt();
            int specialPrice = buffer.ReadInt();
            int priceMultiplier = buffer.ReadInt();
            int demand = buffer.ReadInt();
            return new VillagerTrade(in1, ou, hasSec, in2, disabled, uses, maxUses, xp, specialPrice, priceMultiplier, demand);
        }
    }
}
