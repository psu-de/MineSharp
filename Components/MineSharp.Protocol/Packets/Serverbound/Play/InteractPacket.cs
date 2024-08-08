using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Serverbound.Play.InteractPacket;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record InteractPacket(
    int EntityId,
    InteractionType Interaction,
    float? TargetX,
    float? TargetY,
    float? TargetZ,
    PlayerHand? Hand,
    bool Sneaking
) : IPacket
{
    /// <summary>
    ///     Constructor for all interaction types except <see cref="InteractionType.InteractAt"/>.
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="interaction"></param>
    /// <param name="sneaking"></param>
    public InteractPacket(int entityId, InteractionType interaction, bool sneaking)
        : this(entityId, interaction, null, null, null, null, sneaking)
    {
    }

    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_UseEntity;

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
                interaction,
                buffer.ReadFloat(),
                buffer.ReadFloat(),
                buffer.ReadFloat(),
                (PlayerHand)buffer.ReadVarInt(),
                buffer.ReadBool()
            );
        }

        return new InteractPacket(
            entityId,
            interaction,
            null,
            null,
            null,
            null,
            buffer.ReadBool()
        );
    }

    public enum InteractionType
    {
        Interact = 0,
        Attack = 1,
        InteractAt = 2
    }
}
#pragma warning restore CS1591
