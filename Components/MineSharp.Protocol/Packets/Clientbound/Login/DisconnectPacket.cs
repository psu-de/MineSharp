using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;
#pragma warning disable CS1591
/// <summary>
/// Disconnect packet for login
/// </summary>
public class DisconnectPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_Disconnect;

    /// <summary>
    /// The reason for being disconnected
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
        buffer.WriteString(this.Reason.Json);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        string reason = buffer.ReadString();
        return new DisconnectPacket(new Chat(reason, version));
    }
}
#pragma warning restore CS1591