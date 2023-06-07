namespace MineSharp.Core.Common.Blocks.Property;

public class IntProperty : BlockProperty
{
    public IntProperty(string name, int size) : base(name, size) 
    { }

    public override T GetValue<T>(int state)
    {
        if (typeof(T) != typeof(int))
        {
            throw new NotSupportedException("This is property can only be an integer.");
        }

        return (T)(object)state;
    }
}
