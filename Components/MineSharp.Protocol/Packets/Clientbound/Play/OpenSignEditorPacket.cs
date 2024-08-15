using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sent when the client has placed a sign and is allowed to send Update Sign.
///     There must already be a sign at the given location.
/// </summary>
/// <param name="Location">The position of the sign</param>
/// <param name="IsFrontText">Whether the opened editor is for the front or on the back of the sign</param>
public sealed record OpenSignEditorPacket(Position Location, bool IsFrontText) : IPacketStatic<OpenSignEditorPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_OpenSignEntity;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WritePosition(Location);
        buffer.WriteBool(IsFrontText);
    }

    /// <inheritdoc />
    public static OpenSignEditorPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var location = buffer.ReadPosition();
        var isFrontText = buffer.ReadBool();

        return new OpenSignEditorPacket(location, isFrontText);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
