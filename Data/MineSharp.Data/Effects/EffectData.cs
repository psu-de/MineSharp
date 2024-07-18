using MineSharp.Core.Common.Effects;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Effects;

internal class EffectData(IDataProvider<EffectInfo[]> provider)
    : TypeIdNameIndexedData<EffectType, EffectInfo>(provider), IEffectData;
