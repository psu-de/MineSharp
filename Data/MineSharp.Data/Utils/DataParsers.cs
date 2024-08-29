using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Blocks.Property;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Enchantments;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Particles;
using MineSharp.Core.Registries;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Utils;

internal class DataParser
{
    private readonly EnumNameLookup<BiomeType> BiomeTypeLookup = new();
    private readonly EnumNameLookup<BiomeCategory> BiomeCategoryLookup  = new();
    private readonly EnumNameLookup<Dimension> DimensionLookup = new();
    private readonly EnumNameLookup<ItemType> ItemTypeLookup = new();
    private readonly EnumNameLookup<EnchantmentCategory> EnchantmentCategoryLookup = new();
    private readonly EnumNameLookup<BlockType> BlockTypeLookup = new();
    private readonly EnumNameLookup<Material> MaterialLookup  = new();
    private readonly EnumNameLookup<EffectType> EffectTypeLookup = new();
    private readonly EnumNameLookup<EnchantmentType> EnchantmentTypeLookup = new();
    private readonly EnumNameLookup<EntityType> EntityTypeLookup = new();
    private readonly EnumNameLookup<EntityCategory> EntityCategoryLookup = new();
    private readonly EnumNameLookup<MobType> MobTypeLookup = new();
    private readonly EnumNameLookup<ParticleType> ParticleTypeLookup = new();
    
    #region Biomes
    
    public BiomeInfo ParseBiome(JToken dataToken)
    {
        var id = (int)dataToken.SelectToken("id")!;
        var name = (string)dataToken.SelectToken("name")!;
        var displayName = (string)dataToken.SelectToken("displayToken")!;
        var category = (string)dataToken.SelectToken("category")!;
        var temperature = (float)dataToken.SelectToken("temperature")!;
        var precipitation = (string)dataToken.SelectToken("precipitation")! != "none";
        var dimension = (string)dataToken.SelectToken("dimension")!;
        var color = (int)dataToken.SelectToken("color")!;

        return new(
            id,
            BiomeTypeLookup.FromName(NameUtils.GetBiomeName(name)),
            Identifier.Parse(name),
            displayName,
            BiomeCategoryLookup.FromName(NameUtils.GetBiomeCategory(category)),
            temperature,
            precipitation,
            DimensionLookup.FromName(NameUtils.GetDimensionName(dimension)),
            color
        );
    }
    
    #endregion
    
    #region Items
    
    public ItemInfo ParseItem(JToken dataToken)
    {
        var id = (int)dataToken.SelectToken("id")!;
        var name = (string)dataToken.SelectToken("name")!;
        var displayName = (string)dataToken.SelectToken("displayToken")!;
        var stackSize = (int)dataToken.SelectToken("stackSize")!;
        var durability = (int?)dataToken.SelectToken("maxDurability")!;
        var enchantments = (JArray?)dataToken.SelectToken("enchantCategories");
        var repairWith = (JArray?)dataToken.SelectToken("repairWith")!;

        return new(
            id,
            ItemTypeLookup.FromName(NameUtils.GetItemName(name)),
            Identifier.Parse(name),
            displayName,
            stackSize,
            durability,
            GetEnchantments(enchantments),
            GetRepairItems(repairWith)
        );
    }

    private EnchantmentCategory[] GetEnchantments(JArray? enchantments)
    {
        if (enchantments == null)
        {
            return Array.Empty<EnchantmentCategory>();
        }

        return enchantments
              .ToObject<string[]>()!
              .Select(NameUtils.GetEnchantmentName)
              .Select(EnchantmentCategoryLookup.FromName)
              .ToArray();
    }

    private ItemType[] GetRepairItems(JArray? repairWith)
    {
        if (repairWith == null)
        {
            return Array.Empty<ItemType>();
        }

        return repairWith
              .ToObject<string[]>()!
              .Select(NameUtils.GetItemName)
              .Select(ItemTypeLookup.FromName)
              .ToArray();
    }
    
    #endregion
    
    #region Blocks
    
    
    public BlockInfo ParseBlock(JToken dataToken, ItemRegistry items)
    {
        var id = (int)dataToken.SelectToken("id")!;
        var name = (string)dataToken.SelectToken("name")!;
        var displayName = (string)dataToken.SelectToken("displayToken")!;
        var hardness = (float?)dataToken.SelectToken("hardness") ?? float.MaxValue;
        var resistance = (float)dataToken.SelectToken("resistance")!;
        var minState = (int)dataToken.SelectToken("minStateId")!;
        var maxState = (int)dataToken.SelectToken("maxStateId")!;
        var unbreakable = !(bool)dataToken.SelectToken("diggable")!;
        var transparent = (bool)dataToken.SelectToken("transparent")!;
        var filterLight = (byte)dataToken.SelectToken("filterLight")!;
        var emitLight = (byte)dataToken.SelectToken("emitLight")!;
        var materials = (string)dataToken.SelectToken("material")!;
        var harvestTools = (JObject?)dataToken.SelectToken("harvestTools");
        var states = (JArray)dataToken.SelectToken("states")!;
        var defaultState = (int)dataToken.SelectToken("defaultState")!;

        return new(
            id,
            BlockTypeLookup.FromName(NameUtils.GetBlockName(name)),
            Identifier.Parse(name),
            displayName,
            hardness,
            resistance,
            minState,
            maxState,
            unbreakable,
            transparent,
            filterLight,
            emitLight,
            GetMaterials(materials),
            GetHarvestTools(harvestTools, items),
            defaultState,
            GetBlockState(states)
        );
    }

    private Material[] GetMaterials(string str)
    {
        return str.Split(";")
                  .Select(NameUtils.GetMaterial)
                  .Select(MaterialLookup.FromName)
                  .ToArray();
    }

    private ItemType[] GetHarvestTools(JObject? array, ItemRegistry items)
    {
        if (array == null)
        {
            return Array.Empty<ItemType>();
        }

        return array.Properties()
                    .Select(x => x.Name)
                    .Select(x => Convert.ToInt32(x))
                    .Select(items.ById)
                    .Select(x => x.Type)
                    .ToArray();
    }

    private BlockState GetBlockState(JArray states)
    {
        if (states.Count == 0)
        {
            return new(Array.Empty<IBlockProperty>());
        }

        var properties = states
                        .Select(x => GetBlockProperty((JObject)x))
                        .ToArray();

        return new(properties);
    }

    private IBlockProperty GetBlockProperty(JObject obj)
    {
        var name = (string)obj.SelectToken("name")!;
        var type = (string)obj.SelectToken("type")!;
        var numValues = (int)obj.SelectToken("num_values")!;

        return type switch
        {
            "bool" => new BoolProperty(name),
            "int" => new IntProperty(name, numValues),
            "enum" => new EnumProperty(name, obj.SelectToken("values")!.ToObject<string[]>()!),
            _ => throw new NotSupportedException($"Property of type '{type}' is not supported.")
        };
    }
    
    #endregion
    
    #region Effects
    
    public EffectInfo ParseEffect(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var isGood = (string)token.SelectToken("type")! == "good";

        return new(
            id,
            EffectTypeLookup.FromName(NameUtils.GetEffectName(name)),
            Identifier.Parse(name),
            displayName,
            isGood);
    }
    
    #endregion
    
    #region Enchantments
    
    public EnchantmentInfo ParseEnchantment(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName  = (string)token.SelectToken("displayName")!;
        var maxLevel = (int)token.SelectToken("maxLevel")!;
        var minCost = (JObject)token.SelectToken("minCost")!;
        var maxCost = (JObject)token.SelectToken("maxCost")!;
        var treasureOnly = (bool)token.SelectToken("treasureOnly")!;
        var curse = (bool)token.SelectToken("curse")!;
        var exclude = (JArray)token.SelectToken("exclude")!;
        var category = (string)token.SelectToken("category")!;
        var weight = (int)token.SelectToken("weight")!;
        var tradeable = (bool)token.SelectToken("tradeable")!;
        var discoverable = (bool)token.SelectToken("discoverable")!;

        return new(
            id,
            EnchantmentTypeLookup.FromName(NameUtils.GetEnchantmentName(name)),
            Identifier.Parse(name),
            displayName,
            maxLevel,
            GetEnchantmentCost(minCost),
            GetEnchantmentCost(maxCost),
            treasureOnly,
            curse,
            GetExclusions(exclude),
            EnchantmentCategoryLookup.FromName(NameUtils.GetEnchantmentCategory(category)),
            weight,
            tradeable,
            discoverable);
    }

    private EnchantCost GetEnchantmentCost(JObject obj)
    {
        var a = (int)obj.SelectToken("a")!;
        var b = (int)obj.SelectToken("b")!;

        return new(a, b);
    }

    private EnchantmentType[] GetExclusions(JArray array)
    {
        if (array.Count == 0)
        {
            return Array.Empty<EnchantmentType>();
        }

        return array
              .ToObject<string[]>()!
              .Select(NameUtils.GetEnchantmentName)
              .Select(EnchantmentTypeLookup.FromName)
              .ToArray();
    }
    
    #endregion
    
    #region Entities
    
    public EntityInfo ParseEntity(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var width = (float)token.SelectToken("width")!;
        var height = (float)token.SelectToken("height")!;
        var mobType = (string)token.SelectToken("type")!;
        var category = (string)token.SelectToken("category")!;

        return new(
            id,
            EntityTypeLookup.FromName(NameUtils.GetEntityName(name)),
            Identifier.Parse(name),
            displayName,
            width,
            height,
            MobTypeLookup.FromName(NameUtils.GetEntityName(mobType)),
            EntityCategoryLookup.FromName(NameUtils.GetEntityCategory(category))
        );
    }
    
    #endregion
    
    #region Particles

    public RegistryResource<ParticleType> ParseParticle(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        return new (Identifier.Parse(name), ParticleTypeLookup.FromName(NameUtils.GetParticleName(name)), id);
    }
    
    #endregion
}
