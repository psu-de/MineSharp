using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class DeathCombatEventPacket : Packet {

        public int PlayerID { get; private set; }
        public int EntityID { get; private set; }
        public Chat? Message { get; private set; }

        public DeathCombatEventPacket() { }

        public DeathCombatEventPacket(int playerid, int entityid, Chat message) {
            this.PlayerID = playerid;
            this.EntityID = entityid;
            this.Message = message;
        }

        public override void Read(PacketBuffer buffer) {
            this.PlayerID = buffer.ReadVarInt();
            this.EntityID = buffer.ReadInt();
            this.Message = buffer.ReadChat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.PlayerID);
            buffer.WriteInt(this.EntityID);
            buffer.WriteChat(this.Message!);
        }
    }
}