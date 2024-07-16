using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;
#pragma warning disable CS1591
/// <summary>
/// Configuration Disconnect packet
/// </summary>
public class DisconnectPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_Disconnect;

    /// <summary>
    /// Reason for disconnect
    /// </summary>
    public Chat Reason { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="reason"></param>
    public DisconnectPacket(Chat reason)
    {
        this.Reason = reason;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Reason.ToJson().ToString());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new DisconnectPacket(buffer.ReadChatComponent());
    }
}
#pragma warning restore CS1591
