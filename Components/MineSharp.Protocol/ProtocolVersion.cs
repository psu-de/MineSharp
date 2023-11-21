namespace MineSharp.Protocol;

public static class ProtocolVersion
{
    public const int V_1_20_2 = 764;
    public const int V_1_20 = 763;
    
    public const int V_1_19_4 = 762;
    public const int V_1_19_3 = 761;
    public const int V_1_19_2 = 760;
    public const int V_1_19 = 759;

    public const int V_18_2 = 758;
    public const int V_18_1 = 757;
    public const int V_18 = 757;

    public static bool IsBetween(int version, int min, int max)
    {
        return version >= min && version <= max;
    }
}
