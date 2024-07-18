namespace MineSharp.Core.Common;

// Thanks to MrZoidberg
// https://gist.github.com/MrZoidberg/9bac07cf3f5aa5896f75

/// <summary>
///     Represents an immutable Java universally unique identifier (UUID).
///     A UUID represents a 128-bit value.
/// </summary>
public struct Uuid : IEquatable<Uuid>
{
    /// <summary>
    ///     Empty UUID
    /// </summary>
    public static readonly Uuid Empty;

    static Uuid()
    {
        Empty = new();
    }

    /// <summary>
    ///     Constructs a new UUID using the specified data.
    /// </summary>
    /// <param name="mostSignificantBits">The most significant 64 bits of the UUID.</param>
    /// <param name="leastSignificantBits">The least significant 64 bits of the UUID</param>
    public Uuid(long mostSignificantBits, long leastSignificantBits)
    {
        MostSignificantBits = mostSignificantBits;
        LeastSignificantBits = leastSignificantBits;
    }

    /// <summary>
    ///     Constructs a new UUID using the specified data.
    /// </summary>
    /// <param name="b">Bytes array that represents the UUID.</param>
    public Uuid(byte[] b)
    {
        if (b == null)
        {
            throw new ArgumentNullException("b");
        }

        if (b.Length != 16)
        {
            throw new ArgumentException("Length of the UUID byte array should be 16");
        }

        MostSignificantBits = BitConverter.ToInt64(b, 0);
        LeastSignificantBits = BitConverter.ToInt64(b, 8);
    }

    /// <summary>
    ///     The least significant 64 bits of this UUID's 128 bit value.
    /// </summary>
    public long LeastSignificantBits { get; }

    /// <summary>
    ///     The most significant 64 bits of this UUID's 128 bit value.
    /// </summary>
    public long MostSignificantBits { get; }

    /// <summary>
    ///     Returns a value that indicates whether this instance is equal to a specified
    ///     object.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns>true if o is a <paramref name="obj" /> that has the same value as this instance; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (!(obj is Uuid))
        {
            return false;
        }

        var uuid = (Uuid)obj;

        return Equals(uuid);
    }

    /// <summary>
    ///     Returns a value that indicates whether this instance and a specified <paramref name="uuid" />
    ///     object represent the same value.
    /// </summary>
    /// <param name="uuid">An object to compare to this instance.</param>
    /// <returns>true if <paramref name="uuid" /> is equal to this instance; otherwise, false.</returns>
    public bool Equals(Uuid uuid)
    {
        return MostSignificantBits == uuid.MostSignificantBits && LeastSignificantBits == uuid.LeastSignificantBits;
    }

    /// <summary>
    ///     Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return ((Guid)this).GetHashCode();
    }

    /// <summary>
    ///     Returns a String object representing this UUID.
    /// </summary>
    /// <returns>A string representation of this UUID.</returns>
    public override string ToString()
    {
        //return ((Guid)this).ToString();
        return GetDigits(MostSignificantBits >> 32, 8) + "-" +
            GetDigits(MostSignificantBits >> 16, 4) + "-" +
            GetDigits(MostSignificantBits, 4) + "-" +
            GetDigits(LeastSignificantBits >> 48, 4) + "-" +
            GetDigits(LeastSignificantBits, 12);
    }

    /// <summary>
    ///     Returns a 16-element byte array that contains the value of this instance.
    /// </summary>
    /// <returns>A 16-element byte array</returns>
    public byte[] ToByteArray()
    {
        var uuidMostSignificantBytes = BitConverter.GetBytes(MostSignificantBits);
        var uuidLeastSignificantBytes = BitConverter.GetBytes(LeastSignificantBits);
        byte[] bytes =
        {
            uuidMostSignificantBytes[0], uuidMostSignificantBytes[1], uuidMostSignificantBytes[2],
            uuidMostSignificantBytes[3], uuidMostSignificantBytes[4], uuidMostSignificantBytes[5],
            uuidMostSignificantBytes[6], uuidMostSignificantBytes[7], uuidLeastSignificantBytes[0],
            uuidLeastSignificantBytes[1], uuidLeastSignificantBytes[2], uuidLeastSignificantBytes[3],
            uuidLeastSignificantBytes[4], uuidLeastSignificantBytes[5], uuidLeastSignificantBytes[6],
            uuidLeastSignificantBytes[7]
        };

        return bytes;
    }

    /// <summary>Indicates whether the values of two specified <see cref="T:Uuid" /> objects are equal.</summary>
    /// <returns>true if <paramref name="a" /> and <paramref name="b" /> are equal; otherwise, false.</returns>
    /// <param name="a">The first object to compare. </param>
    /// <param name="b">The second object to compare. </param>
    public static bool operator ==(Uuid a, Uuid b)
    {
        return a.Equals(b);
    }

    /// <summary>Indicates whether the values of two specified <see cref="T:Uuid" /> objects are not equal.</summary>
    /// <returns>true if <paramref name="a" /> and <paramref name="b" /> are not equal; otherwise, false.</returns>
    /// <param name="a">The first object to compare. </param>
    /// <param name="b">The second object to compare. </param>
    public static bool operator !=(Uuid a, Uuid b)
    {
        return !a.Equals(b);
    }

    /// <summary>Converts an <see cref="T:Uuid" /> to a <see cref="T:System.Guid" />.</summary>
    /// <param name="uuid">The value to convert. </param>
    /// <returns>A <see cref="T:System.Guid" /> that represents the converted <see cref="T:Uuid" />.</returns>
    public static explicit operator Guid(Uuid uuid)
    {
        if (uuid == default)
        {
            return default;
        }

        var uuidMostSignificantBytes = BitConverter.GetBytes(uuid.MostSignificantBits);
        var uuidLeastSignificantBytes = BitConverter.GetBytes(uuid.LeastSignificantBits);
        byte[] guidBytes =
        {
            uuidMostSignificantBytes[4], uuidMostSignificantBytes[5], uuidMostSignificantBytes[6],
            uuidMostSignificantBytes[7], uuidMostSignificantBytes[2], uuidMostSignificantBytes[3],
            uuidMostSignificantBytes[0], uuidMostSignificantBytes[1], uuidLeastSignificantBytes[7],
            uuidLeastSignificantBytes[6], uuidLeastSignificantBytes[5], uuidLeastSignificantBytes[4],
            uuidLeastSignificantBytes[3], uuidLeastSignificantBytes[2], uuidLeastSignificantBytes[1],
            uuidLeastSignificantBytes[0]
        };

        return new(guidBytes);
    }

    /// <summary>Converts a <see cref="T:System.Guid" /> to an <see cref="T:Uuid" />.</summary>
    /// <param name="value">The value to convert. </param>
    /// <returns>An <see cref="T:Uuid" /> that represents the converted <see cref="T:System.Guid" />.</returns>
    public static implicit operator Uuid(Guid value)
    {
        if (value == default)
        {
            return default;
        }

        var guidBytes = value.ToByteArray();
        byte[] uuidBytes =
        {
            guidBytes[6], guidBytes[7], guidBytes[4], guidBytes[5], guidBytes[0], guidBytes[1], guidBytes[2],
            guidBytes[3], guidBytes[15], guidBytes[14], guidBytes[13], guidBytes[12], guidBytes[11], guidBytes[10],
            guidBytes[9], guidBytes[8]
        };


        return new(BitConverter.ToInt64(uuidBytes, 0), BitConverter.ToInt64(uuidBytes, 8));
    }

    /// <summary>
    ///     Creates a UUID from the string standard representation as described in the <see cref="ToString()" /> method.
    /// </summary>
    /// <param name="input">A string that specifies a UUID.</param>
    /// <returns>A UUID with the specified value.</returns>
    /// <exception cref="ArgumentNullException">input is null.</exception>
    /// <exception cref="FormatException">input is not in a recognized format.</exception>
    public static Uuid Parse(string input)
    {
        return Guid.Parse(input);
    }

    private static string GetDigits(long val, int digits)
    {
        var hi = 1L << (digits * 4);
        return $"{hi | (val & (hi - 1)):X}".Substring(1);
    }
}
