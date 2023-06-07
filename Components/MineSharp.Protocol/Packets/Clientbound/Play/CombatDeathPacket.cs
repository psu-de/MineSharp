using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class CombatDeathPacket : IPacket
{
    public static int Id => 0x38;

    public int PlayerId { get; set; }
    public int EntityId { get; set; }
    public string Message { get; set; }

    public CombatDeathPacket(int playerId, int entityId, string message)
    {
        this.PlayerId = playerId;
        this.EntityId = entityId;
        this.Message = message;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.PlayerId);
        buffer.WriteInt(EntityId);
        buffer.WriteString(this.Message);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var playerId = buffer.ReadVarInt();
        var entityId = buffer.ReadInt();
        var message = buffer.ReadString();
        return new CombatDeathPacket(playerId, entityId, message);
    }
}
