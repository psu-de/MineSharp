using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Enchantments;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Particles;
using MineSharp.Data.Protocol;

namespace MineSharp.Data.Tests;

public class Tests
{
    private static readonly string[] Versions =
    [
        "1.18",
        "1.18.1",
        "1.18.2",
        "1.19",
        "1.19.1",
        "1.19.2",
        "1.19.3",
        "1.19.4",
        "1.20",
        "1.20.1",
        "1.20.2",
        "1.20.3",
        "1.20.4"
    ];

    [Test]
    public void TestLoadData()
    {
        foreach (var version in Versions)
        {
            var data = MinecraftData.FromVersion(version).Result;

            // load all data
            data.Biomes.ByType(BiomeType.Beach);
            data.BlockCollisionShapes.GetShapes(BlockType.Stone, 0);
            data.Blocks.ByType(BlockType.Air);
            data.Effects.ByType(EffectType.Absorption);
            data.Enchantments.ByType(EnchantmentType.Channeling);
            data.Entities.ByType(EntityType.Allay);
            data.Items.ByType(ItemType.Bamboo);
            data.Language.GetTranslation("menu.quit");
            data.Materials.GetMultiplier(Material.Shovel, ItemType.StoneSword);
            data.Protocol.GetPacketId(PacketType.CB_Play_Login);
            data.Recipes.ByItem(ItemType.DiamondShovel);
            data.Particles.GetProtocolId(ParticleType.Composter);
            data.Windows.ById(0);
        }
    }
}
