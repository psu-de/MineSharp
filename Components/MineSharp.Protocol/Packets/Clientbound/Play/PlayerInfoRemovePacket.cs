using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record PlayerInfoRemovePacket(Uuid[] Players) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_PlayerRemove;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarIntArray(Players, (buffer, uuid) => buffer.WriteUuid(uuid));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var players = buffer.ReadVarIntArray(buffer => buffer.ReadUuid());
        return new PlayerInfoRemovePacket(players);
    }
}
#pragma warning restore CS1591
