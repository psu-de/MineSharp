using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Serverbound.Play.ClientCommandPacket;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Represents a client command packet.
/// </summary>
/// <param name="ActionId">The action ID of the client command.</param>
public sealed record ClientCommandPacket(ClientCommandAction ActionId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ClientCommand;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)ActionId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var actionId = (ClientCommandAction)buffer.ReadVarInt();

        return new ClientCommandPacket(actionId);
    }

    /// <summary>
    ///    Represents the action ID of the client command.
    /// </summary>
    public enum ClientCommandAction
    {
        /// <summary>
        /// Sent when the client is ready to complete login and when the client is ready to respawn after death.
        /// </summary>
        PerformRespawn = 0,
        /// <summary>
        /// Sent when the client opens the Statistics menu.
        /// </summary>
        RequestStats = 1,
    }
}
