using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class CombatDeathPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_DeathCombatEvent;

    public int PlayerId { get; set; }
    public int? EntityId { get; set; }
    public string Message { get; set; }

    /// <summary>
    /// Constructor before 1.20
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="entityId"></param>
    /// <param name="message"></param>
    public CombatDeathPacket(int playerId, int entityId, string message)
    {
        this.PlayerId = playerId;
        this.EntityId = entityId;
        this.Message = message;
    }

    public CombatDeathPacket(int playerId, string message)
    {
        this.PlayerId = playerId;
        this.Message = message;
    }

    private CombatDeathPacket(int playerId, int? entityId, string message)
    {
        this.PlayerId = playerId;
        this.EntityId = entityId;
        this.Message = message;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.PlayerId);
        if (version.Version.Protocol < ProtocolVersion.V_1_20)
            buffer.WriteInt(EntityId!.Value);
        buffer.WriteString(this.Message);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var playerId = buffer.ReadVarInt();
        int? entityId = null;
        if (version.Version.Protocol < ProtocolVersion.V_1_20)
            entityId = buffer.ReadInt();
        var message = buffer.ReadString();
        return new CombatDeathPacket(playerId, entityId, message);
    }
}
