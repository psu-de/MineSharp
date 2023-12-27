namespace MineSharp.Core.Common.Entities.Attributes;

/// <summary>
/// A modifier for an attribute
/// </summary>
/// <param name="UUID">The uuid associated with this Modifier. This is a constant value from minecraft java.</param>
/// <param name="Amount"></param>
/// <param name="Operation"></param>
public record Modifier(UUID UUID, double Amount, ModifierOp Operation);
