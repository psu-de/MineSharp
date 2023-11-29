using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class InteractPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_UseEntity;
    
    public int EntityId { get; set; }
    public InteractionType Interaction { get; set; }
    public float? TargetX { get; set; }
    public float? TargetY { get; set; }
    public float? TargetZ { get; set; }
    public PlayerHand? Hand { get; set; }
    public bool Sneaking { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="interaction"></param>
    /// <param name="sneaking"></param>
    public InteractPacket(int entityId, InteractionType interaction, bool sneaking)
    {
        this.EntityId = entityId;
        this.Interaction = interaction;
        this.Sneaking = sneaking;
    }

    /// <summary>
    /// Constructor for <see cref="InteractionType.InteractAt"/>
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="targetX"></param>
    /// <param name="targetY"></param>
    /// <param name="targetZ"></param>
    /// <param name="hand"></param>
    /// <param name="sneaking"></param>
    public InteractPacket(int entityId, float targetX, float targetY, float targetZ, PlayerHand hand, bool sneaking)
    {
        this.EntityId = entityId;
        this.Interaction = InteractionType.InteractAt;
        this.TargetX = targetX;
        this.TargetY = targetY;
        this.TargetZ = targetZ;
        this.Hand = hand;
        this.Sneaking = sneaking;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteVarInt((int)this.Interaction);
        if (InteractionType.InteractAt == this.Interaction)
        {
            buffer.WriteFloat(this.TargetX!.Value);
            buffer.WriteFloat(this.TargetY!.Value);
            buffer.WriteFloat(this.TargetZ!.Value);
            buffer.WriteVarInt((int)this.Hand!.Value);
        }
        buffer.WriteBool(this.Sneaking);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var interaction = (InteractionType)buffer.ReadVarInt();
        
        if (InteractionType.InteractAt == interaction)
        {
            return new InteractPacket(
                entityId,
                buffer.ReadFloat(),
                buffer.ReadFloat(),
                buffer.ReadFloat(),
                (PlayerHand)buffer.ReadVarInt(),
                buffer.ReadBool());
        }
        
        return new InteractPacket(
            entityId,
            interaction,
            buffer.ReadBool());
    }


    public enum InteractionType
    {
        Interact = 0,
        Attack = 1,
        InteractAt = 2
    }
}
