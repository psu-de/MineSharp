namespace MineSharp.Core.Common.Blocks.Property;

public interface IBlockProperty
{
    public string Name { get; }
    public int StateCount { get; }

    public T GetValue<T>(int state) where T : struct;
}
