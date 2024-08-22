using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     This packet is used to indicate whether the player is on ground (walking/swimming), or airborne (jumping/falling).
/// </summary>
/// <param name="OnGround">True if the client is on the ground, false otherwise.</param>
public sealed partial record SetPlayerOnGroundPacket(bool OnGround) : IPacketStatic<SetPlayerOnGroundPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_Flying;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteBool(OnGround);
    }

    /// <inheritdoc />
    public static SetPlayerOnGroundPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var onGround = buffer.ReadBool();

        return new(onGround);
    }
}
