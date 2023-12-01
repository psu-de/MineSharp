using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Data;

namespace MineSharp.Windows.Tests;

public class WindowTests
{
    private Window _mainInventory;
    private Window _inventory;
    private MinecraftData _data;

    private ItemInfo _oakLog;
    private ItemInfo _netherStar;
    private ItemInfo _diamond;

    [SetUp]
    public void Setup()
    {
        this._data = MinecraftData.FromVersion("1.19.3");
        this._mainInventory = new Window(255, "", 4 * 9, null, null);
        this._inventory = new Window(0, "Inventory", 9, this._mainInventory, null);

        this._oakLog = this._data.Items.GetByName("oak_log");
        this._netherStar = this._data.Items.GetByName("nether_star");
        this._diamond = this._data.Items.GetByName("diamond");
    }
    
    [Test]
    public void Create()
    {
        var w1 = new Window(
            255, "TestInventory", 4 * 9, null, null);
        var w2 = new Window(
            0, "inventory", 9, w1, null);
    }

    [Test]
    public void Slots()
    {
        Assert.Multiple(() =>
        {
            Assert.That(this._mainInventory.TotalSlotCount, Is.EqualTo(4 * 9));
            Assert.That(this._mainInventory.Slots.Length, Is.EqualTo(4 * 9));
            Assert.That(this._inventory.Slots.Length, Is.EqualTo(9));
            Assert.That(this._inventory.TotalSlotCount, Is.EqualTo(5 * 9 + 1));
            Assert.That(this._inventory.GetSlot(45), Is.Not.Null);

            Assert.That(this._inventory.GetSelectedSlot(), Is.EqualTo(this._mainInventory.GetSelectedSlot()));

            Assert.That(this._inventory.IsContainerSlotIndex(1), Is.EqualTo(true));
            Assert.That(this._inventory.IsContainerSlotIndex(9), Is.EqualTo(false));
        });
    }

    [Test]
    public void GetSetSlots()
    {
        this._inventory.SetSlot(new Slot(
            new Item(this._oakLog, 16, null, null), 9));

        this._inventory.SetSlot(new Slot(
            new Item(this._netherStar, 2, null, null), -1));

        Assert.That(this._inventory.GetSlot(9).Item?.Info.Id, Is.EqualTo(this._oakLog.Id));
        Assert.That(this._mainInventory.GetSlot(0).Item?.Info.Id, Is.EqualTo(this._oakLog.Id));

        this._inventory.SetSlot(new Slot(
            new Item(this._diamond, 4, null, null), 1));
        
        Assert.Multiple(() =>
        {
            Assert.That(this._inventory.GetSlot(1).Item?.Info.Id, Is.EqualTo(this._diamond.Id));
            Assert.That(this._inventory.GetSlot(-1).Item?.Info.Id, Is.EqualTo(this._netherStar.Id));
            
            Assert.That(this._inventory.GetSlot(-1).Item?.Info.Id, Is.EqualTo(this._inventory.GetSelectedSlot().Item?.Info.Id));
            Assert.That(this._mainInventory.GetSelectedSlot().Item?.Info.Id, Is.EqualTo(this._netherStar.Id));
            
            Assert.Catch<IndexOutOfRangeException>(() => this._inventory.GetSlot(46));
            
            Assert.That(this._inventory.GetAllSlots().Length, Is.EqualTo(10 + 4 * 9));
            Assert.That(this._inventory.GetContainerSlots().Length, Is.EqualTo(10));
            Assert.That(this._inventory.GetInventorySlots().Length, Is.EqualTo(4 * 9));
            
            
            Assert.Catch<NotSupportedException>(() => this._mainInventory.GetInventorySlots());
        });
    }

    [Test]
    public void SimpleClickTest()
    {
        this._inventory.SetSlot(new Slot(
            new Item(this._diamond, 16, null, null), 9));

        this._inventory.SetSlot(new Slot(
            new Item(this._netherStar, 6, null, null), 17));

        this._inventory.SetSlot(new Slot(
            new Item(this._diamond, 54, null, null), 18));
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseRight, 9);
        Assert.That(this._inventory.GetSlot(9).Item?.Count, Is.EqualTo(8));
        Assert.That(this._inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(8));
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseRight, 9);
        Assert.That(this._inventory.GetSlot(9).Item?.Count, Is.EqualTo(9));
        Assert.That(this._inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(7));
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 10);
        Assert.That(this._inventory.GetSelectedSlot().Item, Is.Null);
        Assert.That(this._inventory.GetSlot(10).Item?.Count, Is.EqualTo(7));
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 10);
        Assert.That(this._inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(7));
        Assert.That(this._inventory.GetSlot(10).Item, Is.Null);
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 9);
        Assert.That(this._inventory.GetSelectedSlot().Item, Is.Null);
        Assert.That(this._inventory.GetSlot(9).Item?.Count, Is.EqualTo(16));
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 9);
        this._inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 18);
        Assert.That(this._inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(6));
        Assert.That(this._inventory.GetSlot(18).Item?.Count, Is.EqualTo(64));
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseRight, -999);
        Assert.That(this._inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(5));
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseLeft, -999);
        Assert.That(this._inventory.GetSelectedSlot().Item, Is.Null);
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 17);
        this._inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 18);
        Assert.That(this._inventory.GetSelectedSlot().Item?.Info.Id, Is.EqualTo(this._diamond.Id));
        Assert.That(this._inventory.GetSlot(18).Item?.Info.Id, Is.EqualTo(this._netherStar.Id));
        
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseRight, 18);
        Assert.That(this._inventory.GetSelectedSlot().Item?.Info.Id, Is.EqualTo(this._netherStar.Id));
        Assert.That(this._inventory.GetSlot(18).Item?.Info.Id, Is.EqualTo(this._diamond.Id));
    }

    [Test]
    public void PickupTest()
    {
        this._inventory.SetSlot(new Slot(
            new Item(this._diamond, 16, null, null), 9));
        this._inventory.SetSlot(new Slot(
            new Item(this._netherStar, 31, null, null), 10));
        this._inventory.SetSlot(new Slot(
            new Item(this._netherStar, 16, null, null), 11));
        
        Assert.That(this._inventory.GetSlot(9).Item?.Count, Is.EqualTo(16));
        
        this._inventory.PickupItems(9, 6);
        Assert.That(this._inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(6));
        Assert.That(this._inventory.GetSlot(9).Item?.Count, Is.EqualTo(10));
        
        this._inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 12);
        Assert.That(this._inventory.GetSelectedSlot().Item, Is.EqualTo(null));
        
        this._inventory.PickupInventoryItems(ItemType.NetherStar, 47);
        Assert.That(this._inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(47));
        Assert.That(this._inventory.GetSelectedSlot().Item?.Info.Type, Is.EqualTo(ItemType.NetherStar));
        
        Assert.Catch(() => this._inventory.PickupItems(9, 5));
    }

    [Test]
    public void MoveItemsTest()
    {
        this._inventory.SetSlot(new Slot(
            new Item(this._diamond, 64, null, null), 9));
        
        this._inventory.MoveItemsFromSlot(9, 10, 24);
        Assert.That(this._inventory.GetSlot(9).Item?.Count, Is.EqualTo(40));
        Assert.That(this._inventory.GetSlot(10).Item?.Count, Is.EqualTo(24));
        
        this._inventory.MoveItemsFromSlot(9, 10, 20);
        Assert.That(this._inventory.GetSlot(9).Item?.Count, Is.EqualTo(20));
        Assert.That(this._inventory.GetSlot(10).Item?.Count, Is.EqualTo(44));
        
        this._inventory.MoveItemsFromSlot(9, 10, 20);
        Assert.That(this._inventory.GetSlot(9).Item, Is.Null);
        Assert.That(this._inventory.GetSlot(10).Item?.Count, Is.EqualTo(64));
        
        this._inventory.MoveItemsFromSlot(10, 9, 1);
        Assert.That(this._inventory.GetSlot(9).Item?.Count, Is.EqualTo(1));
        Assert.That(this._inventory.GetSlot(10).Item?.Count, Is.EqualTo(63));
    }
}