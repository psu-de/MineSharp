using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;

namespace MineSharp.Data.Materials;

internal record MaterialDataBlob(Dictionary<Material, Dictionary<ItemType, float>> MultiplierMap);
