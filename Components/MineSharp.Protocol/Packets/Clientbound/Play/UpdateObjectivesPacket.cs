using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;
using static MineSharp.Protocol.Packets.Clientbound.Play.UpdateObjectivesPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Update Objectives packet
/// </summary>
/// <param name="ObjectiveName">A unique name for the objective</param>
/// <param name="Mode">The mode of the objective update</param>
/// <param name="ObjectiveValue">The text to be displayed for the score</param>
/// <param name="ObjectiveTypeValue">The type of the objective</param>
/// <param name="NumberFormat">The number format for the score</param>
public sealed record UpdateObjectivesPacket(
    string ObjectiveName,
    ObjectiveMode Mode,
    Chat? ObjectiveValue,
    // this field can not be named "Type" or "ObjectiveType"
    ObjectiveType? ObjectiveTypeValue,
    IScoreboardNumberFormat? NumberFormat
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ScoreboardObjective;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(ObjectiveName);
        buffer.WriteByte((byte)Mode);

        if (Mode == ObjectiveMode.Create || Mode == ObjectiveMode.Update)
        {
            buffer.WriteChatComponent(ObjectiveValue ?? throw new InvalidOperationException($"{nameof(ObjectiveValue)} is required if Mode is Create or Update"));
            buffer.WriteVarInt((int)(ObjectiveTypeValue ?? throw new InvalidOperationException($"{nameof(ObjectiveTypeValue)} is required if Mode is Create or Update")));
            var hasNumberFormat = NumberFormat != null;
            buffer.WriteBool(hasNumberFormat);

            if (hasNumberFormat)
            {
                buffer.WriteVarInt((int)NumberFormat!.Type);
                NumberFormat!.Write(buffer);
            }
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var objectiveName = buffer.ReadString();
        var mode = (ObjectiveMode)buffer.ReadByte();
        Chat? objectiveValue = null;
        ObjectiveType? objectiveType = null;
        IScoreboardNumberFormat? numberFormat = null;

        if (mode == ObjectiveMode.Create || mode == ObjectiveMode.Update)
        {
            objectiveValue = buffer.ReadChatComponent();
            objectiveType = (ObjectiveType)buffer.ReadVarInt();
            var hasNumberFormat = buffer.ReadBool();

            if (hasNumberFormat)
            {
                var type = (ScoreboardNumberFormatType)buffer.ReadVarInt();
                numberFormat = ScoreboardNumberFormatRegistry.Read(buffer, type);
            }
        }

        return new UpdateObjectivesPacket(
            objectiveName,
            mode,
            objectiveValue,
            objectiveType,
            numberFormat
        );
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    ///     Objective mode enumeration
    /// </summary>
    public enum ObjectiveMode : byte
    {
        Create = 0,
        Remove = 1,
        Update = 2
    }

    /// <summary>
    ///     Objective type enumeration
    /// </summary>
    public enum ObjectiveType : int
    {
        Integer = 0,
        Hearts = 1
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
