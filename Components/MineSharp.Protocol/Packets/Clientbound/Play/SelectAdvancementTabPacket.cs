using System.Collections.Frozen;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sent by the server to indicate that the client should switch advancement tab.
///     Sent either when the client switches tab in the GUI or when an advancement in another tab is made.
/// </summary>
/// <param name="Identifier">The identifier of the advancement tab. If no or an invalid identifier is sent, the client will switch to the first tab in the GUI.</param>
public sealed record SelectAdvancementTabPacket(Identifier? Identifier) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SelectAdvancementTab;

    /// <summary>
    ///     Set of all possible identifiers.
    /// </summary>
    public static readonly FrozenSet<Identifier> PossibleIdentifiers = new HashSet<Identifier>()
    {
        Identifier.Parse("minecraft:story/root"),
        Identifier.Parse("minecraft:nether/root"),
        Identifier.Parse("minecraft:end/root"),
        Identifier.Parse("minecraft:adventure/root"),
        Identifier.Parse("minecraft:husbandry/root")
    }.ToFrozenSet();

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        var hasId = Identifier is not null;
        buffer.WriteBool(hasId);
        if (hasId)
        {
            buffer.WriteIdentifier(Identifier!);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var hasId = buffer.ReadBool();
        Identifier? identifier = hasId ? buffer.ReadIdentifier() : null;

        return new SelectAdvancementTabPacket(identifier);
    }
}
