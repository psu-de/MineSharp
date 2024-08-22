﻿using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Combat death packet
/// </summary>
public sealed partial record CombatDeathPacket : IPacketStatic<CombatDeathPacket>
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
    public CombatDeathPacket(int playerId, int entityId, Chat message)
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
    public CombatDeathPacket(int playerId, Chat message)
    {
        PlayerId = playerId;
        Message = message;
    }

    private CombatDeathPacket(int playerId, int? entityId, Chat message)
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
    public Chat Message { get; init; }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(PlayerId);
        if (data.Version.Protocol < ProtocolVersion.V_1_20_0)
        {
            buffer.WriteInt(EntityId!.Value);
        }

        buffer.WriteChatComponent(Message);
    }

    /// <inheritdoc />
    public static CombatDeathPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var playerId = buffer.ReadVarInt();
        int? entityId = null;
        if (data.Version.Protocol < ProtocolVersion.V_1_20_0)
        {
            entityId = buffer.ReadInt();
        }

        var message = buffer.ReadChatComponent();
        return new CombatDeathPacket(playerId, entityId, message);
    }
}
