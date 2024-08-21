using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the world border center.
/// </summary>
/// <param name="X">The X coordinate of the world border center.</param>
/// <param name="Z">The Z coordinate of the world border center.</param>
public sealed partial record SetBorderCenterPacket(double X, double Z) : IPacketStatic<SetBorderCenterPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WorldBorderCenter;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteDouble(X);
        buffer.WriteDouble(Z);
    }

    /// <inheritdoc />
    public static SetBorderCenterPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var x = buffer.ReadDouble();
        var z = buffer.ReadDouble();

        return new SetBorderCenterPacket(x, z);
    }
}
