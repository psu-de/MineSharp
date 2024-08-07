using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     Declare commands packet
/// </summary>
public class DeclareCommandsPacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="buffer"></param>
    public DeclareCommandsPacket(PacketBuffer buffer)
    {
        RawBuffer = buffer;
    }

    /// <summary>
    ///     Raw buffer. The Command tree is not parsed here
    /// </summary>
    public PacketBuffer RawBuffer { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_DeclareCommands;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteBytes(RawBuffer.GetBuffer());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var clone = new PacketBuffer(buffer.ReadBytes((int)buffer.ReadableBytes), version.Version.Protocol);
        return new DeclareCommandsPacket(clone);
    }
}
#pragma warning restore CS1591
