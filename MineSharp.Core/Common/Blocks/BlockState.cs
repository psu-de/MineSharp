using MineSharp.Core.Common.Blocks.Property;

namespace MineSharp.Core.Common.Blocks;

/// <summary>
/// Represents a block state with multiple properties.
/// </summary>
/// <param name="properties"></param>
public class BlockState(params IBlockProperty[] properties)
{
    /// <summary>
    /// Returns the value of the property with the given <paramref name="name"/>
    /// </summary>
    /// <param name="name">The name of the property</param>
    /// <param name="state">The current block state</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetPropertyValue<T>(string name, int state)
    {
        for (int i = properties.Length - 1; i >= 0; i--)
        {
            var prop = properties[i];
            if (prop.Name == name)
            {
                return prop.GetValue<T>(state % prop.StateCount);
            }
            state /= prop.StateCount;
        }
        return default(T)!;
    }
}
