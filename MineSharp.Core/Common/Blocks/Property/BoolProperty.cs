namespace MineSharp.Core.Common.Blocks.Property;

/// <summary>
/// A boolean block property
/// </summary>
/// <param name="name"></param>
public class BoolProperty(string name) : IBlockProperty
{
    /// <inheritdoc />
    public string Name { get; } = name;
    
    /// <inheritdoc />
    public int StateCount => 2;
    
    /// <inheritdoc />
    public T GetValue<T>(int state)
    {
        if (typeof(T) != typeof(bool))
        {
            throw new NotSupportedException("This is property can only be a boolean.");
        }

        return (T)(object)(0 != state);
    }
}
