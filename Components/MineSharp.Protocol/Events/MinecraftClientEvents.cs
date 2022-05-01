using MineSharp.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Events {
    public class MinecraftClientEvents {

        public delegate void ClientEvent(MinecraftClient client);
        public event ClientEvent? Spawned;

        public delegate void ClientStringEvent(MinecraftClient client, string message);
        public event ClientStringEvent? Disconnected;

        public delegate void ClientPacketEvent(MinecraftClient client, Packet packet);
        public event ClientPacketEvent? PacketReceived;
        public event ClientPacketEvent? PacketSent;

        internal void InvokeClientSpawned(MinecraftClient client) => this.Spawned?.Invoke(client);
        internal void InvokeClientDisconnected(MinecraftClient client, string reason) => this.Disconnected?.Invoke(client, reason);
        internal void InvokePacketReceived(MinecraftClient client, Packet packet) => this.PacketReceived?.Invoke(client, packet);
        internal void InvokePacketSent(MinecraftClient client, Packet packet) => this.PacketSent?.Invoke(client, packet);
    }
}
