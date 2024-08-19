using MineSharp.Core.Common;
using MineSharp.Core.Common.Particles;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Particles;

internal class ParticleData(IDataProvider<IReadOnlyDictionary<Identifier, int>> provider) 
    : NameAndProtocolNumberIndexedData<ParticleType>(provider), IParticleData;
