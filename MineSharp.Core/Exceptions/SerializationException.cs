namespace MineSharp.Core.Exceptions;

/// <summary>
///     Thrown when de-/serialization of anything fails.
///     This might be a packet, from the root stream or a common element such as Attribute, Item or NbtTag, or anything else.
/// </summary>
/// <param name="message"></param>
public class SerializationException(string message) : MineSharpException(message);
