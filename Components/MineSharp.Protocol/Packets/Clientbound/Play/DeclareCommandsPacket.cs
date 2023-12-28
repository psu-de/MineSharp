using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
/// Declare commands packet
/// </summary>
public class DeclareCommandsPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Play_DeclareCommands;

    /// <summary>
    /// Raw buffer. The Command tree is not parsed here
    /// </summary>
    public PacketBuffer RawBuffer { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="buffer"></param>
    public DeclareCommandsPacket(PacketBuffer buffer)
    {
        this.RawBuffer = buffer;
    }
    
    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteBytes(this.RawBuffer.GetBuffer());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var clone = new PacketBuffer(buffer.ReadBytes((int)buffer.ReadableBytes));
        return new DeclareCommandsPacket(clone);
    }
}
#pragma warning restore CS1591