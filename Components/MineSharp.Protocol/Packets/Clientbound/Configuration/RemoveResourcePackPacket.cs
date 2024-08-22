﻿using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Packet sent by the server to remove a resource pack (or all of them).
/// </summary>
/// <param name="Uuid">The UUID of the resource pack to be removed. Or all of the resource packs if this value is <c>null</c>.</param>
public sealed partial record RemoveResourcePackPacket(Uuid? Uuid) : IPacketStatic<RemoveResourcePackPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_RemoveResourcePack;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        var hasUuid = Uuid != null;
        buffer.WriteBool(hasUuid);
        if (hasUuid)
        {
            buffer.WriteUuid(Uuid!.Value);
        }
    }

    /// <inheritdoc />
    public static RemoveResourcePackPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var hasUuid = buffer.ReadBool();
        Uuid? uuid = hasUuid ? buffer.ReadUuid() : null;

        return new RemoveResourcePackPacket(uuid);
    }
}
