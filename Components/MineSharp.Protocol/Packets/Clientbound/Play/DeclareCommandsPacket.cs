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
public sealed partial record DeclareCommandsPacket(PacketBuffer RawBuffer) : IPacketStatic<DeclareCommandsPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_DeclareCommands;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteBytes(RawBuffer.GetBuffer());
    }

    /// <inheritdoc />
    public static DeclareCommandsPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var clone = new PacketBuffer(buffer.ReadBytes((int)buffer.ReadableBytes), data.Version.Protocol);
        return new DeclareCommandsPacket(clone);
    }
}
