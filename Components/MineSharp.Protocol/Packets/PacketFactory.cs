using MineSharp.Core.Logging;

namespace MineSharp.Protocol.Packets {
    internal class PacketFactory {

        private static Logger Logger = Logger.GetLogger();
        private MinecraftClient client;

        public PacketFactory(MinecraftClient client) {
            this.client = client;
        }


        public Packet? BuildPacket (byte[] data, int uncompressedLength) {
            PacketBuffer packetBuffer;
            if (uncompressedLength > 0) packetBuffer = PacketBuffer.Decompress(data, uncompressedLength);
            else packetBuffer = new PacketBuffer(data);

            int packetId = packetBuffer.ReadVarInt();
            Type? packetType = Packet.GetPacketType(this.client.GameState, PacketFlow.CLIENTBOUND, packetId);


            if (packetType != null) {
                //Logger.Debug($"Received Packet of type {packetType.Name}");
                Packet? packet = Activator.CreateInstance(packetType) as Packet;
                if (packet == null) {
                    Logger.Error("Could not create instance of packet");
                } else {
                    try {
                        packet.Read(packetBuffer);
                        if (packetBuffer.ReadableBytes > 0) Logger.Debug3($"PacketBuffer should be empty after reading ({packet.GetType().Name})"); //throw new Exception("PacketBuffer must be empty after reading");
                    } catch (Exception e){
                        Logger.Debug("Error reading packet: " + packet.GetType().Name);
                        Logger.Error(e.ToString());
                        return null;
                    }
                }
                return packet;
            }
            return null;
        }
    }
}
