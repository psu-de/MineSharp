namespace MineSharp.World.Tests.Containers;

using MineSharp.World.Containers;

public class IntBitArrayTests
{
    private static readonly long[] Data = { 0x0020863148418841, 0x01018A7260F68C87 };
    private static readonly int[] Expected = { 1, 2, 2, 3, 4, 4, 5, 6, 6, 4, 8, 0, 7, 4, 3, 13, 15, 16, 9, 14, 10, 12, 0, 2 };

    [SetUp]
    public void Setup()
    { }

    [Test]
    public void CapacityTest()
    {
        var array = new IntBitArray(Data, 5);
        Assert.That(array.Capacity, Is.EqualTo(24));
    }

    [Test]
    public void DataTest()
    {
        var array = new IntBitArray(Data, 5);

        for (int i = 0; i < array.Capacity; i++)
        {
            Assert.That(array.Get(i), Is.EqualTo(Expected[i]));
        }
    }

    [Test]
    public void IndexOutOfRange()
    {
        var array = new IntBitArray(Data, 5);

        Assert.Catch(() => array.Get(25));
        Assert.Catch(() => array.Set(25, 1));
    }

    [Test]
    public void ChaneBitsPerEntry()
    {
        var array = new IntBitArray(Data, 5);
        
        array.ChangeBitsPerEntry(5);
        
        array.ChangeBitsPerEntry(6);
        
        Assert.That(array.Data.Length, Is.EqualTo(3));
        Assert.That(array.Capacity, Is.EqualTo(30));
        
        for (int i = 0; i < array.Capacity; i++)
        {
            var expected = i >= Expected.Length ? 0 : Expected[i];
            Assert.That(array.Get(i), Is.EqualTo(expected));
        }
    }
}
