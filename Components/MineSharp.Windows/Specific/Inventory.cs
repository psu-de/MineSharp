namespace MineSharp.Windows.Specific;

/// <summary>
///     Helper class for the Player inventory
/// </summary>
/// <param name="inventory"></param>
/// <param name="windowSynchronizer"></param>
public class Inventory(Window? inventory = null, Window.WindowSynchronizer? windowSynchronizer = null)
    : Window(InventoryId, "Inventory", 9, inventory, windowSynchronizer)
{
    /// <summary>
    ///     Slot indexes for the Inventory
    /// </summary>
    public enum Slot
    {
        /// <summary>
        ///     The player's 2x2 crafting result slot
        /// </summary>
        CraftingResult = 0,

        /// <summary>
        ///     The helmet slot index
        /// </summary>
        Helmet = 5,

        /// <summary>
        ///     The chest plate slot index
        /// </summary>
        Chest = 6,

        /// <summary>
        ///     The leggins slot index
        /// </summary>
        Leggings = 7,

        /// <summary>
        ///     The boots slot index
        /// </summary>
        Boots = 8,

        /// <summary>
        ///     The offhand slot index
        /// </summary>
        OffHand = 45
    }

    /// <summary>
    ///     Get a slot by the slot enum
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public Core.Common.Slot GetSlot(Slot slot)
    {
        return GetSlot((short)slot);
    }
}
