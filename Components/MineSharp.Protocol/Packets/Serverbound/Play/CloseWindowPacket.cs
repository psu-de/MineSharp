using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record CloseWindowPacket(byte WindowId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_CloseWindow;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(WindowId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new CloseWindowPacket(buffer.ReadByte());
    }
}
#pragma warning restore CS1591
