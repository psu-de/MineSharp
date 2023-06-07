namespace MineSharp.Core.Common.Blocks.Property;

public class EnumProperty<TEnum> : StringProperty where TEnum : struct, Enum
{
    public EnumProperty(string name) : base(name, Enum.GetNames<TEnum>())
    { }

    public override T GetValue<T>(int state)
    {
        if (typeof(T) != typeof(TEnum))
        {
            throw new NotSupportedException($"This property must be of enum type {typeof(TEnum).Name}.");
        }
        
        if (state >= this.Size)
        {
            throw new IndexOutOfRangeException($"State {state} is out of range for property with {this.Size} entries.");
        }

        return (T)(object)Enum.GetValues<TEnum>()[state];
    }
}
