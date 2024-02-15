using MineSharp.Core.Common.Entities;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Entities;

internal class EntityData(IDataProvider<EntityInfo[]> provider)
    : TypeIdNameIndexedData<EntityType, EntityInfo>(provider), IEntityData;