using MineSharp.Core.Types;

namespace MineSharp.Data.Windows
{
    public class WindowInfo
    {

        public Identifier Name { get; set; }
        public string Title { get; set; }
        public int UniqueSlots { get; private set; }
        public bool ExcludeInventory { get; private set; }
        public bool HasOffHandSlot { get; private set; }

        public WindowInfo(Identifier name, string title, int uniqueSlots, bool excludeInventory = false, bool hasOffHandSlot = false)
        {
            this.Name = name;
            this.Title = title;
            this.UniqueSlots = uniqueSlots;
            this.ExcludeInventory = excludeInventory;
            this.HasOffHandSlot = hasOffHandSlot;
        }

    }

    public enum PlayerWindowSlots
    {
        CraftingOutput = 0,
        CraftingInputStart = 1,
        CraftingInputEnd = 4,
        ArmorStart = 5,
        ArmorEnd = 8,
        InventoryStart = 9,
        InventoryEnd = 35,
        HotbarStart = 36,
        HotbarEnd = 44,
        OffhandSlot = 45
    }

    public enum ChestWindowSlots
    {
        ChestStart = 0,
        ChestEnd = 26,
        InventoryStart = 27,
        InventoryEnd = 53,
        HotbarStart = 54,
        HotbarEnd = 62
    }

    public enum LargeChestWindowSlots
    {
        ChestStart = 0,
        ChestEnd = 53,
        InventoryStart = 54,
        InventoryEnd = 80,
        HotbarStart = 81,
        HotbarEnd = 89
    }

    public enum CraftingTableWindowSlots
    {
        CraftingOutput = 0,
        CraftingInputStart = 1,
        CraftingInputEnd = 9,
        InventoryStart = 10,
        InventoryEnd = 36,
        HotbarStart = 37,
        HotbarEnd = 45
    }

    public enum FurnaceWindowSlots
    {
        Ingredient = 0,
        Fuel = 1,
        Output = 2,
        InventoryStart = 3,
        InventoryEnd = 29,
        HotbarStart = 30,
        HotbarEnd = 38
    }

    public enum BlastFurnaceWindowSlots
    {
        Ingredient = 0,
        Fuel = 1,
        Output = 2,
        InventoryStart = 3,
        InventoryEnd = 29,
        HotbarStart = 30,
        HotbarEnd = 38
    }

    public enum SmokerWindowSlots
    {
        Ingredient = 0,
        Fuel = 1,
        Output = 2,
        InventoryStart = 3,
        InventoryEnd = 29,
        HotbarStart = 30,
        HotbarEnd = 38
    }

    public enum DispenserWindowSlots
    {
        ContentStart = 0,
        ContentEnd = 8,
        InventoryStart = 9,
        InventoryEnd = 35,
        HotbarStart = 36,
        HotbarEnd = 44
    }

    public enum EnchantmentTableWindowSlots
    {
        ItemToEnchant = 0,
        LapisLazuli = 1,
        InventoryStart = 2,
        InventoryEnd = 28,
        HotbarStart = 29,
        HotbarEnd = 37
    }

    public enum BrewingTableWindowSlots
    {
        BottlesStart = 0,
        BottlesEnd = 2,
        PotionIngredient = 3,
        BlazePower = 4,
        InventoryStart = 5,
        InventoryEnd = 31,
        HotbarStart = 32,
        HotbarEnd = 40
    }

    public enum VillagerTradingWindowSlots
    {
        InputStart = 0,
        InputEnd = 1,
        Result = 2,
        InventoryStart = 3,
        InventoryEnd = 29,
        HotbarStart = 30,
        HotbarEnd = 38
    }

    public enum BeaconWindowSlots
    {
        PaymentItem = 0,
        InventoryStart = 1,
        InventoryEnd = 27,
        HotbarStart = 28,
        HotbarEnd = 36
    }

    public enum AnvilWindowSlots
    {
        FirstItem = 0,
        SecondItem = 1,
        Result = 2,
        InventoryStart = 3,
        InventoryEnd = 29,
        HotbarStart = 30,
        HotbarEnd = 38
    }

    public enum HopperWindowSlots
    {
        HopperSlotsStart = 0,
        HopperSlotsEnd = 4,
        InventoryStart = 5,
        InventoryEnd = 31,
        HotbarStart = 32,
        HotbarEnd = 40
    }

    public enum ShulkerBoxWindowSlots
    {
        BoxStart = 0,
        BoxEnd = 26,
        InventoryStart = 27,
        InventoryEnd = 53,
        HotbarStart = 54,
        HotbarEnd = 62
    }

    public enum LlamaWindowSlots
    {
        Saddle = 0,
        Carpet = 1,
        LlamaInventoryStart = 2,
        LlamaInventorySlotsPerStrength = 3
    }

    public enum HorseWindowSlots
    {
        Saddle = 0,
        Armor = 1,
        InventoryStart = 2,
        InventortyEnd = 28,
        HotbarStart = 29,
        HotbarEnd = 37
    }

    public enum UnchestedDonkeyWindowSlots
    {
        Saddle = 0,
        Armor = 1,
        InventoryStart = 2,
        InventoryEnd = 28,
        HotbarStart = 29,
        HotbarEnd = 37
    }

    public enum ChestedDonkeyWindowSlots
    {
        Saddle = 0,
        Armor = 1,
        DonkeyInventoryStart = 2,
        DonkeyInventoryEnd = 16,
        InventoryStart = 17,
        InventoryEnd = 43,
        HotbarStart = 44,
        HotbarEnd = 52
    }

    public enum CartographyTableWindowSlots
    {
        Map = 0,
        Paper = 1,
        Output = 2,
        InventoryStart = 3,
        InventoryEnd = 29,
        HotbarStart = 30,
        HotbarEnd = 38
    }

    public enum GrindstoneWindowSlots
    {
        FirstItem = 0,
        SecondItem = 1,
        Result = 2,
        InventoryStart = 3,
        InventoryEnd = 29,
        HotbarStart = 30,
        HotbarEnd = 38
    }

    public enum LecternWindowSlots
    {
        Book = 0
    }

    public enum LoomWindowSlots
    {
        Banner = 0,
        Dye = 1,
        Pattern = 2,
        Result = 3,
        InventoryStart = 4,
        InventoryEnd = 30,
        HotbarStart = 31,
        HotbarEnd = 39
    }

    public enum StonecutterWindowSlots
    {
        Input = 0,
        Result = 1,
        InventoryStart = 2,
        InventoryEnd = 28,
        HotbarStart = 29,
        HotbarEnd = 37
    }
}
