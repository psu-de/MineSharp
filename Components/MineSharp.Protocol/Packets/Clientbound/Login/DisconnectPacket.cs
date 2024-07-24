using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Disconnect packet for login
///     See https://wiki.vg/Protocol#Disconnect_.28login.29
/// </summary>
public class DisconnectPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_Disconnect;
    
    /// <summary>
    ///     The reason for being disconnected
    /// </summary>
    public required Chat Reason { get; init; }
    
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
        return new DisconnectPacket() { Reason = Chat.Parse(reason) };
    }
}
