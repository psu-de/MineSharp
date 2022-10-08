using fNbt;
using MineSharp.Core.Types;

namespace MineSharp.Data.Items
{
    public class ItemFactory
    {

        public static Item CreateItem(Type type, byte count, int? damage, NbtCompound? metadata)
        {

            if (!type.IsAssignableTo(typeof(Item)))
                throw new ArgumentException();

            var parameters = new object?[] {
                count,
                damage,
                metadata
            };

            return (Item)Activator.CreateInstance(type, parameters)!;
        }

        public static Item CreateItem(int id, byte count, int? damage, NbtCompound? metadata)
        {
            var type = ItemPalette.GetItemTypeById(id);
            return CreateItem(type, count, damage, metadata);
        }
    }
}
