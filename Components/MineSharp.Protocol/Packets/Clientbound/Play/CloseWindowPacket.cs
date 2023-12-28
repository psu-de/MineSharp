using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
/// Close window packet
/// </summary>
public class CloseWindowPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Play_CloseWindow;
    
    /// <summary>
    /// The id of the window to close
    /// </summary>
    public byte WindowId { get; set; }
    
    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="windowId"></param>
    public CloseWindowPacket(byte windowId)
    {
        this.WindowId = windowId;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(this.WindowId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new CloseWindowPacket(
            buffer.ReadByte());
    }
}
#pragma warning restore CS1591