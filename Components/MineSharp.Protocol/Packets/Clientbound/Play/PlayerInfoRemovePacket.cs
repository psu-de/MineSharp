using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class PlayerInfoRemovePacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_PlayerRemove;
    
    public UUID[] Players { get; set; }

    public PlayerInfoRemovePacket(UUID[] players)
    {
        this.Players = players;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarIntArray(this.Players, (buffer, uuid) => buffer.WriteUuid(uuid));
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var players = buffer.ReadVarIntArray(buffer => buffer.ReadUuid());
        return new PlayerInfoRemovePacket(players);
    }
}
#pragma warning restore CS1591