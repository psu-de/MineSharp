using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed partial record CloseWindowPacket(byte WindowId) : IPacketStatic<CloseWindowPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_CloseWindow;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteByte(WindowId);
    }

    public static CloseWindowPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new CloseWindowPacket(buffer.ReadByte());
    }
}
#pragma warning restore CS1591
