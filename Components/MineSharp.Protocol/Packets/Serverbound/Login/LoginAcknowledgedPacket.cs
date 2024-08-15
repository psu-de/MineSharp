using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Login;

/// <summary>
///     Login acknowledged packet
/// </summary>
public sealed record LoginAcknowledgedPacket() : IPacketStatic<LoginAcknowledgedPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Login_LoginAcknowledged;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    { }

    /// <inheritdoc />
    public static LoginAcknowledgedPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new LoginAcknowledgedPacket();
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}

