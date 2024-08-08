using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Login;

/// <summary>
///     Login acknowledged packet
/// </summary>
public sealed record LoginAcknowledgedPacket() : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Login_LoginAcknowledged;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    { }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new LoginAcknowledgedPacket();
    }
}

