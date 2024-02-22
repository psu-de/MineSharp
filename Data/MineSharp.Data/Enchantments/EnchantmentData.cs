using MineSharp.Core.Common.Enchantments;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Enchantments;

internal class EnchantmentData(IDataProvider<EnchantmentInfo[]> provider)
    : TypeIdNameIndexedData<EnchantmentType, EnchantmentInfo>(provider), IEnchantmentData;