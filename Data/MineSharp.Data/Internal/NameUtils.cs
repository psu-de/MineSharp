using Humanizer;

namespace MineSharp.Data.Internal;

// IMPORTANT: Must be the same code as MineSharp.SourceGenerator/Utils/NameUtils.cs
internal static class NameUtils
{
    private static string CommonGetName(string x)
    {
        return x.Pascalize();
    }

    public static string GetMaterial(string x)
    {
        if (x == "coweb")
        {
            return "Cobweb";
        }

        if (x.StartsWith("mineable/"))
        {
            return x.Substring("mineable/".Length).Pascalize();
        }

        return CommonGetName(x);
    }

    public static string GetItemName(string name)
    {
        name = CommonGetName(name);

        return name switch
        {
            "PotteryShardArcher" => "ArcherPotterySherd",
            "PotteryShardPrize" => "PrizePotterySherd",
            "PotteryShardArmsUp" => "ArmsUpPotterySherd",
            "PotteryShardSkull" => "SkullPotterySherd",
            _ => name
        };
    }

    public static string GetBiomeName(string name)
    {
        return CommonGetName(name);
    }

    public static string GetBiomeCategory(string name)
    {
        if (name == "icy")
        {
            name = "ice";
        }

        return CommonGetName(name);
    }

    public static string GetBlockName(string name)
    {
        return CommonGetName(name);
    }

    public static string GetEffectName(string name)
    {
        return CommonGetName(name);
    }

    public static string GetEnchantmentName(string name)
    {
        return CommonGetName(name);
    }

    public static string GetEnchantmentCategory(string name)
    {
        return CommonGetName(name);
    }

    public static string GetEntityName(string name)
    {
        return CommonGetName(name);
    }

    public static string GetEntityCategory(string name)
    {
        if (name == "UNKNOWN")
        {
            name = name.ToLower();
        }

        return CommonGetName(name);
    }

    public static string GetDimensionName(string name)
    {
        return CommonGetName(name);
    }

    public static string GetGameState(string name)
    {
        return CommonGetName(name);
    }
    
    public static string GetParticleName(string name)
    {
        return CommonGetName(name); 
    }

    public static string GetPacketName(string name, string direction, string ns)
    {
        direction = direction == "toClient" ? "CB" : "SB";
        ns = ns == "handshaking" ? "Handshake" : ns.Pascalize();
        name = name.Pascalize()
                   .Replace("Packet", "")
                   .Replace("ConfiguationAcknowledged", "ConfigurationAcknowledged");

        return $"{direction}_{ns}_{name}";
    }
}
