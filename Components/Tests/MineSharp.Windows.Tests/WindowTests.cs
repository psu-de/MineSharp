using MineSharp.Core.Common.Items;
using MineSharp.Data;

namespace MineSharp.Windows.Tests;

public class WindowTests
{
    private MinecraftData data;
    private ItemInfo diamond;
    private Window inventory;
    private Window mainInventory;
    private ItemInfo netherStar;

    private ItemInfo oakLog;

    [SetUp]
    public void Setup()
    {
        data = MinecraftData.FromVersion("1.19.3").Result;
        mainInventory = new(255, "", 4 * 9);
        inventory = new(0, "Inventory", 9, mainInventory);

        oakLog = data.Items.ByName("oak_log")!;
        netherStar = data.Items.ByName("nether_star")!;
        diamond = data.Items.ByName("diamond")!;
    }

    [Test]
    public void Create()
    {
        var w1 = new Window(
            255, "TestInventory", 4 * 9);
        var w2 = new Window(
            0, "inventory", 9, w1);
    }

    [Test]
    public void Slots()
    {
        Assert.Multiple(() =>
        {
            Assert.That(mainInventory.TotalSlotCount, Is.EqualTo(4 * 9));
            Assert.That(mainInventory.SlotCount, Is.EqualTo(4 * 9));
            Assert.That(inventory.SlotCount, Is.EqualTo(9));
            Assert.That(inventory.TotalSlotCount, Is.EqualTo((5 * 9) + 1));
            Assert.That(inventory.GetSlot(45), Is.Not.Null);

            Assert.That(inventory.GetSelectedSlot().Item, Is.EqualTo(mainInventory.GetSelectedSlot().Item));

            Assert.That(inventory.IsContainerSlotIndex(1), Is.EqualTo(true));
            Assert.That(inventory.IsContainerSlotIndex(9), Is.EqualTo(false));
        });
    }

    [Test]
    public void GetSetSlots()
    {
        inventory.SetSlot(new(
                              new(oakLog, 16, null, null), 9));

        inventory.SetSlot(new(
                              new(netherStar, 2, null, null), -1));

        Assert.That(inventory.GetSlot(9).Item?.Info.Id, Is.EqualTo(oakLog.Id));
        Assert.That(mainInventory.GetSlot(0).Item?.Info.Id, Is.EqualTo(oakLog.Id));

        inventory.SetSlot(new(
                              new(diamond, 4, null, null), 1));

        Assert.Multiple(() =>
        {
            Assert.That(inventory.GetSlot(1).Item?.Info.Id, Is.EqualTo(diamond.Id));
            Assert.That(inventory.GetSlot(-1).Item?.Info.Id, Is.EqualTo(netherStar.Id));

            Assert.That(inventory.GetSlot(-1).Item?.Info.Id, Is.EqualTo(inventory.GetSelectedSlot().Item?.Info.Id));
            Assert.That(mainInventory.GetSelectedSlot().Item?.Info.Id, Is.EqualTo(netherStar.Id));

            Assert.Catch<IndexOutOfRangeException>(() => inventory.GetSlot(46));

            Assert.That(inventory.GetAllSlots().Length, Is.EqualTo(10 + (4 * 9)));
            Assert.That(inventory.GetContainerSlots().Length, Is.EqualTo(10));
            Assert.That(inventory.GetInventorySlots().Length, Is.EqualTo(4 * 9));


            Assert.Catch<InvalidOperationException>(() => mainInventory.GetInventorySlots());
        });
    }

    [Test]
    public void SimpleClickTest()
    {
        inventory.SetSlot(new(
                              new(diamond, 16, null, null), 9));

        inventory.SetSlot(new(
                              new(netherStar, 6, null, null), 17));

        inventory.SetSlot(new(
                              new(diamond, 54, null, null), 18));

        inventory.DoSimpleClick(WindowMouseButton.MouseRight, 9);
        Assert.That(inventory.GetSlot(9).Item?.Count, Is.EqualTo(8));
        Assert.That(inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(8));

        inventory.DoSimpleClick(WindowMouseButton.MouseRight, 9);
        Assert.That(inventory.GetSlot(9).Item?.Count, Is.EqualTo(9));
        Assert.That(inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(7));

        inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 10);
        Assert.That(inventory.GetSelectedSlot().Item, Is.Null);
        Assert.That(inventory.GetSlot(10).Item?.Count, Is.EqualTo(7));

        inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 10);
        Assert.That(inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(7));
        Assert.That(inventory.GetSlot(10).Item, Is.Null);

        inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 9);
        Assert.That(inventory.GetSelectedSlot().Item, Is.Null);
        Assert.That(inventory.GetSlot(9).Item?.Count, Is.EqualTo(16));

        inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 9);
        inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 18);
        Assert.That(inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(6));
        Assert.That(inventory.GetSlot(18).Item?.Count, Is.EqualTo(64));

        inventory.DoSimpleClick(WindowMouseButton.MouseRight, -999);
        Assert.That(inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(5));

        inventory.DoSimpleClick(WindowMouseButton.MouseLeft, -999);
        Assert.That(inventory.GetSelectedSlot().Item, Is.Null);

        inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 17);
        inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 18);
        Assert.That(inventory.GetSelectedSlot().Item?.Info.Id, Is.EqualTo(diamond.Id));
        Assert.That(inventory.GetSlot(18).Item?.Info.Id, Is.EqualTo(netherStar.Id));


        inventory.DoSimpleClick(WindowMouseButton.MouseRight, 18);
        Assert.That(inventory.GetSelectedSlot().Item?.Info.Id, Is.EqualTo(netherStar.Id));
        Assert.That(inventory.GetSlot(18).Item?.Info.Id, Is.EqualTo(diamond.Id));
    }

    [Test]
    public void PickupTest()
    {
        inventory.SetSlot(new(
                              new(diamond, 16, null, null), 9));
        inventory.SetSlot(new(
                              new(netherStar, 31, null, null), 10));
        inventory.SetSlot(new(
                              new(netherStar, 16, null, null), 11));

        Assert.That(inventory.GetSlot(9).Item?.Count, Is.EqualTo(16));

        inventory.PickupItems(9, 6);
        Assert.That(inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(6));
        Assert.That(inventory.GetSlot(9).Item?.Count, Is.EqualTo(10));

        inventory.DoSimpleClick(WindowMouseButton.MouseLeft, 12);
        Assert.That(inventory.GetSelectedSlot().Item, Is.EqualTo(null));

        inventory.PickupInventoryItems(ItemType.NetherStar, 47);
        Assert.That(inventory.GetSelectedSlot().Item?.Count, Is.EqualTo(47));
        Assert.That(inventory.GetSelectedSlot().Item?.Info.Type, Is.EqualTo(ItemType.NetherStar));

        Assert.Catch(() => inventory.PickupItems(9, 5));
    }
}
