using MineSharp.Protocol.Packets;

namespace MineSharp.Protocol;

/// <summary>
/// Event delegates used by MineSharp.Protocol
/// </summary>
public static class Events
{
    /// <summary>
    /// Delegate with MinecraftClient and IPacket
    /// </summary>
    public delegate void ClientPacketEvent(MinecraftClient sender, IPacket packet);

    /// <summary>
    /// Delegate with MinecraftClient and a string
    /// </summary>
    public delegate void ClientStringEvent(MinecraftClient sender, string message);
}
