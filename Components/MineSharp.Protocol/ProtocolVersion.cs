namespace MineSharp.Protocol;

/// <summary>
/// Utility class for Minecraft's protocol versions
/// </summary>
public static class ProtocolVersion
{
    /// <summary>
    /// Protocol version of Minecraft Java 1.20.3 & 1.20.4
    /// </summary>
    public const int V_1_20_3 = 765;
    
    /// <summary>
    /// Protocol version of Minecraft Java 1.20.2
    /// </summary>
    public const int V_1_20_2 = 764;
    
    /// <summary>
    /// Protocol version of Minecraft Java 1.20 and 1.20.1
    /// </summary>
    public const int V_1_20 = 763;
    
    /// <summary>
    /// Protocol version of Minecraft Java 1.19.4
    /// </summary>
    public const int V_1_19_4 = 762;
    
    /// <summary>
    /// Protocol version of Minecraft Java 1.19.3
    /// </summary>
    public const int V_1_19_3 = 761;
    
    /// <summary>
    /// Protocol version of Minecraft Java 1.19.2
    /// </summary>
    public const int V_1_19_2 = 760;
    
    /// <summary>
    /// Protocol version of Minecraft Java 1.19 and 1.19.1
    /// </summary>
    public const int V_1_19 = 759;

    /// <summary>
    /// Protocol version of Minecraft Java 1.18.2
    /// </summary>
    public const int V_18_2 = 758;
    
    /// <summary>
    /// Protocol version of Minecraft Java 1.18.1
    /// </summary>
    public const int V_18_1 = 757;
    
    /// <summary>
    /// Protocol version of Minecraft Java 1.18
    /// </summary>
    public const int V_18 = 757;

    /// <summary>
    /// Whether the <paramref name="version"/> is between <paramref name="min"/> and <paramref name="max"/>
    /// </summary>
    /// <param name="version"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static bool IsBetween(int version, int min, int max)
    {
        return version >= min && version <= max;
    }
}
