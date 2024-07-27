using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     Close window packet
/// </summary>
public class CloseWindowPacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="windowId"></param>
    public CloseWindowPacket(byte windowId)
    {
        WindowId = windowId;
    }

    /// <summary>
    ///     The id of the window to close
    /// </summary>
    public byte WindowId { get; set; }

    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Play_CloseWindow;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(WindowId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new CloseWindowPacket(
            buffer.ReadByte());
    }
}
#pragma warning restore CS1591
