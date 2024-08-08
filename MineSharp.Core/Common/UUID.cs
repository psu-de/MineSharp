using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MineSharp.Core.Common;

// Thanks to MrZoidberg
// https://gist.github.com/MrZoidberg/9bac07cf3f5aa5896f75

/// <summary>
///     Represents an immutable Java universally unique identifier (UUID).
///     A UUID represents a 128-bit value.
/// </summary>
public readonly struct Uuid : IEquatable<Uuid>
{
    /// <summary>
    ///     Empty UUID
    /// </summary>
    public static readonly Uuid Empty = new();

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
    /// <param name="bytes">Bytes array that represents the UUID. Must be 16 bytes in big-endian order.</param>
    public Uuid(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 16)
        {
            throw new ArgumentException("Length of the UUID byte array should be 16");
        }

        Span<byte> byteSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref this, 1));
        if (BitConverter.IsLittleEndian)
        {
            bytes.CopyTo(byteSpan);
            byteSpan.Reverse();
        }
        else
        {
            // architecure is big-endian but our files are little-endian ordered
            // so we need to reverse the longs
            var longSpan = MemoryMarshal.Cast<byte, long>(byteSpan);
            longSpan.Reverse();
        }
        // since we do these operations in-place, we are done here
    }

    // The order of the most significant and least significant fields is important
    // when serializing and deserializing the UUID as unmanaged data types (little-endian)
    /// <summary>
    ///     The least significant 64 bits of this UUID's 128 bit value.
    /// </summary>
    public readonly long LeastSignificantBits;

    /// <summary>
    ///     The most significant 64 bits of this UUID's 128 bit value.
    /// </summary>
    public readonly long MostSignificantBits;

    /// <summary>
    ///     Returns a value that indicates whether this instance is equal to a specified
    ///     object.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns>true if o is a <paramref name="obj" /> that has the same value as this instance; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (!(obj is Uuid uuid))
        {
            return false;
        }

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
        return LeastSignificantBits == uuid.LeastSignificantBits && MostSignificantBits == uuid.MostSignificantBits;
    }

    /// <summary>
    ///     Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code for this instance.</returns>
    public override int GetHashCode()
    {
        // our hash code must not be compatible with Guid.GetHashCode so we do it ourselves
        return HashCode.Combine(LeastSignificantBits, MostSignificantBits);
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

    private ReadOnlySpan<byte> AsByteSpan()
    {
        return MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in this), 1));
    }

    /// <summary>
    /// Writes the UUID to the specified destination span in big-endian order.
    /// </summary>
    /// <param name="destination">The destination span to write the UUID to.</param>
    public void WriteTo(Span<byte> destination)
    {
        if (destination.Length != 16)
        {
            throw new ArgumentException("Destination span must be 16 bytes long");
        }

        var byteSpan = AsByteSpan();

        if (BitConverter.IsLittleEndian)
        {
            byteSpan.CopyTo(destination);
            destination.Reverse();
        }
        else
        {
            // architecture is big-endian but our fields are little-endian ordered
            // so we need to reverse the longs
            var longSpan = MemoryMarshal.Cast<byte, long>(destination);
            longSpan.Reverse();
        }
    }

    /// <summary>
    ///     Returns a 16-element byte array that contains the value of this instance in big-endian.
    /// </summary>
    /// <returns>A 16-element byte array</returns>
    public byte[] ToByteArray()
    {
        var destinationBytes = new byte[16];
        WriteTo(destinationBytes);
        return destinationBytes;
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

        var byteSpan = uuid.AsByteSpan();
        return new(byteSpan, true);
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

        Span<byte> byteSpan = stackalloc byte[16];
        value.TryWriteBytes(byteSpan, true, out _);
        return new(byteSpan);
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
