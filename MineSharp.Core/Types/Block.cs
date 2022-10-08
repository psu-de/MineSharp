namespace MineSharp.Core.Types
{
    public class BlockProperties
    {


        public BlockStateProperty[] Properties;
        public int State;

        public BlockProperties(BlockStateProperty[] properties, int defaultState)
        {
            this.Properties = properties;
            this.State = defaultState;
            this.Set(defaultState);
        }

        public BlockStateProperty? Get(string name)
        {
            return this.Properties.FirstOrDefault(p => p.Name == name);
        }

        public void Set(int data)
        {
            this.State = data;
            foreach (var property in this.Properties.Reverse())
            {
                property.SetValue(data % property.NumValues);
                data = data / property.NumValues;
            }
        }

        public BlockProperties Clone() => new BlockProperties(this.Properties.Clone() as BlockStateProperty[] ?? throw new Exception(), this.State);
    }

    public class BlockStateProperty
    {

        public string Name { get; set; }
        public BlockStatePropertyType Type { get; set; }
        public int State { get; set; } = 0;
        public int NumValues { get; set; }
        public string[]? AcceptedValues { get; set; }

        public BlockStateProperty(string name, BlockStatePropertyType type, int numValues, string[]? values)
        {
            this.Name = name;
            this.Type = type;
            this.NumValues = numValues;
            this.AcceptedValues = values;
        }


        public void SetValue(int state)
        {
            if (state >= this.NumValues) throw new ArgumentOutOfRangeException();

            this.State = state;
        }

        public T GetValue<T>()
        {
            switch (this.Type)
            {
                case BlockStatePropertyType.Int:
                    if (typeof(T) != typeof(int)) throw new NotSupportedException();
                    else return (T)(object)this.State;
                case BlockStatePropertyType.Bool:
                    if (typeof(T) != typeof(bool)) throw new NotSupportedException();
                    else return (T)(object)!Convert.ToBoolean(this.State);
                case BlockStatePropertyType.Enum:
                    if (typeof(T) != typeof(string) || this.AcceptedValues == null) throw new NotSupportedException();
                    else return (T)(object)this.AcceptedValues[this.State];
                default:
                    throw new NotImplementedException();

            }
        }

        public enum BlockStatePropertyType
        {
            Enum,
            Bool,
            Int
        }
    }


    public class Block
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

        public int? State { get; set; }
        public Position? Position { get; set; }
        public int Metadata => (int)this.State! - this.MinStateId;

        public Block(int id, string name, string displayName, float hardness, float resistance, bool diggable, bool transparent, int filterLight, int emitLight, string boundingBox, int stackSize, string material, int defaultState, int minStateId, int maxStateId, int[]? harvestTools, BlockProperties properties)
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
        }

        public Block(int state, Position pos, int id, string name, string displayName, float hardness, float resistance, bool diggable, bool transparent, int filterLight, int emitLight, string boundingBox, int stackSize, string material, int defaultState, int minStateId, int maxStateId, int[]? harvestTools, BlockProperties properties)
            : this(id, name, displayName, hardness, resistance, diggable, transparent, filterLight, emitLight, boundingBox, stackSize, material, defaultState, minStateId, maxStateId, harvestTools, properties)
        {
            this.State = state;
            this.Position = pos;
            this.Properties.Set(this.Metadata);
        }

        public override string ToString() => $"Block (Name={this.Name} Id={this.Id} StateId={this.State} Position={this.Position})";
    }
}
