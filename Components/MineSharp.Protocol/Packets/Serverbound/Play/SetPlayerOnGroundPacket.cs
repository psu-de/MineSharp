using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     This packet is used to indicate whether the player is on ground (walking/swimming), or airborne (jumping/falling).
/// </summary>
/// <param name="OnGround">True if the client is on the ground, false otherwise.</param>
public sealed record SetPlayerOnGroundPacket(bool OnGround) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_Flying;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteBool(OnGround);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var onGround = buffer.ReadBool();

        return new SetPlayerOnGroundPacket(onGround);
    }
}
