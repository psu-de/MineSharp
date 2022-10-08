using MineSharp.Core.Types;
namespace MineSharp.Data.Blocks
{
    public static class BlockFactory
    {
        public static Block CreateBlock(Type type, int state, Position pos)
        {

            if (!type.IsAssignableTo(typeof(Block)))
                throw new ArgumentException();

            var parameters = new object[] {
                state, pos
            };

            return (Block)Activator.CreateInstance(type, parameters)!;
        }

        public static Block CreateBlock(int id, int state, Position pos)
        {
            var type = BlockPalette.GetBlockTypeById(id);
            return CreateBlock(type, state, pos);
        }
    }
}
