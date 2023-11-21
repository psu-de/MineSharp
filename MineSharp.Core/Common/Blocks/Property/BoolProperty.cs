namespace MineSharp.Core.Common.Blocks.Property;

public class BoolProperty : IBlockProperty
{
    public string Name { get; }
    public int StateCount => 2;

    public BoolProperty(string name)
    {
        this.Name = name;
    }

    public T GetValue<T>(int state) where T : struct
    {
        if (typeof(T) != typeof(bool))
        {
            throw new NotSupportedException("This is property can only be a boolean.");
        }

        return (T)(object)(0 != state);
    }
}
