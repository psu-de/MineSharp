using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Teleports the player to the given entity. The player must be in spectator mode.
///     
///     The Notchian client only uses this to teleport to players, but it appears to accept any type of entity.
///     The entity does not need to be in the same dimension as the player; if necessary, the player will be respawned in the right world.
///     If the given entity cannot be found (or isn't loaded), this packet will be ignored.
///     It will also be ignored if the player attempts to teleport to themselves. 
/// </summary>
/// <param name="TargetPlayer">UUID of the player to teleport to (can also be an entity UUID).</param>
public sealed record TeleportToEntityPacket(Uuid TargetPlayer) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_Spectate;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteUuid(TargetPlayer);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var targetPlayer = buffer.ReadUuid();

        return new TeleportToEntityPacket(targetPlayer);
    }
}
