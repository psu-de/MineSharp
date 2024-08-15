using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sent by the integrated singleplayer server when changing render distance. 
///     This packet is sent by the server when the client reappears in the overworld after leaving the end.
/// </summary>
/// <param name="ViewDistance">Render distance (2-32).</param>
public sealed record SetRenderDistancePacket(int ViewDistance) : IPacketStatic<SetRenderDistancePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_UpdateViewDistance;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(ViewDistance);
    }

    /// <inheritdoc />
    public static SetRenderDistancePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var viewDistance = buffer.ReadVarInt();
        return new SetRenderDistancePacket(viewDistance);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
