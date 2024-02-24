using MineSharp.Core.Common.Items;
using MineSharp.Core.Exceptions;

namespace MineSharp.Bot.Exceptions;

/// <summary>
/// Thrown when an item could not be found.
/// </summary>
/// <param name="type"></param>
public class ItemNotFoundException(ItemType type) : MineSharpException($"Could not find item of type {type}")
{ }
