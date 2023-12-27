namespace MineSharp.Core.Common.Blocks.Property;

/// <summary>
/// An integer block state property.
/// </summary>
/// <param name="name"></param>
/// <param name="stateCount"></param>
public class IntProperty(string name, int stateCount) : IBlockProperty
{
    /// <inheritdoc />
    public string Name { get; } = name;

    /// <inheritdoc />
    public int StateCount { get; } = stateCount;

     
    /// <inheritdoc />
    public T GetValue<T>(int state)
    {
        if (typeof(T) != typeof(int))
        {
            throw new NotSupportedException("This is property can only be an integer.");
        }

        return (T)(object)state;
    }
}
