using MineSharp.Core.Types;

namespace MineSharp.Data.Enchantments
{
    public static class EnchantmentFactory
    {

        public static Enchantment CreateEnchantment(Type type, int level)
        {

            if (!type.IsAssignableTo(typeof(Enchantment)))
                throw new ArgumentException();

            var parameters = new object[] {
                level
            };

            return (Enchantment)Activator.CreateInstance(type, parameters)!;
        }

        public static Enchantment CreateEnchantment(int id, int level)
        {
            var type = EnchantmentPalette.GetEnchantmentTypeById(id);
            return CreateEnchantment(type, level);
        }

    }
}
