using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Blocks {
    public class BlockStateProperty  {

        public string Name { get; set; }
        public BlockStatePropertyType Type { get; set; }
        public int State { get; set; } = 0;
        public int NumValues { get; set; }
        public string[]? AcceptedValues { get; set; }

        public BlockStateProperty(string name, BlockStatePropertyType type, int numValues, string[]? values) {
            this.Name = name;
            this.Type = type;
            this.NumValues = numValues;
            this.AcceptedValues = values;   
        }


        public void SetValue(int state) {
            if (state >= NumValues) throw new ArgumentOutOfRangeException();

            this.State = state;
        }

        public T GetValue<T> () {
            switch (Type) {
                case BlockStatePropertyType.Int:
                    if (typeof(T) != typeof(int)) throw new NotSupportedException();
                    else return (T)(object)State;
                case BlockStatePropertyType.Bool:
                    if (typeof(T) != typeof(bool)) throw new NotSupportedException();
                    else return (T)(object)!Convert.ToBoolean(State);
                case BlockStatePropertyType.Enum:
                    if (typeof(T) != typeof(string) || this.AcceptedValues == null) throw new NotSupportedException();
                    else return (T)(object)this.AcceptedValues[State];
                default:
                    throw new NotImplementedException();

            }
        }

        public enum BlockStatePropertyType {
            Enum,
            Bool,
            Int
        }
    }
}
