namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class ClientStatusPacket : Packet {

        public ClientStatusAction ActionID { get; private set; }

        public ClientStatusPacket() { }

        public ClientStatusPacket(ClientStatusAction actionid) {
            this.ActionID = actionid;
        }

        public override void Read(PacketBuffer buffer) {
            this.ActionID = (ClientStatusAction)buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt((int)this.ActionID);
        }

        public enum ClientStatusAction {
            PerformRespawn = 0,
            RequestStats = 1
        }
    }
}