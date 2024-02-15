using MineSharp.Core.Common.Blocks;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Blocks;

internal class BlockData(IDataProvider<BlockInfo[]> provider)
    : TypeIdNameIndexedData<BlockType, BlockInfo>(provider), IBlockData;