namespace MineSharp.Core.Common.Blocks.Property;

/// <summary>
///     A string block property.
/// </summary>
public class EnumProperty : IBlockProperty
{
    private readonly string[] acceptedValues;

    /// <summary>
    ///     Create a new EnumProperty
    /// </summary>
    /// <param name="name"></param>
    /// <param name="acceptedValues"></param>
    public EnumProperty(string name, string[] acceptedValues)
    {
        this.acceptedValues = acceptedValues;
        Name = name;
        StateCount = this.acceptedValues.Length;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public int StateCount { get; }

    /// <inheritdoc />
    public T GetValue<T>(int state)
    {
        if (typeof(T) != typeof(string))
        {
            throw new NotSupportedException("This is property can only be an enum.");
        }

        if (state >= StateCount)
        {
            throw new IndexOutOfRangeException(
                $"State {state} is out of range for property with {StateCount} entries.");
        }

        return (T)(object)acceptedValues[state];
    }
}
