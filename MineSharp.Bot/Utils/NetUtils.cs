namespace MineSharp.Bot.Utils;

internal static class NetUtils
{
    private const double VelocityConversion = 8000d;

    public static double ConvertToVelocity(double value)
    {
        return value / VelocityConversion;
    }

    public static double ConvertDeltaPosition(short delta)
    {
        return delta / (128 * 32d);
    }

    public static float FromAngleByte(sbyte angle)
    {
        return angle * 360.0f / 256.0f;
    }
}
