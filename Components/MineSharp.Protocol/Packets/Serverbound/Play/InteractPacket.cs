using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class InteractPacket : IPacket
{
    public enum InteractionType
    {
        Interact = 0,
        Attack = 1,
        InteractAt = 2
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="interaction"></param>
    /// <param name="sneaking"></param>
    public InteractPacket(int entityId, InteractionType interaction, bool sneaking)
    {
        EntityId = entityId;
        Interaction = interaction;
        Sneaking = sneaking;
    }

    /// <summary>
    ///     Constructor for <see cref="InteractionType.InteractAt" />
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="targetX"></param>
    /// <param name="targetY"></param>
    /// <param name="targetZ"></param>
    /// <param name="hand"></param>
    /// <param name="sneaking"></param>
    public InteractPacket(int entityId, float targetX, float targetY, float targetZ, PlayerHand hand, bool sneaking)
    {
        EntityId = entityId;
        Interaction = InteractionType.InteractAt;
        TargetX = targetX;
        TargetY = targetY;
        TargetZ = targetZ;
        Hand = hand;
        Sneaking = sneaking;
    }

    public int EntityId { get; set; }
    public InteractionType Interaction { get; set; }
    public float? TargetX { get; set; }
    public float? TargetY { get; set; }
    public float? TargetZ { get; set; }
    public PlayerHand? Hand { get; set; }
    public bool Sneaking { get; set; }
    public PacketType Type => PacketType.SB_Play_UseEntity;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteVarInt((int)Interaction);
        if (InteractionType.InteractAt == Interaction)
        {
            buffer.WriteFloat(TargetX!.Value);
            buffer.WriteFloat(TargetY!.Value);
            buffer.WriteFloat(TargetZ!.Value);
            buffer.WriteVarInt((int)Hand!.Value);
        }

        buffer.WriteBool(Sneaking);
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
}
#pragma warning restore CS1591
