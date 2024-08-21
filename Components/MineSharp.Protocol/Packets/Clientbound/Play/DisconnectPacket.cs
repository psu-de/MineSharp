using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Disconnect packet for play
/// </summary>
/// <param name="Reason">
///     The reason for being disconnected
/// </param>
public sealed partial record DisconnectPacket(Chat Reason) : IPacketStatic<DisconnectPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_KickDisconnect;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteChatComponent(Reason);
    }

    /// <inheritdoc />
    public static DisconnectPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new DisconnectPacket(buffer.ReadChatComponent());
    }
}

