namespace MineSharp.Core;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
/// <summary>
///     Enum for all the protocol versions for non- preview/snapshot/beta versions of Minecraft Java Edition.
///     If multiple Minecraft versions have the same protocol version only the lowest Minecraft versions gets an enum value here.
/// </summary>
public enum ProtocolVersion
{
    Unknown = -1,
    // older Minecraft versions did not use Netty and not these protocol ids

    V_1_7_0 = 3,
    V_1_7_2 = 4,
    V_1_7_6 = 5,
    V_1_8_0 = 47,
    V_1_9_0 = 107,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_9_1 = 108,
    V_1_9_2 = 109,
    V_1_9_3 = 110,
    V_1_10_0 = 210,
    V_1_11_0 = 315,
    V_1_11_1 = 316,
    V_1_12_0 = 335,
    V_1_12_1 = 338,
    V_1_12_2 = 340,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_13_0 = 393,
    V_1_13_1 = 401,
    V_1_13_2 = 404,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_14_0 = 477,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_14_1 = 480,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_14_2 = 485,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_14_3 = 490,
    V_1_14_4 = 498,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_15_0 = 573,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_15_1 = 575,
    V_1_15_2 = 578,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_16_0 = 735,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_16_1 = 736,
    /// <summary>
    /// Pre-release
    /// </summary>
    V_1_16_2 = 751,
    V_1_16_3 = 753,
    V_1_16_4 = 754,
    V_1_17_0 = 755,
    V_1_17_1 = 756,
    V_1_18_0 = 757,
    V_1_18_2 = 758,
    V_1_19_0 = 759,
    V_1_19_1 = 760,
    V_1_19_3 = 761,
    V_1_19_4 = 762,
    V_1_20_0 = 763,
    V_1_20_2 = 764,
    V_1_20_3 = 765,
    V_1_20_5 = 766,
    V_1_21_0 = 767,

    // add new protocol versions here
}

public static class ProtocolVersionExtensions
{
    /// <summary>
    ///     Whether the <paramref name="version" /> is between <paramref name="min" /> and <paramref name="max" />
    /// </summary>
    public static bool IsBetween(this ProtocolVersion version, ProtocolVersion min, ProtocolVersion max)
    {
        return version >= min && version <= max;
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
