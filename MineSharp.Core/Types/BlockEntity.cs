using MineSharp.Core.Types.Enums;

namespace MineSharp.Core.Types {
    public class BlockEntity {

        public Position Position { get; set; }
        public BlockEntityType Type { get; set; }
        public fNbt.NbtCompound Data { get; set; }


        public BlockEntity(Position pos, BlockEntityType type, fNbt.NbtCompound data) {
            this.Position = pos;
            this.Type = type;
            this.Data = data;
        }


        public override string ToString() {
            return $"BlockEntity(id={(int)Type}, name={Enum.GetName(typeof(BlockEntityType), Type)}, data={Data})";
        }
    }
}
