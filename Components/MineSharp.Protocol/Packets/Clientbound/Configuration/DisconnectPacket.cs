using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Configuration Disconnect packet
///     See https://wiki.vg/Protocol#Disconnect_.28configuration.29
/// </summary>
public class DisconnectPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_Disconnect;
    
    /// <summary>
    ///     Reason for disconnect
    /// </summary>
    public required Chat Reason { get; init; }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteChatComponent(Reason);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new DisconnectPacket { Reason = buffer.ReadChatComponent() };
    }
}
