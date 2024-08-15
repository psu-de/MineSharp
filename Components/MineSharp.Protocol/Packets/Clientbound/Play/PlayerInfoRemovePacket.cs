using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record PlayerInfoRemovePacket(Uuid[] Players) : IPacketStatic<PlayerInfoRemovePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_PlayerRemove;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarIntArray(Players, (buffer, uuid) => buffer.WriteUuid(uuid));
    }

    public static PlayerInfoRemovePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var players = buffer.ReadVarIntArray(buffer => buffer.ReadUuid());
        return new PlayerInfoRemovePacket(players);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591
