using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Configuration Disconnect packet
///     See https://wiki.vg/Protocol#Disconnect_.28configuration.29
/// </summary>
/// <param name="Reason">Reason for disconnect</param>
public sealed partial record DisconnectPacket(Chat Reason) : IPacketStatic<DisconnectPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_Disconnect;

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
