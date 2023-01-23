namespace MineSharp.Core.Types
{
    public class BlockProperties
    {
        public BlockStateProperty[] Properties;

        public BlockProperties(BlockStateProperty[] properties)
        {
            this.Properties = properties;
        }

        public T GetPropertyValue<T>(string name, int state)
        {
            for (int i = this.Properties.Length - 1; i >= 0; i--)
            {
                var prop = this.Properties[i];
                if (prop.Name == name)
                {
                    return prop.GetValue<T>(state % prop.NumValues);
                }
                state /= prop.NumValues;
            }
            return default(T)!;
        }
    }

    public class BlockStateProperty
    {
        public enum BlockStatePropertyType
        {
            Enum,
            Bool,
            Int
        }

        public BlockStateProperty(string name, BlockStatePropertyType type, int numValues, string[]? values)
        {
            this.Name = name;
            this.Type = type;
            this.NumValues = numValues;
            this.AcceptedValues = values;
        }

        public string Name { get; set; }
        public BlockStatePropertyType Type { get; set; }
        public int NumValues { get; set; }
        public string[]? AcceptedValues { get; set; }

        public T GetValue<T>(int state)
        {
            switch (this.Type)
            {
                case BlockStatePropertyType.Int:
                    if (typeof(T) != typeof(int)) throw new NotSupportedException();
                    return (T)(object)state;
                case BlockStatePropertyType.Bool:
                    if (typeof(T) != typeof(bool)) throw new NotSupportedException();
                    return (T)(object)!Convert.ToBoolean(state);
                case BlockStatePropertyType.Enum:
                    if (typeof(T) != typeof(string) || this.AcceptedValues == null) throw new NotSupportedException();
                    return (T)(object)this.AcceptedValues[state];
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class BlockInfo
    {
        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public float Hardness { get; }
        public float Resistance { get; }
        public bool Diggable { get; }
        public bool Transparent { get; }
        public int FilterLight { get; }
        public int EmitLight { get; }
        public string BoundingBox { get; }
        public int StackSize { get; }
        public string Material { get; }
        public int DefaultState { get; }
        public int MinStateId { get; }
        public int MaxStateId { get; }
        public int[]? HarvestTools { get; }
        public BlockProperties Properties { get; }
        public int[] BlockShapeIndices { get; }
        
        public BlockInfo(int id, string name, string displayName, float hardness, float resistance, bool diggable, bool transparent, int filterLight, int emitLight, string boundingBox, int stackSize, string material, int defaultState, int minStateId, int maxStateId, int[]? harvestTools, BlockProperties properties, int[] blockShapeIndices)
        {
            this.Id = id;
            this.Name = name;
            this.DisplayName = displayName;
            this.Hardness = hardness;
            this.Resistance = resistance;
            this.Diggable = diggable;
            this.Transparent = transparent;
            this.FilterLight = filterLight;
            this.EmitLight = emitLight;
            this.BoundingBox = boundingBox;
            this.StackSize = stackSize;
            this.Material = material;
            this.DefaultState = defaultState;
            this.MinStateId = minStateId;
            this.MaxStateId = maxStateId;
            this.HarvestTools = harvestTools;
            this.Properties = properties;
            this.BlockShapeIndices = blockShapeIndices;
        }
    }

    public class Block
    {
        public BlockInfo Info { get; }
        public int State { get; set; }
        public Position Position { get; set; }
        public int Metadata => (int)this.State - this.Info.MinStateId;

        public Block(BlockInfo info, int state, Position pos)
        {
            this.Info = info;
            this.State = state;
            this.Position = pos;
        }

        public T GetProperty<T>(string name)
        {
            return this.Info.Properties.GetPropertyValue<T>(name, this.Metadata);
        }

        public override string ToString() => $"Block (Name={this.Info.Name} Id={this.Info.Id} StateId={this.State} Position={this.Position})";
    }
}
