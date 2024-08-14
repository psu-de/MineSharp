using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Update Score packet
/// </summary>
/// <param name="EntityName">The entity whose score this is. For players, this is their username; for other entities, it is their UUID.</param>
/// <param name="ObjectiveName">The name of the objective the score belongs to</param>
/// <param name="Value">The score to be displayed next to the entry</param>
/// <param name="DisplayName">The custom display name</param>
/// <param name="NumberFormat">The number format for the score</param>
public sealed record UpdateScorePacket(
    string EntityName,
    string ObjectiveName,
    int Value,
    Chat? DisplayName,
    IScoreboardNumberFormat? NumberFormat
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ScoreboardScore;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(EntityName);
        buffer.WriteString(ObjectiveName);
        buffer.WriteVarInt(Value);
        var hasDisplayName = DisplayName != null;
        buffer.WriteBool(hasDisplayName);

        if (hasDisplayName)
        {
            buffer.WriteChatComponent(DisplayName!);
        }

        var hasNumberFormat = NumberFormat != null;
        buffer.WriteBool(hasNumberFormat);

        if (hasNumberFormat)
        {
            buffer.WriteVarInt((int)NumberFormat!.Type);
            NumberFormat!.Write(buffer);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityName = buffer.ReadString();
        var objectiveName = buffer.ReadString();
        var value = buffer.ReadVarInt();
        var hasDisplayName = buffer.ReadBool();
        Chat? displayName = null;
        if (hasDisplayName)
        {
            displayName = buffer.ReadChatComponent();
        }

        var hasNumberFormat = buffer.ReadBool();
        IScoreboardNumberFormat? numberFormat = null;
        if (hasNumberFormat)
        {
            var type = (ScoreboardNumberFormatType)buffer.ReadVarInt();
            numberFormat = ScoreboardNumberFormatRegistry.Read(buffer, type);
        }

        return new UpdateScorePacket(
            entityName,
            objectiveName,
            value,
            displayName,
            numberFormat
        );
    }
}
