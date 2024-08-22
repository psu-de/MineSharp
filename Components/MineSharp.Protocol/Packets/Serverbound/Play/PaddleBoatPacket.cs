using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Used to visually update whether boat paddles are turning.
/// </summary>
/// <param name="LeftPaddleTurning">Indicates if the left paddle is turning</param>
/// <param name="RightPaddleTurning">Indicates if the right paddle is turning</param>
public sealed record PaddleBoatPacket(bool LeftPaddleTurning, bool RightPaddleTurning) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_SteerBoat;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteBool(LeftPaddleTurning);
        buffer.WriteBool(RightPaddleTurning);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var leftPaddleTurning = buffer.ReadBool();
        var rightPaddleTurning = buffer.ReadBool();

        return new PaddleBoatPacket(leftPaddleTurning, rightPaddleTurning);
    }
}
