namespace MineSharp.Data.Tests;

public class Tests
{
    private static readonly string[] VERSIONS =
    [
        "1.18", "1.18.1", "1.18.2",
        "1.19", "1.19.1", "1.19.2", "1.19.3", "1.19.4",
        "1.20", "1.20.1", "1.20.2", "1.20.3", "1.20.4"
    ];

    [Test]
    public void Test1()
    {
        foreach (var version in VERSIONS)
        {
            MinecraftData.FromVersion(version).Wait();
        }
    }
}