using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class CloseWindowPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_CloseWindow;

    public byte WindowId { get; set; }

    public CloseWindowPacket(byte windowId)
    {
        this.WindowId = windowId;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(this.WindowId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new CloseWindowPacket(
            buffer.ReadByte());
    }
}
#pragma warning restore CS1591
