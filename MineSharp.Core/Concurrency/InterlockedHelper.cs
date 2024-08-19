using System.Runtime.CompilerServices;

namespace MineSharp.Core.Concurrency;

public static class InterlockedHelper
{
    /// <summary>
    /// Compares two 32-bit signed integers for equality and, if they are not equal to the specified value, replaces the first value.
    /// </summary>
    /// <param name="location1">The variable to set to the specified value.</param>
    /// <param name="value">The value to which the <paramref name="location1"/> is set if the comparison succeeds.</param>
    /// <param name="notComparand">The value that is compared to the value at <paramref name="location1"/>.</param>
    /// <returns>The original value of <paramref name="location1"/>.</returns>
    public static int CompareExchangeIfNot(ref int location1, int value, int notComparand)
    {
        var newCurrent = location1;
        var oldCurrent = newCurrent;
        do
        {
            oldCurrent = newCurrent;
            if (oldCurrent == notComparand)
            {
                return oldCurrent;
            }
        } while ((newCurrent = Interlocked.CompareExchange(ref location1, value, oldCurrent)) != oldCurrent);

        return newCurrent;
    }

    /// <summary>
    /// Compares two 64-bit signed integers for equality and, if they are not equal to the specified value, replaces the first value.
    /// </summary>
    /// <param name="location1">The variable to set to the specified value.</param>
    /// <param name="value">The value to which the <paramref name="location1"/> is set if the comparison succeeds.</param>
    /// <param name="notComparand">The value that is compared to the value at <paramref name="location1"/>.</param>
    /// <returns>The original value of <paramref name="location1"/>.</returns>
    public static long CompareExchangeIfNot(ref long location1, long value, long notComparand)
    {
        var newCurrent = location1;
        var oldCurrent = newCurrent;
        do
        {
            oldCurrent = newCurrent;
            if (oldCurrent == notComparand)
            {
                return oldCurrent;
            }
        } while ((newCurrent = Interlocked.CompareExchange(ref location1, value, oldCurrent)) != oldCurrent);

        return newCurrent;
    }

    #region Enum

    /// <summary>
    /// Exchanges the enum value of the specified location with the given value.
    /// 
    /// Note: This is very slow and also a bit unsafe but it's the only way to do it before .NET 9
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be an enum.</typeparam>
    /// <param name="location1">The variable to set to the specified value.</param>
    /// <param name="value">The value to which the <paramref name="location1"/> is set.</param>
    /// <returns>The original value of <paramref name="location1"/>.</returns>
    public static T Exchange<T>(ref T location1, T value)
        where T : unmanaged, Enum
    {
        switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
        {
            case TypeCode.UInt32:
            case TypeCode.Int32:
                var resultInt = Interlocked.Exchange(ref Unsafe.As<T, int>(ref location1), Unsafe.As<T, int>(ref value));
                return Unsafe.As<int, T>(ref resultInt);
            case TypeCode.UInt64:
            case TypeCode.Int64:
                var resultLong = Interlocked.Exchange(ref Unsafe.As<T, long>(ref location1), Unsafe.As<T, long>(ref value));
                return Unsafe.As<long, T>(ref resultLong);
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Compares two enum values for equality and, if they are equal, replaces the first value.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be an enum.</typeparam>
    /// <param name="location1">The variable to set to the specified value.</param>
    /// <param name="value">The value to which the <paramref name="location1"/> is set if the comparison succeeds.</param>
    /// <param name="comparand">The value that is compared to the value at <paramref name="location1"/>.</param>
    /// <returns>The original value of <paramref name="location1"/>.</returns>
    public static T CompareExchange<T>(ref T location1, T value, T comparand)
        where T : unmanaged, Enum
    {
        switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
        {
            case TypeCode.UInt32:
            case TypeCode.Int32:
                var resultInt = Interlocked.CompareExchange(ref Unsafe.As<T, int>(ref location1), Unsafe.As<T, int>(ref value), Unsafe.As<T, int>(ref comparand));
                return Unsafe.As<int, T>(ref resultInt);
            case TypeCode.UInt64:
            case TypeCode.Int64:
                var resultLong = Interlocked.CompareExchange(ref Unsafe.As<T, long>(ref location1), Unsafe.As<T, long>(ref value), Unsafe.As<T, long>(ref comparand));
                return Unsafe.As<long, T>(ref resultLong);
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Compares two enum values for equality and, if they are not equal to the specified value, replaces the first value.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be an enum.</typeparam>
    /// <param name="location1">The variable to set to the specified value.</param>
    /// <param name="value">The value to which the <paramref name="location1"/> is set if the comparison succeeds.</param>
    /// <param name="notComparand">The value that is compared to the value at <paramref name="location1"/>.</param>
    /// <returns>The original value of <paramref name="location1"/>.</returns>
    public static T CompareExchangeIfNot<T>(ref T location1, T value, T notComparand)
        where T : unmanaged, Enum
    {
        switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
        {
            case TypeCode.UInt32:
            case TypeCode.Int32:
                var resultInt = CompareExchangeIfNot(ref Unsafe.As<T, int>(ref location1), Unsafe.As<T, int>(ref value), Unsafe.As<T, int>(ref notComparand));
                return Unsafe.As<int, T>(ref resultInt);
            case TypeCode.UInt64:
            case TypeCode.Int64:
                var resultLong = CompareExchangeIfNot(ref Unsafe.As<T, long>(ref location1), Unsafe.As<T, long>(ref value), Unsafe.As<T, long>(ref notComparand));
                return Unsafe.As<long, T>(ref resultLong);
            default:
                throw new NotSupportedException();
        }
    }
    #endregion
}
