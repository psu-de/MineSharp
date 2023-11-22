using fNbt;

namespace MineSharp.Core.Common.Items;

public class Item
{
    public readonly ItemInfo Info;
    public byte Count { get; set; }
    public int? Damage { get; set; }
    public NbtCompound? Metadata { get; set; }
    
    public Item(ItemInfo info, byte count, int? damage, NbtCompound? metadata)
    {
        this.Info = info;
        this.Count = count;
        this.Damage = damage;
        this.Metadata = metadata;
    }

    public Item Clone()
    {
        return new Item(this.Info, Count, Damage, Metadata);
    }

    public override string ToString() 
        => $"Item(Info={Info}, Count={Count}, Damage={Damage}, Metadata={Metadata})";
}
