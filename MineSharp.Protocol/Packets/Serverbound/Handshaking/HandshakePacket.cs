using MineSharp.Protocol.Packets.Serverbound.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Serverbound.Handshaking {
    public class HandshakePacket : Packet {

        public int? ProtocolVersion { get; private set; }
        public string? Hostname { get; private set; }
        public ushort Port { get; private set; }
        public GameState NextState { get; private set; }

        public HandshakePacket() { }

        public HandshakePacket(int protocolVersion, string hostname, ushort port, GameState nextState) {
            ProtocolVersion = protocolVersion;
            Hostname = hostname;
            Port = port;
            NextState = nextState;
        }

        public override void Read(PacketBuffer buffer) {
            this.ProtocolVersion = buffer.ReadVarInt();
            this.Hostname = buffer.ReadString();
            this.Port = buffer.ReadUShort();
            this.NextState = (GameState)buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(ProtocolVersion ?? 0);
            buffer.WriteString(Hostname ?? "");
            buffer.WriteUShort(Port);
            buffer.WriteVarInt((int)NextState);
        }

        public override async Task Sent(MinecraftClient client) {
            client.SetGameState(this.NextState);
            await base.Sent(client);
        }
    }
}
