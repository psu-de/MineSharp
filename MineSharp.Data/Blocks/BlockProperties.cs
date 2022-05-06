using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Blocks {
    public class BlockProperties {


        public BlockStateProperty[] Properties;
        public int State;

        public BlockProperties(BlockStateProperty[] properties, int defaultState) {
            this.Properties = properties;
            this.State = defaultState;
            this.Set(defaultState);
        }

        public BlockStateProperty? Get(string name) {
            return Properties.FirstOrDefault(p => p.Name == name);
        }

        public void Set(int data) {
            this.State = data;
            foreach (var property in this.Properties.Reverse()) {
                property.SetValue(data % property.NumValues);
                data = data / property.NumValues;
            }
        }

        public BlockProperties Clone() {
            return new BlockProperties(Properties.Clone() as BlockStateProperty[] ?? throw new Exception(), this.State);
        }
    }
}
