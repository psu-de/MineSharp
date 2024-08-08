using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Spawn Experience Orb packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="X">The X coordinate</param>
/// <param name="Y">The Y coordinate</param>
/// <param name="Z">The Z coordinate</param>
/// <param name="Count">The amount of experience this orb will reward once collected</param>
public sealed record SpawnExperienceOrbPacket(int EntityId, double X, double Y, double Z, short Count) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SpawnEntityExperienceOrb;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteShort(Count);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var count = buffer.ReadShort();

        return new SpawnExperienceOrbPacket(entityId, x, y, z, count);
    }
}
