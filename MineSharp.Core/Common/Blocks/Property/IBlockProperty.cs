namespace MineSharp.Core.Common.Blocks.Property;

/// <summary>
///     Interface for implementing block properties.
/// </summary>
public interface IBlockProperty
{
    /// <summary>
    ///     The name of the property.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     The number of different states this property has.
    /// </summary>
    public int StateCount { get; }

    /// <summary>
    ///     Return the value of this property for the given state.
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetValue<T>(int state);
}
