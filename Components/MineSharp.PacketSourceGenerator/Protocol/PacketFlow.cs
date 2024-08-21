using System.Collections.Generic;
using static MineSharp.PacketSourceGenerator.Protocol.PacketFlow;

namespace MineSharp.PacketSourceGenerator.Protocol;

/// <summary>
///     Specifies the direction of a packet
/// </summary>
public enum PacketFlow
{
	/// <summary>
	///     Packets sent to the client by the server
	/// </summary>
	Clientbound,

	/// <summary>
	///     Packets sent to the server by the client
	/// </summary>
	Serverbound
}

public static class PacketFlowHelper
{
	public static readonly IReadOnlyDictionary<string, PacketFlow> ByNameLookup = new Dictionary<string, PacketFlow>()
	{
		{ nameof(Clientbound), Clientbound },
		{ nameof(Serverbound), Serverbound }
	};

	public static readonly IReadOnlyDictionary<string, PacketFlow> ByShortNameLookup = new Dictionary<string, PacketFlow>()
	{
		{ "CB", Clientbound },
		{ "SB", Serverbound }
	};
}
