using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Serverbound.Play.SeenAdvancementsPacket;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Seen Advancements packet
/// </summary>
/// <param name="Action">The action taken</param>
/// <param name="TabId">The identifier of the tab, only present if action is <see cref="SeenAdvancementsAction.OpenedTab"/></param>
public sealed record SeenAdvancementsPacket(SeenAdvancementsAction Action, Identifier? TabId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_AdvancementTab;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)Action);
        if (Action == SeenAdvancementsAction.OpenedTab)
        {
            buffer.WriteIdentifier(TabId!);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var action = (SeenAdvancementsAction)buffer.ReadVarInt();
        Identifier? tabId = null;
        if (action == SeenAdvancementsAction.OpenedTab)
        {
            tabId = buffer.ReadIdentifier();
        }

        return new SeenAdvancementsPacket(action, tabId);
    }

    /// <summary>
    ///     Enum representing the actions for the Seen Advancements packet
    /// </summary>
    [Flags]
    public enum SeenAdvancementsAction
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        None = 0,
        OpenedTab = 1 << 0,
        ClosedScreen = 1 << 1
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
