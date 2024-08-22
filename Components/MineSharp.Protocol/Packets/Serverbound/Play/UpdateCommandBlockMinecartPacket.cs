using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Program Command Block Minecart packet
/// </summary>
/// <param name="EntityId">The entity ID</param>
/// <param name="Command">The command to be executed</param>
/// <param name="TrackOutput">Whether to track the output of the command</param>
public sealed record UpdateCommandBlockMinecartPacket(int EntityId, string Command, bool TrackOutput) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_UpdateCommandBlockMinecart;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteString(Command);
        buffer.WriteBool(TrackOutput);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var command = buffer.ReadString();
        var trackOutput = buffer.ReadBool();

        return new UpdateCommandBlockMinecartPacket(
            entityId,
            command,
            trackOutput);
    }
}
