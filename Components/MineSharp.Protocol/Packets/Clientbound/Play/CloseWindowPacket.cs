using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Close window packet
///     <param name="WindowId">The id of the window to close</param>
/// </summary>
public sealed record CloseWindowPacket(byte WindowId) : IPacketStatic<CloseWindowPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_CloseWindow;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteByte(WindowId);
    }

    /// <inheritdoc />
    public static CloseWindowPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new CloseWindowPacket(buffer.ReadByte());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
