using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Advances the client processing by the specified number of ticks. Has no effect unless client ticking is frozen.
/// </summary>
/// <param name="TickSteps">The number of tick steps to advance</param>
public sealed record StepTickPacket(int TickSteps) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_StepTick;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(TickSteps);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var tickSteps = buffer.ReadVarInt();
        return new StepTickPacket(tickSteps);
    }
}
