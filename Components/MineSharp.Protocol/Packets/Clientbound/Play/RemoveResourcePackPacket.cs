using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to remove a resource pack.
/// </summary>
/// <param name="Uuid">The UUID of the resource pack to be removed.</param>
public sealed record RemoveResourcePackPacket(Uuid? Uuid) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_RemoveResourcePack;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        var hasUuid = Uuid != null;
        buffer.WriteBool(hasUuid);
        if (hasUuid)
        {
            buffer.WriteUuid(Uuid!.Value);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var hasUuid = buffer.ReadBool();
        Uuid? uuid = hasUuid ? buffer.ReadUuid() : null;

        return new RemoveResourcePackPacket(uuid);
    }
}
