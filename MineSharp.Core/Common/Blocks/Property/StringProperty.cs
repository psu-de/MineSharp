namespace MineSharp.Core.Common.Blocks.Property;

public class StringProperty : BlockProperty
{
    private readonly string[] _acceptedValues;

    public StringProperty(string name, string[] acceptedValues) : base(name, acceptedValues.Length)
    {
        this._acceptedValues = acceptedValues;
    }

    public override T GetValue<T>(int state)
    {
        if (typeof(T) != typeof(string))
        {
            throw new NotSupportedException("This is property can only be a string.");
        }
        
        if (state >= this.Size)
        {
            throw new IndexOutOfRangeException($"State {state} is out of range for property with {this.Size} entries.");
        }

        return (T)(object)this._acceptedValues[state];
    }
}
