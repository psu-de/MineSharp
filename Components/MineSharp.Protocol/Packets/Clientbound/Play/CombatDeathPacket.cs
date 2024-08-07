﻿using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     Combat death packet
/// </summary>
public sealed record CombatDeathPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_DeathCombatEvent;

    // Here is no non-argument constructor allowed
    // Do not use
#pragma warning disable CS8618
    private CombatDeathPacket()
#pragma warning restore CS8618
    {
    }

    /// <summary>
    ///     Constructor before 1.20
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="entityId"></param>
    /// <param name="message"></param>
    public CombatDeathPacket(int playerId, int entityId, string message)
    {
        PlayerId = playerId;
        EntityId = entityId;
        Message = message;
    }

    /// <summary>
    ///     Constructor after 1.20
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="message"></param>
    public CombatDeathPacket(int playerId, string message)
    {
        PlayerId = playerId;
        Message = message;
    }

    private CombatDeathPacket(int playerId, int? entityId, string message)
    {
        PlayerId = playerId;
        EntityId = entityId;
        Message = message;
    }

    /// <summary>
    ///     Id of the player
    /// </summary>
    public int PlayerId { get; init; }

    /// <summary>
    ///     Id of the entity
    /// </summary>
    public int? EntityId { get; init; }

    /// <summary>
    ///     Death message
    /// </summary>
    public string Message { get; init; }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(PlayerId);
        if (version.Version.Protocol < ProtocolVersion.V_1_20)
        {
            buffer.WriteInt(EntityId!.Value);
        }

        buffer.WriteString(Message);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var playerId = buffer.ReadVarInt();
        int? entityId = null;
        if (version.Version.Protocol < ProtocolVersion.V_1_20)
        {
            entityId = buffer.ReadInt();
        }

        var message = buffer.ReadString();
        return new CombatDeathPacket(playerId, entityId, message);
    }
}
#pragma warning restore CS1591
