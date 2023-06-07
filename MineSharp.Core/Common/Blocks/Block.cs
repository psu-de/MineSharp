namespace MineSharp.Core.Common.Blocks;

public class Block
{
    public BlockInfo Info { get; }
    public int State { get; set; }
    public Position Position { get; set; }
    public int Metadata => (int)this.State! - this.Info.MinStateId;
    
    public Block(BlockInfo info, int state, Position position)
    {
        this.Info = info;
        this.State = state;
        this.Position = position;
    }

    public T GetProperty<T>(string name) => this.Info.Properties.GetPropertyValue<T>(name, this.State);

    public override string ToString() => $"Block (Position={Position}, State={State}, Info={Info})";
}
