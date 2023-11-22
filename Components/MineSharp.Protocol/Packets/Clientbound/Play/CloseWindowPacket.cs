using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class CloseWindowPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_CloseWindow;
    
    public int WindowId { get; set; }
    
    public CloseWindowPacket(int windowId)
    {
        this.WindowId = windowId;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.WindowId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new CloseWindowPacket(
            buffer.ReadVarInt());
    }
}
