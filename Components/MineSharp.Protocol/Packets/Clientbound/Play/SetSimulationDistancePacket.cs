using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Set Simulation Distance packet
/// </summary>
/// <param name="SimulationDistance">The distance that the client will process specific things, such as entities.</param>
public sealed record SetSimulationDistancePacket(int SimulationDistance) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SimulationDistance;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(SimulationDistance);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var simulationDistance = buffer.ReadVarInt();
        return new SetSimulationDistancePacket(simulationDistance);
    }
}
