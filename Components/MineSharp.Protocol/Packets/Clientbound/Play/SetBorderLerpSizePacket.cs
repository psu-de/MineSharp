﻿using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the world border lerp size.
/// </summary>
/// <param name="OldDiameter">Current length of a single side of the world border, in meters.</param>
/// <param name="NewDiameter">Target length of a single side of the world border, in meters.</param>
/// <param name="Speed">Number of real-time milliseconds until New Diameter is reached.</param>
public sealed partial record SetBorderLerpSizePacket(double OldDiameter, double NewDiameter, long Speed) : IPacketStatic<SetBorderLerpSizePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WorldBorderLerpSize;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteDouble(OldDiameter);
        buffer.WriteDouble(NewDiameter);
        buffer.WriteVarLong(Speed);
    }

    /// <inheritdoc />
    public static SetBorderLerpSizePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var oldDiameter = buffer.ReadDouble();
        var newDiameter = buffer.ReadDouble();
        var speed = buffer.ReadVarLong();

        return new SetBorderLerpSizePacket(oldDiameter, newDiameter, speed);
    }
}
