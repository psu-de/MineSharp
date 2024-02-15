namespace MineSharp.Data.Framework;

public interface IMinecraftData
{
    public IBiomeData Biomes { get; }
    public IBlockCollisionShapeData BlockCollisionShapes { get; }
    public IBlockData Blocks { get; }
    public IEffectData Effects { get; }
    public IEntityData Entities { get; }
    public IItemData Items { get; }
    public ILanguageData Language { get; }
    public IMaterialData Materials { get; }
    public IProtocolData Protocol { get; }
    public IRecipeData Recipes { get; }
    public IWindowData Windows { get; }
    public MinecraftVersion Version { get; }
}