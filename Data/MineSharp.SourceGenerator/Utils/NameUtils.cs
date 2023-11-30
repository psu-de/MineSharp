using Humanizer;

namespace MineSharp.SourceGenerator.Utils;

public static class NameUtils
{
    private static string CommonGetName(string x)
        => x.Pascalize();
    
    public static string GetMaterial(string x)
    {
        if (x == "coweb")
            return "Cobweb";
        if (x.StartsWith("mineable/"))
            return x.Substring("mineable/".Length).Pascalize();

        return CommonGetName(x);
    }

    public static string GetItemName(string name)
    {
        name = CommonGetName(name);

        return name switch {
            "PotteryShardArcher" => "ArcherPotterySherd",
            "PotteryShardPrize" => "PrizePotterySherd",
            "PotteryShardArmsUp" => "ArmsUpPotterySherd",
            "PotteryShardSkull" => "SkullPotterySherd",
            _ => name
        };
    }

    public static string GetBiomeName(string name)
        => CommonGetName(name);

    public static string GetBlockName(string name)
        => CommonGetName(name);

    public static string GetEffectName(string name)
        => CommonGetName(name);

    public static string GetEnchantmentName(string name)
        => CommonGetName(name);

    public static string GetEntityName(string name)
        => CommonGetName(name);
}
