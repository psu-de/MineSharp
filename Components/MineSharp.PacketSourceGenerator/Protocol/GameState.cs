using System.Collections.Generic;
using static MineSharp.PacketSourceGenerator.Protocol.GameState;

namespace MineSharp.PacketSourceGenerator.Protocol;

/// <summary>
///     Specifies the GameState
/// </summary>
public enum GameState
{
#pragma warning disable CS1591
	Handshaking = 0,
	Status = 1,
	Login = 2,
	Play = 3,
	Configuration = 4,
#pragma warning restore CS1591
}

public static class GameStateHelper
{
	public static readonly IReadOnlyDictionary<string, GameState> ByNameLookup = new Dictionary<string, GameState>()
	{
		{ nameof(Handshaking), Handshaking },
		{ "Handshake", Handshaking }, // Alias
		{ nameof(Status), Status },
		{ nameof(Login), Login },
		{ nameof(Play), Play },
		{ nameof(Configuration), Configuration }
	};
}
