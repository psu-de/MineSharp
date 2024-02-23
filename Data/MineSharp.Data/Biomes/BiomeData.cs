using MineSharp.Core.Common.Biomes;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Biomes;

internal class BiomeData(IDataProvider<BiomeInfo[]> provider)
    : TypeIdNameIndexedData<BiomeType, BiomeInfo>(provider), IBiomeData;
