namespace MineSharp.Core.Common.Blocks.Property;

public abstract class BlockProperty
{
    public string Name { get; }
    public int Size { get; }

    public BlockProperty(string name, int size)
    {
        this.Name = name;
        this.Size = size;
    }

    public abstract T GetValue<T>(int state);
}
