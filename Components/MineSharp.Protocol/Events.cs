using MineSharp.Protocol.Packets;

namespace MineSharp.Protocol;

public static class Events
{
    public delegate void ClientPacketEvent(MinecraftClient sender, IPacket packet);

    public delegate void ClientStringEvent(MinecraftClient sender, string message);
}
