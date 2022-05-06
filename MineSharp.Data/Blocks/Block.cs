using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Blocks {
    public class Block {

        public BlockInfo Info { get; set; }
        public Position Position { get; set; }
        public int State { get; set; }
        public int Metadata => State - Info.MinStateId;
        public BlockProperties Properties { get; set; }

        public Block(BlockInfo info, Position position, int state) {
            Info = info;
            Position = position;
            this.State = state;
            this.Properties = Info.DefaultProperties.Clone();
            this.Properties.Set(Metadata);
        }

        public bool IsSolid() {
            return !IsAir();
        }

        public bool IsAir() {
            return (Info.Id == BlockType.Air || Info.Id == BlockType.CaveAir || Info.Id == BlockType.VoidAir);
        }

        public override string ToString() {
            return $"Block: (id={Info.Id}, Name={Info.Name}) at {Position}";
        }


        public float[][] GetBlockShape() {
            var shapeIdx = BlockShapes.BlockToShapeMapping[this.Info.Name][0];
            return BlockShapes.Shapes[shapeIdx];
        }
    }
}
