using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the world border size.
/// </summary>
/// <param name="Diameter">Length of a single side of the world border, in meters.</param>
public sealed record SetBorderSizePacket(double Diameter) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WorldBorderSize;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(Diameter);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var diameter = buffer.ReadDouble();
        return new SetBorderSizePacket(diameter);
    }
}
