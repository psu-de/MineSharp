using Humanizer;

namespace MineSharp.SourceGenerator.Utils;

public static class NameUtils
{
    public static string GetMaterial(string x)
    {
        if (x == "coweb")
            return "Cobweb";
        if (x.StartsWith("mineable/"))
            return x.Substring("mineable/".Length).Pascalize();

        return x.Pascalize();
    }

    public static string GetItemName(string name)
    {
        name = name.Pascalize();

        return name switch {
            "PotteryShardArcher" => "ArcherPotterySherd",
            "PotteryShardPrize" => "PrizePotterySherd",
            "PotteryShardArmsUp" => "ArmsUpPotterySherd",
            "PotteryShardSkull" => "SkullPotterySherd",
            _ => name
        };
    }
}
