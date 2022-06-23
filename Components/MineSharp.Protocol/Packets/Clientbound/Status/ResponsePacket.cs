namespace MineSharp.Protocol.Packets.Clientbound.Status {
    public class ResponsePacket : Packet {


        public string? JSONResponse { get; private set; }

        public ResponsePacket() { }
        public ResponsePacket(string jsonResponse) {
            this.JSONResponse = jsonResponse;
        }

        public override void Read(PacketBuffer buffer) {
            this.JSONResponse = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString(JSONResponse!);
        }
    }
}
