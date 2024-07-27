using MineSharp.Core.Exceptions;

namespace MineSharp.Bot.Exceptions;

/// <summary>
///     Thrown when trying to get a plugin that is not loaded by the bot
/// </summary>
/// <param name="message"></param>
public class PluginNotLoadedException(string message) : MineSharpException(message);
