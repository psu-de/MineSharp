using MineSharp.Core.Common.Blocks.Property;

namespace MineSharp.Core.Common.Blocks;

public class BlockProperties
{
    private readonly BlockProperty[] _properties;
    
    public BlockProperties(BlockProperty[] properties)
    {
        this._properties = properties;
    }
    
    public T GetPropertyValue<T>(string name, int state)
    {
        for (int i = this._properties.Length - 1; i >= 0; i--)
        {
            var prop = this._properties[i];
            if (prop.Name == name)
            {
                return prop.GetValue<T>(state % prop.Size);
            }
            state /= prop.Size;
        }
        return default(T)!;
    }
}
