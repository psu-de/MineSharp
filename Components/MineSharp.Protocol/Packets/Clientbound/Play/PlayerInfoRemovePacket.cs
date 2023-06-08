using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class PlayerInfoRemovePacket : IPacket
{
    public static int Id => 0x39;
    
    public UUID[] Players { get; set; }

    public PlayerInfoRemovePacket(UUID[] players)
    {
        this.Players = players;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarIntArray(this.Players, (buffer, uuid) => buffer.WriteUuid(uuid));
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var players = buffer.ReadVarIntArray(buffer => buffer.ReadUuid());
        return new PlayerInfoRemovePacket(players);
    }
}
