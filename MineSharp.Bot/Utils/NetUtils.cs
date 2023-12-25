namespace MineSharp.Bot.Utils;

internal static class NetUtils
{
    private const double VELOCITY_CONVERSION = 8000d;
    
    public static double ConvertToVelocity(double value)
        => value / VELOCITY_CONVERSION;

    public static double ConvertDeltaPosition(short delta)
        => delta / (128 * 32d);

    public static float FromAngleByte(sbyte angle)
        => angle * 360.0f / 256.0f;
    
}
