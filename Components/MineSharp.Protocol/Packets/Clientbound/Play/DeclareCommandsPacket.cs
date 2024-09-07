using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Declare commands packet
/// </summary>
/// <param name="RawBuffer">
///     Raw buffer. The Command tree is not parsed here
/// </param>
public sealed record DeclareCommandsPacket(PacketBuffer RawBuffer) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
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
