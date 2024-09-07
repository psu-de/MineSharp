using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sets the entity that the player renders from. This is normally used when the player left-clicks an entity while in spectator mode.
/// </summary>
/// <param name="CameraId">ID of the entity to set the client's camera to.</param>
public sealed record SetCameraPacket(int CameraId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Camera;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(CameraId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var cameraId = buffer.ReadVarInt();
        return new SetCameraPacket(cameraId);
    }
}
