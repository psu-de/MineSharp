using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.CommandSuggestionsResponsePacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Command Suggestions Response packet
/// </summary>
/// <param name="Id">Transaction ID</param>
/// <param name="Start">Start of the text to replace</param>
/// <param name="Length">Length of the text to replace</param>
/// <param name="Matches">Array of matches</param>
public sealed record CommandSuggestionsResponsePacket(int Id, int Start, int Length, Match[] Matches) : IPacket
{
	/// <inheritdoc />
	public PacketType Type => StaticType;
	/// <inheritdoc />
	public static PacketType StaticType => PacketType.CB_Play_TabComplete;

	/// <inheritdoc />
	public void Write(PacketBuffer buffer, MinecraftData version)
	{
		buffer.WriteVarInt(Id);
		buffer.WriteVarInt(Start);
		buffer.WriteVarInt(Length);
		buffer.WriteVarInt(Matches.Length);
		foreach (var match in Matches)
		{
			match.Write(buffer);
		}
	}

	/// <inheritdoc />
	public static IPacket Read(PacketBuffer buffer, MinecraftData version)
	{
		var id = buffer.ReadVarInt();
		var start = buffer.ReadVarInt();
		var length = buffer.ReadVarInt();
		var count = buffer.ReadVarInt();
		var matches = new Match[count];
		for (int i = 0; i < count; i++)
		{
			matches[i] = Match.Read(buffer);
		}

		return new CommandSuggestionsResponsePacket(id, start, length, matches);
	}

    /// <summary>
    ///     Represents a match in the command suggestions response
    /// </summary>
    /// <param name="Value">The match string</param>
    /// <param name="Tooltip">The tooltip text component or <c>null</c></param>
    public sealed record Match(string Value, Chat? Tooltip) : ISerializable<Match>
	{
		/// <inheritdoc />
		public void Write(PacketBuffer buffer)
		{
			buffer.WriteString(Value);
            var hasTooltip = Tooltip != null;
            buffer.WriteBool(hasTooltip);
			if (hasTooltip)
			{
				buffer.WriteChatComponent(Tooltip!);
			}
		}

		/// <inheritdoc />
		public static Match Read(PacketBuffer buffer)
		{
			var match = buffer.ReadString();
			var hasTooltip = buffer.ReadBool();
			Chat? tooltip = null;
			if (hasTooltip)
			{
				tooltip = buffer.ReadChatComponent();
			}

			return new Match(match, tooltip);
		}
	}
}
