using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Disconnect packet for login
///     See https://wiki.vg/Protocol#Disconnect_.28login.29
/// </summary>
/// <param name="Reason">The reason for being disconnected</param>
public sealed partial record DisconnectPacket(Chat Reason) : IPacketStatic<DisconnectPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Login_Disconnect;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        // Disconnect (login) packet is always sent as JSON text component according to wiki.vg
        buffer.WriteString(Reason.ToJson().ToString());
    }

    /// <inheritdoc />
    public static DisconnectPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var reason = buffer.ReadString();
        // Disconnect (login) packet is always sent as JSON text component according to wiki.vg
        var chat = Chat.Parse(reason);
        return new DisconnectPacket(chat);
    }
}
