namespace MineSharp.Core.Common.Entities.Attributes;

/// <summary>
/// Specifies the operation of a modifier
/// </summary>
public enum ModifierOp
{
    /// <summary>
    /// The value of the modifier is added to the attribute
    /// </summary>
    Add = 0,

    /// <summary>
    /// The base of the Attribute is multiplied with 1 + the value of the Modifier and added to the attribute
    /// </summary>
    MultiplyBase = 1,

    /// <summary>
    /// The attribute's value is multiplied with the modifiers value
    /// </summary>
    Multiply = 2
}
