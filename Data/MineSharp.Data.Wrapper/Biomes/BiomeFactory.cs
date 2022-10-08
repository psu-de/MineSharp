using MineSharp.Core.Types;

namespace MineSharp.Data.Biomes
{
    public static class BiomeFactory
    {

        public static Biome CreateBiome(Type type)
        {

            if (!type.IsAssignableTo(typeof(Biome)))
                throw new ArgumentException();

            return (Biome)Activator.CreateInstance(type)!;
        }

        public static Biome CreateBiome(int id)
        {
            var type = BiomePalette.GetBiomeTypeById(id);
            return CreateBiome(type);
        }
    }
}
