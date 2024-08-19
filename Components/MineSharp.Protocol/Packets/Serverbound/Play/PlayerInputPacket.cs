using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Serverbound.Play.PlayerInputPacket;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Player input packet sent by the client to the server.
/// </summary>
/// <param name="Sideways">Positive to the left of the player.</param>
/// <param name="Forward">Positive forward.</param>
/// <param name="Flags">Bit mask of flags. See <see cref="PlayerInputFlags"/>.</param>
public sealed record PlayerInputPacket(float Sideways, float Forward, PlayerInputFlags Flags) : IPacketStatic<PlayerInputPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_SteerVehicle;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(Sideways);
        buffer.WriteFloat(Forward);
        buffer.WriteByte((byte)Flags);
    }

    /// <inheritdoc />
    public static PlayerInputPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var sideways = buffer.ReadFloat();
        var forward = buffer.ReadFloat();
        var flags = (PlayerInputFlags)buffer.ReadByte();

        return new(sideways, forward, flags);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }

    /// <summary>
    ///     Flags indicating player actions.
    /// </summary>
    [Flags]
    public enum PlayerInputFlags : byte
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        None = 0x0,
        Jump = 0x1,
        Unmount = 0x2
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
