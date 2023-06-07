namespace MineSharp.Core.Common.Blocks.Property;

public class BoolProperty : BlockProperty
{
    public BoolProperty(string name) : base(name, 2) 
    { }

    public override T GetValue<T>(int state)
    {
        if (typeof(T) != typeof(bool))
        {
            throw new NotSupportedException("This is property can only be a boolean.");
        }

        return (T)(object)(0 != state);
    }
}
