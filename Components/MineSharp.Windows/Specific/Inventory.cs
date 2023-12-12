namespace MineSharp.Windows.Specific;

public class Inventory : Window
{
    public Inventory(Window? inventory = null, WindowSynchronizer? windowSynchronizer = null) 
        : base(INVENTORY_ID, "Inventory", 9, inventory, windowSynchronizer)
    { }

    public Core.Common.Slot GetSlot(Slot slot)
    {
        return this.GetSlot((short)slot);
    }

    public enum Slot
    {
        CraftingResult = 0,
        Helmet = 5,
        Chest = 6,
        Leggings = 7,
        Boots = 8,
        OffHand = 45
    }
}