namespace MineSharp.Core.Common.Blocks.Property;

/// <summary>
/// A string block property.
/// </summary>
public class EnumProperty : IBlockProperty
{
    private readonly string[] _acceptedValues;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public int StateCount { get; }

    /// <summary>
    /// Create a new EnumProperty
    /// </summary>
    /// <param name="name"></param>
    /// <param name="acceptedValues"></param>
    public EnumProperty(string name, string[] acceptedValues)
    {
        this._acceptedValues = acceptedValues;
        this.Name            = name;
        this.StateCount      = this._acceptedValues.Length;
    }

    /// <inheritdoc />
    public T GetValue<T>(int state)
    {
        if (typeof(T) != typeof(string))
        {
            throw new NotSupportedException("This is property can only be an enum.");
        }

        if (state >= this.StateCount)
        {
            throw new IndexOutOfRangeException($"State {state} is out of range for property with {this.StateCount} entries.");
        }

        return (T)(object)this._acceptedValues[state];
    }
}
