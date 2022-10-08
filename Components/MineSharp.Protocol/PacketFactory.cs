using ICSharpCode.SharpZipLib.Zip.Compression;
using MineSharp.Core.Logging;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Protocol;
using MineSharp.Data.Protocol.Handshaking.Clientbound;
using MineSharp.Data.Protocol.Login.Clientbound;
using MineSharp.Data.Protocol.Play.Clientbound;
using MineSharp.Data.Protocol.Status.Clientbound;
using Packet = MineSharp.Data.Protocol.Handshaking.Clientbound.Packet;
namespace MineSharp.Protocol
{
    internal class PacketFactory
    {

        private static readonly Logger Logger = Logger.GetLogger();
        private readonly MinecraftClient client;
        private readonly Dictionary<GameState, IPacketFactory> ClientboundPacketFactories;
        private readonly Dictionary<GameState, IPacketFactory> ServerboundPacketFactories;

        public PacketFactory(MinecraftClient client)
        {
            this.client = client;

            this.ClientboundPacketFactories = new Dictionary<GameState, IPacketFactory>();
            this.ClientboundPacketFactories.Add(GameState.HANDSHAKING, new HandshakingPacketFactory());
            this.ClientboundPacketFactories.Add(GameState.STATUS, new StatusPacketFactory());
            this.ClientboundPacketFactories.Add(GameState.LOGIN, new LoginPacketFactory());
            this.ClientboundPacketFactories.Add(GameState.PLAY, new PlayPacketFactory());


            this.ServerboundPacketFactories = new Dictionary<GameState, IPacketFactory>();
            this.ServerboundPacketFactories.Add(GameState.HANDSHAKING, new Data.Protocol.Handshaking.Serverbound.HandshakingPacketFactory());
            this.ServerboundPacketFactories.Add(GameState.STATUS, new Data.Protocol.Status.Serverbound.StatusPacketFactory());
            this.ServerboundPacketFactories.Add(GameState.LOGIN, new Data.Protocol.Login.Serverbound.LoginPacketFactory());
            this.ServerboundPacketFactories.Add(GameState.PLAY, new Data.Protocol.Play.Serverbound.PlayPacketFactory());
        }

        public static PacketBuffer Decompress(byte[] buffer, int length)
        {
            if (length == 0) return new PacketBuffer(buffer);

            var inflater = new Inflater();

            inflater.SetInput(buffer);
            var abyte1 = new byte[length];
            inflater.Inflate(abyte1);
            inflater.Reset();
            return new PacketBuffer(abyte1);
        }

        public static PacketBuffer Compress(PacketBuffer input, int compressionThreshold)
        {
            var output = new PacketBuffer();
            if (input.Size < compressionThreshold)
            {
                output.WriteVarInt(0);
                output.WriteRaw(input.ToArray());
                return output;
            }

            var buffer = input.ToArray();
            output.WriteVarInt(buffer.Length);

            var deflater = new Deflater();
            deflater.SetInput(buffer);
            deflater.Finish();

            var deflateBuf = new byte[8192];
            while (!deflater.IsFinished)
            {
                var j = deflater.Deflate(deflateBuf);
                output.WriteRaw(deflateBuf, 0, j);
            }
            deflater.Reset();
            return output;
        }


        public IPacketPayload? BuildPacket(byte[] data, int uncompressedLength)
        {
            PacketBuffer packetBuffer;
            if (uncompressedLength > 0) packetBuffer = Decompress(data, uncompressedLength);
            else packetBuffer = new PacketBuffer(data);

            try
            {
                var packet = this.ClientboundPacketFactories[this.client.GameState].ReadPacket(packetBuffer);
                if (packetBuffer.ReadableBytes > 0)
                    Logger.Debug3($"PacketBuffer should be empty after reading ({packet.GetType().Name})"); //throw new Exception("PacketBuffer must be empty after reading");

                return packet switch {
                    Packet chPacket => (IPacketPayload)chPacket.Params.Value!,
                    Data.Protocol.Status.Clientbound.Packet csPacket => (IPacketPayload)csPacket.Params.Value!,
                    Data.Protocol.Login.Clientbound.Packet clPacket => (IPacketPayload)clPacket.Params.Value!,
                    Data.Protocol.Play.Clientbound.Packet cpPacket => (IPacketPayload)cpPacket.Params.Value!,
                    _ => throw new Exception()
                };
            } catch (Exception e)
            {
                Logger.Error("Error reading packet!");
                Logger.Error(e.ToString());
                return null;
            }
        }

        public PacketBuffer WritePacket(IPacketPayload packet)
        {
            try
            {
                var packetBuffer = new PacketBuffer();

                this.ServerboundPacketFactories[this.client.GameState].WritePacket(packetBuffer, packet);

                if (this.client.CompressionThreshold > 0)
                {
                    packetBuffer = Compress(packetBuffer, this.client.CompressionThreshold);
                }
                return packetBuffer;
            } catch (Exception ex)
            {
                Logger.Error($"Error while writing packet of type {packet.GetType().FullName}: " + ex);
                throw new Exception($"Error while writing packet of type {packet.GetType().FullName}", ex);
            }
        }
    }
}
