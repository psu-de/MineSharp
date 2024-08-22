﻿using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Advances the client processing by the specified number of ticks. Has no effect unless client ticking is frozen.
/// </summary>
/// <param name="TickSteps">The number of tick steps to advance</param>
public sealed partial record StepTickPacket(int TickSteps) : IPacketStatic<StepTickPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_StepTick;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(TickSteps);
    }

    /// <inheritdoc />
    public static StepTickPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var tickSteps = buffer.ReadVarInt();
        return new StepTickPacket(tickSteps);
    }
}
