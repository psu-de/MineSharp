using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;
#pragma warning disable CS1591
/// <summary>
///     Disconnect packet for login
/// </summary>
public class DisconnectPacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="reason"></param>
    public DisconnectPacket(Chat reason)
    {
        Reason = reason;
    }

    /// <summary>
    ///     The reason for being disconnected
    /// </summary>
    public Chat Reason { get; set; }

    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_Disconnect;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        // Disconnect (login) packet is always sent as JSON text component according to wiki.vg
        buffer.WriteString(Reason.ToJson().ToString());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var reason = buffer.ReadString();
        return new DisconnectPacket(Chat.Parse(reason));
    }
}
#pragma warning restore CS1591
