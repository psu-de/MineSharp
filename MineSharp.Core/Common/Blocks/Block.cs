using MineSharp.Core.Common.Items;

namespace MineSharp.Core.Common.Blocks;

public class Block
{
    public BlockInfo Info { get; }
    public int State { get; set; }
    public Position Position { get; set; }
    public int Metadata => this.State - this.Info.MinState;
    
    public Block(BlockInfo info, int state, Position position)
    {
        this.Info = info;
        this.State = state;
        this.Position = position;
    }

    public T GetProperty<T>(string name) where T : struct
        => this.Info.State.GetPropertyValue<T>(name, this.State);


    public bool IsSolid()
        => this.Info.IsSolid();

    public bool CanBeHarvestedBy(ItemType? item)
        => this.Info.HarvestTools == null 
           || (item != null && this.Info.HarvestTools.Contains(item.Value));

    public override string ToString() => $"Block (Position={Position}, State={State}, Info={Info})";
}
