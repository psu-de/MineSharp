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
public sealed record DisconnectPacket(Chat Reason) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_KickDisconnect;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteChatComponent(Reason);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new DisconnectPacket(buffer.ReadChatComponent());
    }
}

