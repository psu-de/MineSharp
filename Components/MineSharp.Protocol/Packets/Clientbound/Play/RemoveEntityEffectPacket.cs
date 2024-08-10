using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Remove Entity Effect packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="EffectId">The effect ID</param>
public sealed record RemoveEntityEffectPacket(int EntityId, int EffectId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_RemoveEntityEffect;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteVarInt(EffectId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var effectId = buffer.ReadVarInt();

        return new RemoveEntityEffectPacket(entityId, effectId);
    }
}
