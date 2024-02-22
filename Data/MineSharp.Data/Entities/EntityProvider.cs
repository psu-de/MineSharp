using MineSharp.Core.Common.Entities;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Entities;

internal class EntityProvider : IDataProvider<EntityInfo[]>
{
    private static readonly EnumNameLookup<EntityType> EntityTypeLookup = new();
    private static readonly EnumNameLookup<EntityCategory> EntityCategoryLookup = new();
    private static readonly EnumNameLookup<MobType> MobTypeLookup = new();
    
    
    private JArray token;

    public EntityProvider(JToken token)
    {
        if (token.Type != JTokenType.Array)
        {
            throw new ArgumentException($"Expected token to be an array");
        }

        this.token = (JArray)token;
    }
    
    public EntityInfo[] GetData()
    {
        var data = new EntityInfo[this.token.Count];

        for (int i = 0; i < token.Count; i++)
        {
            data[i] = FromToken(this.token[i]);
        }

        return data;
    }


    private static EntityInfo FromToken(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var width = (float)token.SelectToken("width")!;
        var height = (float)token.SelectToken("height")!;
        var mobType = (string)token.SelectToken("type")!;
        var category = (string)token.SelectToken("category")!;

        return new EntityInfo(
            id,
            EntityTypeLookup.FromName(NameUtils.GetEntityName(name)),
            name,
            displayName,
            width,
            height,
            MobTypeLookup.FromName(NameUtils.GetEntityName(mobType)),
            EntityCategoryLookup.FromName(NameUtils.GetEntityCategory(category))
        );
    }
}