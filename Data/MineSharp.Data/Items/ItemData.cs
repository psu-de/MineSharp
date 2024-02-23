using MineSharp.Core.Common.Items;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Items;

internal class ItemData(IDataProvider<ItemInfo[]> provider)
    : TypeIdNameIndexedData<ItemType, ItemInfo>(provider), IItemData;
