using ICSharpCode.SharpZipLib.Zip.Compression;
using MineSharp.Core.Logging;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Protocol;
using MineSharp.Data.Protocol.Handshaking.Clientbound;
using MineSharp.Data.Protocol.Login.Clientbound;
using MineSharp.Data.Protocol.Play.Clientbound;
using MineSharp.Data.Protocol.Status.Clientbound;
using System.Diagnostics;
using Packet = MineSharp.Data.Protocol.Handshaking.Clientbound.Packet;

namespace MineSharp.Protocol
{
    
    public static class PacketFactory
    {
        private static readonly Logger Logger = Logger.GetLogger();

        public static IPacketPayload? BuildPacket(PacketBuffer packetBuffer, GameState gameState)
        {
            try
            {
                var packet = gameState switch {
                    GameState.HANDSHAKING => HandshakingPacketFactory.ReadPacket(packetBuffer),
                    GameState.LOGIN => LoginPacketFactory.ReadPacket(packetBuffer),
                    GameState.PLAY => PlayPacketFactory.ReadPacket(packetBuffer),
                    GameState.STATUS => StatusPacketFactory.ReadPacket(packetBuffer),
                    _ => throw new UnreachableException()
                };
                
                if (packetBuffer.ReadableBytes > 0)
                    Logger.Debug3($"PacketBuffer should be empty after reading ({packet.Name})"); //throw new Exception("PacketBuffer must be empty after reading");

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

        public static PacketBuffer WritePacket(IPacketPayload packet, GameState gameState)
        {
            try
            {
                var packetBuffer = new PacketBuffer();

                switch (gameState)
                {
                    case GameState.HANDSHAKING: Data.Protocol.Handshaking.Serverbound.HandshakingPacketFactory.WritePacket(packetBuffer, packet); break;
                    case GameState.LOGIN: Data.Protocol.Login.Serverbound.LoginPacketFactory.WritePacket(packetBuffer, packet); break;
                    case GameState.PLAY: Data.Protocol.Play.Serverbound.PlayPacketFactory.WritePacket(packetBuffer, packet); break;
                    case GameState.STATUS: Data.Protocol.Status.Serverbound.StatusPacketFactory.WritePacket(packetBuffer, packet); break;
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
