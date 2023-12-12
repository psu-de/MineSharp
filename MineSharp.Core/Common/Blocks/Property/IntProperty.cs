namespace MineSharp.Core.Common.Blocks.Property;

public class IntProperty : IBlockProperty
{
    public string Name { get; }
    public int StateCount { get; }

    public IntProperty(string name, int stateCount)
    {
        this.Name = name;
        this.StateCount = stateCount;
    }

    public T GetValue<T>(int state)
    {
        if (typeof(T) != typeof(int))
        {
            throw new NotSupportedException("This is property can only be an integer.");
        }

        return (T)(object)state;
    }
}
