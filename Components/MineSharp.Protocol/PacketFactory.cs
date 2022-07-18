using ICSharpCode.SharpZipLib.Zip.Compression;
using MineSharp.Core.Logging;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol {
    internal class PacketFactory {

        private static Logger Logger = Logger.GetLogger();
        private MinecraftClient client;
        private Dictionary<GameState, IPacketFactory> ClientboundPacketFactories;
        private Dictionary<GameState, IPacketFactory> ServerboundPacketFactories;

        public PacketFactory(MinecraftClient client) {
            this.client = client;

            this.ClientboundPacketFactories = new Dictionary<GameState, IPacketFactory>();
            this.ClientboundPacketFactories.Add(GameState.HANDSHAKING, new MineSharp.Data.Protocol.Handshaking.Clientbound.HandshakingPacketFactory());
            this.ClientboundPacketFactories.Add(GameState.STATUS, new MineSharp.Data.Protocol.Status.Clientbound.StatusPacketFactory());
            this.ClientboundPacketFactories.Add(GameState.LOGIN, new MineSharp.Data.Protocol.Login.Clientbound.LoginPacketFactory());
            this.ClientboundPacketFactories.Add(GameState.PLAY, new MineSharp.Data.Protocol.Play.Clientbound.PlayPacketFactory());


            this.ServerboundPacketFactories = new Dictionary<GameState, IPacketFactory>();
            this.ServerboundPacketFactories.Add(GameState.HANDSHAKING, new MineSharp.Data.Protocol.Handshaking.Serverbound.HandshakingPacketFactory());
            this.ServerboundPacketFactories.Add(GameState.STATUS, new MineSharp.Data.Protocol.Status.Serverbound.StatusPacketFactory());
            this.ServerboundPacketFactories.Add(GameState.LOGIN, new MineSharp.Data.Protocol.Login.Serverbound.LoginPacketFactory());
            this.ServerboundPacketFactories.Add(GameState.PLAY, new MineSharp.Data.Protocol.Play.Serverbound.PlayPacketFactory());
        }

        public static PacketBuffer Decompress(byte[] buffer, int length) {
            if (length == 0) return new PacketBuffer(buffer);

            var inflater = new Inflater();

            inflater.SetInput(buffer);
            byte[] abyte1 = new byte[length];
            inflater.Inflate(abyte1);
            inflater.Reset();
            return new PacketBuffer(abyte1);
        }

        public static PacketBuffer Compress(PacketBuffer input, int compressionThreshold) {
            PacketBuffer output = new PacketBuffer();
            if (input.Size < compressionThreshold) {
                output.WriteVarInt(0);
                output.WriteRaw(input.ToArray());
                return output;
            }

            byte[] buffer = input.ToArray();
            output.WriteVarInt(buffer.Length);

            var deflater = new Deflater();
            deflater.SetInput(buffer);
            deflater.Finish();

            byte[] deflateBuf = new byte[8192];
            while (!deflater.IsFinished) {
                int j = deflater.Deflate(deflateBuf);
                output.WriteRaw(deflateBuf, 0, j);
            }
            deflater.Reset();
            return output;
        }


        public IPacketPayload? BuildPacket(byte[] data, int uncompressedLength) {
            PacketBuffer packetBuffer;
            if (uncompressedLength > 0) packetBuffer = PacketFactory.Decompress(data, uncompressedLength);
            else packetBuffer = new PacketBuffer(data);

            try {
                IPacket packet = ClientboundPacketFactories[client.GameState].ReadPacket(packetBuffer);
                if (packetBuffer.ReadableBytes > 0)
                    Logger.Debug3($"PacketBuffer should be empty after reading ({packet.GetType().Name})"); //throw new Exception("PacketBuffer must be empty after reading");

                return packet switch {
                    MineSharp.Data.Protocol.Handshaking.Clientbound.Packet chPacket => (IPacketPayload)chPacket.Params.Value!,
                    MineSharp.Data.Protocol.Status.Clientbound.Packet csPacket => (IPacketPayload)csPacket.Params.Value!,
                    MineSharp.Data.Protocol.Login.Clientbound.Packet clPacket => (IPacketPayload)clPacket.Params.Value!,
                    MineSharp.Data.Protocol.Play.Clientbound.Packet cpPacket => (IPacketPayload)cpPacket.Params.Value!,
                    _ => throw new Exception()
                };
            } catch (Exception e) {
                Logger.Error($"Error reading packet!");
                Logger.Error(e.ToString());
                return null;
            }
        }

        public PacketBuffer WritePacket(IPacketPayload packet) {
            try {
                PacketBuffer packetBuffer = new PacketBuffer();

                ServerboundPacketFactories[client.GameState].WritePacket(packetBuffer, packet);

                if (client.CompressionThreshold > 0) {
                    packetBuffer = PacketFactory.Compress(packetBuffer, client.CompressionThreshold);
                }
                return packetBuffer;
            } catch (Exception ex) {
                Logger.Error($"Error while writing packet of type {packet.GetType().FullName}: " + ex.ToString());
                throw new Exception($"Error while writing packet of type {packet.GetType().FullName}", ex);
            }
        }
    }
}
