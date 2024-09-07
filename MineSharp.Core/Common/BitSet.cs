/*
This file is a port of the BitSet class from Java 7.
The original Java code is available at: https://github.com/openjdk-mirror/jdk7u-jdk/blob/f4d80957e89a19a29bb9f9807d2a28351ed7f7df/src/share/classes/java/util/BitSet.java
The port was made with assistance from GitHub Copilot Chat.
*/

using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace MineSharp.Core.Common;

// TODO: Unit tests would be great to see whether the ported code works as expected.

/// <summary>
/// This class implements a vector of bits that grows as needed. Each
/// component of the bit set has a <see cref="bool"/> value. The
/// bits of a <see cref="BitSet"/> are indexed by nonnegative integers.
/// Individual indexed bits can be examined, set, or cleared. One
/// <see cref="BitSet"/> may be used to modify the contents of another
/// <see cref="BitSet"/> through logical AND, logical inclusive OR, and
/// logical exclusive OR operations.
/// </summary>
/// <remarks>
/// <para>By default, all bits in the set initially have the value
/// <c>false</c>.</para>
/// <para>Every bit set has a current size, which is the number of bits
/// of space currently in use by the bit set. Note that the size is
/// related to the implementation of a bit set, so it may change with
/// implementation. The length of a bit set relates to logical length
/// of a bit set and is defined independently of implementation.</para>
/// <para>Unless otherwise noted, passing a null parameter to any of the
/// methods in a <see cref="BitSet"/> will result in an
/// <c>ArgumentNullException</c>.</para>
/// 
/// <para>A <see cref="BitSet"/> is not safe for multithreaded use without
/// external synchronization.</para>
/// </remarks>
public sealed class BitSet : IEquatable<BitSet>, ICloneable
{
    /// <summary>
    /// BitSets are packed into arrays of "words."  Currently a word is
    /// a long, which consists of 64 bits, requiring 6 address bits.
    /// The choice of word size is determined purely by performance concerns.
    /// </summary>
    private const int ADDRESS_BITS_PER_WORD = 6;
    private const int BITS_PER_WORD = 1 << ADDRESS_BITS_PER_WORD;
    private const int BIT_INDEX_MASK = BITS_PER_WORD - 1;
    /// <summary>
    /// Used to shift left or right for a partial word mask.
    /// </summary>
    private const ulong WORD_MASK = 0xffffffffffffffffUL;

    /// <summary>
    /// The internal field corresponding to the serialField "bits".
    /// </summary>
    private ulong[] words;
    /// <summary>
    /// The number of words in the logical size of this BitSet.
    /// </summary>
    private int wordsInUse = 0;
    /// <summary>
    /// Whether the size of "words" is user-specified.  If so, we assume
    /// the user knows what he's doing and try harder to preserve it.
    /// </summary>
    private bool sizeIsSticky = false;


    /// <summary>
    /// Given a bit index, return word index containing it.
    /// </summary>
    private static int WordIndex(int bitIndex) => bitIndex >> ADDRESS_BITS_PER_WORD;

    /// <summary>
    /// Every public method must preserve these invariants.
    /// </summary>
    private void CheckInvariants()
    {
        // for performance reasons, we only check these in debug mode
#if DEBUG
        if (wordsInUse > 0)
        {
            if (words[wordsInUse - 1] == 0) throw new InvalidOperationException();
        }
        if (wordsInUse < 0 || wordsInUse > words.Length) throw new InvalidOperationException();
        if (wordsInUse != words.Length && words[wordsInUse] != 0) throw new InvalidOperationException();
#endif
    }

    /// <summary>
    /// Sets the field wordsInUse to the logical size in words of the bit set.
    /// WARNING: This method assumes that the number of words actually in use is
    /// less than or equal to the current value of wordsInUse!
    /// </summary>
    private void RecalculateWordsInUse()
    {
        var lastUsedIndex = words.AsSpan(0, wordsInUse).LastIndexOfAnyExcept(0ul);
        wordsInUse = lastUsedIndex + 1;
    }

    private void InitWords(int nbits)
    {
        words = new ulong[WordIndex(nbits - 1) + 1];
    }

    #region Constructors

#pragma warning disable CS8618 // words in initialized in InitWords
    /// <summary>
    /// Creates a new bit set. All bits are initially <c>false</c>.
    /// </summary>
    public BitSet()
    {
        InitWords(BITS_PER_WORD);
        sizeIsSticky = false;
    }

    /// <summary>
    /// Creates a bit set whose initial size is large enough to explicitly
    /// represent bits with indices in the range <c>0</c> through
    /// <c>nbits-1</c>. All bits are initially <c>false</c>.
    /// </summary>
    /// <param name="nbits">The initial size of the bit set.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the specified initial size is negative.</exception>
    public BitSet(int nbits)
    {
        if (nbits < 0) throw new ArgumentOutOfRangeException(nameof(nbits), "nbits < 0");
        InitWords(nbits);
        sizeIsSticky = true;
    }

    /// <summary>
    /// Creates a bit set using words as the internal representation.
    /// The last word (if there is one) must be non-zero.
    /// </summary>
    private BitSet(ulong[] words)
    {
        this.words = words;
        this.wordsInUse = words.Length;
        CheckInvariants();
    }
#pragma warning restore CS8618

    #endregion

    /// <summary>
    /// Ensures that the BitSet can hold enough words.
    /// </summary>
    /// <param name="wordsRequired">The minimum acceptable number of words.</param>
    private void EnsureCapacity(int wordsRequired)
    {
        if (words.Length < wordsRequired)
        {
            int request = Math.Max(2 * words.Length, wordsRequired);
            Array.Resize(ref words, request);
            sizeIsSticky = false;
        }
    }

    /// <summary>
    /// Ensures that the BitSet can accommodate a given wordIndex,
    /// temporarily violating the invariants. The caller must
    /// restore the invariants before returning to the user,
    /// possibly using <see cref="RecalculateWordsInUse"/>.
    /// </summary>
    /// <param name="wordIndex">The index to be accommodated.</param>
    private void ExpandTo(int wordIndex)
    {
        int wordsRequired = wordIndex + 1;
        if (wordsInUse < wordsRequired)
        {
            EnsureCapacity(wordsRequired);
            wordsInUse = wordsRequired;
        }
    }

    /// <summary>
    /// Checks that fromIndex ... toIndex is a valid range of bit indices.
    /// </summary>
    private static void CheckRange(int fromIndex, int toIndex)
    {
        if (fromIndex < 0) throw new ArgumentOutOfRangeException(nameof(fromIndex), "fromIndex < 0");
        if (toIndex < 0) throw new ArgumentOutOfRangeException(nameof(toIndex), "toIndex < 0");
        if (fromIndex > toIndex) throw new ArgumentOutOfRangeException(nameof(fromIndex), $"fromIndex: {fromIndex} > toIndex: {toIndex}");
    }

    /// <summary>
    /// Sets all of the bits in this BitSet to <c>false</c>.
    /// </summary>
    public void Clear()
    {
        words.AsSpan(0, wordsInUse).Clear();
    }

    #region Single Bit Operations

    /// <summary>
    /// Sets the bit at the specified index to the complement of its
    /// current value.
    /// </summary>
    /// <param name="bitIndex">The index of the bit to flip.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the specified index is negative.</exception>
    public void Flip(int bitIndex)
    {
        if (bitIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(bitIndex), "bitIndex < 0");

        int wordIndex = WordIndex(bitIndex);
        ExpandTo(wordIndex);

        words[wordIndex] ^= (1UL << bitIndex);

        RecalculateWordsInUse();
        CheckInvariants();
    }

    /// <summary>
    /// Sets the bit at the specified index to <c>true</c>.
    /// </summary>
    /// <param name="bitIndex">A bit index.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the specified index is negative.</exception>
    public void Set(int bitIndex)
    {
        if (bitIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(bitIndex), "bitIndex < 0");

        int wordIndex = WordIndex(bitIndex);
        ExpandTo(wordIndex);

        words[wordIndex] |= (1UL << bitIndex); // Restores invariants

        CheckInvariants();
    }

    /// <summary>
    /// Sets the bit at the specified index to the specified value.
    /// </summary>
    /// <param name="bitIndex">A bit index.</param>
    /// <param name="value">A boolean value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the specified index is negative.</exception>
    public void Set(int bitIndex, bool value)
    {
        if (value)
            Set(bitIndex);
        else
            Clear(bitIndex);
    }

    /// <summary>
    /// Sets the bit specified by the index to <c>false</c>.
    /// </summary>
    /// <param name="bitIndex">The index of the bit to be cleared.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the specified index is negative.</exception>
    public void Clear(int bitIndex)
    {
        if (bitIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(bitIndex), "bitIndex < 0");

        int wordIndex = WordIndex(bitIndex);
        if (wordIndex >= wordsInUse)
            return;

        words[wordIndex] &= ~(1UL << bitIndex);

        RecalculateWordsInUse();
        CheckInvariants();
    }

    /// <summary>
    /// Returns the index of the first bit that is set to <c>true</c>
    /// that occurs on or after the specified starting index. If no such
    /// bit exists then <c>-1</c> is returned.
    /// </summary>
    /// <param name="fromIndex">The index to start checking from (inclusive).</param>
    /// <returns>The index of the next set bit, or <c>-1</c> if there is no such bit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the specified index is negative.</exception>
    public int NextSetBit(int fromIndex)
    {
        if (fromIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(fromIndex), "fromIndex < 0");

        CheckInvariants();

        int u = WordIndex(fromIndex);
        if (u >= wordsInUse)
            return -1;

        ulong word = words[u] & (WORD_MASK << fromIndex);


        // TODO: Use Span
        while (true)
        {
            if (word != 0)
                return (u * BITS_PER_WORD) + BitOperations.TrailingZeroCount(word);
            if (++u == wordsInUse)
                return -1;
            word = words[u];
        }
    }

    /// <summary>
    /// Returns the index of the first bit that is set to <c>false</c>
    /// that occurs on or after the specified starting index.
    /// </summary>
    /// <param name="fromIndex">The index to start checking from (inclusive).</param>
    /// <returns>The index of the next clear bit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the specified index is negative.</exception>
    public int NextClearBit(int fromIndex)
    {
        if (fromIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(fromIndex), "fromIndex < 0");

        CheckInvariants();

        int u = WordIndex(fromIndex);
        if (u >= wordsInUse)
            return fromIndex;

        ulong word = ~words[u] & (WORD_MASK << fromIndex);

        while (true)
        {
            if (word != 0)
                return (u * BITS_PER_WORD) + BitOperations.TrailingZeroCount(word);
            if (++u == wordsInUse)
                return wordsInUse * BITS_PER_WORD;
            word = ~words[u];
        }
    }

    /// <summary>
    /// Returns the index of the nearest bit that is set to <c>true</c>
    /// that occurs on or before the specified starting index.
    /// If no such bit exists, or if <c>-1</c> is given as the
    /// starting index, then <c>-1</c> is returned.
    /// </summary>
    /// <param name="fromIndex">The index to start checking from (inclusive).</param>
    /// <returns>The index of the previous set bit, or <c>-1</c> if there is no such bit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the specified index is less than <c>-1</c>.</exception>
    public int PreviousSetBit(int fromIndex)
    {
        if (fromIndex < 0)
        {
            if (fromIndex == -1)
                return -1;
            throw new ArgumentOutOfRangeException(nameof(fromIndex), "fromIndex < -1");
        }

        CheckInvariants();

        int u = WordIndex(fromIndex);
        if (u >= wordsInUse)
            return Length() - 1;

        ulong word = words[u] & (WORD_MASK >> -(fromIndex + 1));

        while (true)
        {
            if (word != 0)
                return (u + 1) * BITS_PER_WORD - 1 - BitOperations.LeadingZeroCount(word);
            if (u-- == 0)
                return -1;
            word = words[u];
        }
    }

    /// <summary>
    /// Returns the index of the nearest bit that is set to <c>false</c>
    /// that occurs on or before the specified starting index.
    /// If no such bit exists, or if <c>-1</c> is given as the
    /// starting index, then <c>-1</c> is returned.
    /// </summary>
    /// <param name="fromIndex">The index to start checking from (inclusive).</param>
    /// <returns>The index of the previous clear bit, or <c>-1</c> if there is no such bit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the specified index is less than <c>-1</c>.</exception>
    public int PreviousClearBit(int fromIndex)
    {
        if (fromIndex < 0)
        {
            if (fromIndex == -1)
                return -1;
            throw new ArgumentOutOfRangeException(nameof(fromIndex), "fromIndex < -1");
        }

        CheckInvariants();

        int u = WordIndex(fromIndex);
        if (u >= wordsInUse)
            return fromIndex;

        ulong word = ~words[u] & (WORD_MASK >> -(fromIndex + 1));

        while (true)
        {
            if (word != 0)
                return (u + 1) * BITS_PER_WORD - 1 - BitOperations.LeadingZeroCount(word);
            if (u-- == 0)
                return -1;
            word = ~words[u];
        }
    }

    /// <summary>
    /// Returns the value of the bit with the specified index. The value
    /// is <c>true</c> if the bit with the index <c>bitIndex</c>
    /// is currently set in this <c>BitSet</c>; otherwise, the result
    /// is <c>false</c>.
    /// </summary>
    /// <param name="bitIndex">The bit index.</param>
    /// <returns>The value of the bit with the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the specified index is negative.</exception>
    public bool Get(int bitIndex)
    {
        if (bitIndex < 0) throw new ArgumentOutOfRangeException(nameof(bitIndex), "bitIndex < 0");

        CheckInvariants();

        int wordIndex = WordIndex(bitIndex);
        return (wordIndex < wordsInUse) && ((words[wordIndex] & (1UL << bitIndex)) != 0);
    }

    #endregion

    #region BitSet Operations


    /// <summary>
    /// Returns a new <see cref="BitSet"/> composed of bits from this <see cref="BitSet"/>
    /// from <paramref name="fromIndex"/> (inclusive) to <paramref name="toIndex"/> (exclusive).
    /// </summary>
    /// <param name="fromIndex">Index of the first bit to include.</param>
    /// <param name="toIndex">Index after the last bit to include.</param>
    /// <returns>A new <see cref="BitSet"/> from a range of this <see cref="BitSet"/>.</returns>
    /// <exception cref="IndexOutOfRangeException">If <paramref name="fromIndex"/> is negative,
    /// or <paramref name="toIndex"/> is negative, or <paramref name="fromIndex"/> is
    /// larger than <paramref name="toIndex"/>.</exception>
    public BitSet Get(int fromIndex, int toIndex)
    {
        CheckRange(fromIndex, toIndex);
        CheckInvariants();

        int len = Length();

        // If no set bits in range return empty bitset
        if (len <= fromIndex || fromIndex == toIndex)
            return new BitSet(0);

        // An optimization
        if (toIndex > len)
            toIndex = len;

        BitSet result = new BitSet(toIndex - fromIndex);
        int targetWords = WordIndex(toIndex - fromIndex - 1) + 1;
        int sourceIndex = WordIndex(fromIndex);
        bool wordAligned = ((fromIndex & BIT_INDEX_MASK) == 0);

        // Process all words but the last word
        for (int i = 0; i < targetWords - 1; i++, sourceIndex++)
            result.words[i] = wordAligned ? words[sourceIndex] :
                (words[sourceIndex] >>> fromIndex) |
                (words[sourceIndex + 1] << -fromIndex);

        // Process the last word
        ulong lastWordMask = WORD_MASK >>> -toIndex;
        result.words[targetWords - 1] =
            ((toIndex - 1) & BIT_INDEX_MASK) < (fromIndex & BIT_INDEX_MASK)
            ? /* straddles source words */
            ((words[sourceIndex] >>> fromIndex) |
             (words[sourceIndex + 1] & lastWordMask) << -fromIndex)
            :
            ((words[sourceIndex] & lastWordMask) >>> fromIndex);

        // Set wordsInUse correctly
        result.wordsInUse = targetWords;
        result.RecalculateWordsInUse();
        result.CheckInvariants();

        return result;
    }

    /// <summary>
    /// Sets the bits from the specified <paramref name="fromIndex"/> (inclusive) to the
    /// specified <paramref name="toIndex"/> (exclusive) to <c>true</c>.
    /// </summary>
    /// <param name="fromIndex">Index of the first bit to be set.</param>
    /// <param name="toIndex">Index after the last bit to be set.</param>
    /// <exception cref="IndexOutOfRangeException">If <paramref name="fromIndex"/> is negative,
    /// or <paramref name="toIndex"/> is negative, or <paramref name="fromIndex"/> is
    /// larger than <paramref name="toIndex"/>.</exception>
    public void Set(int fromIndex, int toIndex)
    {
        CheckRange(fromIndex, toIndex);

        if (fromIndex == toIndex)
            return;

        // Increase capacity if necessary
        int startWordIndex = WordIndex(fromIndex);
        int endWordIndex = WordIndex(toIndex - 1);
        ExpandTo(endWordIndex);

        ulong firstWordMask = WORD_MASK << fromIndex;
        ulong lastWordMask = WORD_MASK >>> -toIndex;
        if (startWordIndex == endWordIndex)
        {
            // Case 1: One word
            words[startWordIndex] |= (firstWordMask & lastWordMask);
        }
        else
        {
            // Case 2: Multiple words
            // Handle first word
            words[startWordIndex] |= firstWordMask;

            // Handle intermediate words, if any
            for (int i = startWordIndex + 1; i < endWordIndex; i++)
                words[i] = WORD_MASK;

            // Handle last word (restores invariants)
            words[endWordIndex] |= lastWordMask;
        }

        CheckInvariants();
    }

    /// <summary>
    /// Sets the bits from the specified <paramref name="fromIndex"/> (inclusive) to the
    /// specified <paramref name="toIndex"/> (exclusive) to the specified value.
    /// </summary>
    /// <param name="fromIndex">Index of the first bit to be set.</param>
    /// <param name="toIndex">Index after the last bit to be set.</param>
    /// <param name="value">Value to set the selected bits to.</param>
    /// <exception cref="IndexOutOfRangeException">If <paramref name="fromIndex"/> is negative,
    /// or <paramref name="toIndex"/> is negative, or <paramref name="fromIndex"/> is
    /// larger than <paramref name="toIndex"/>.</exception>
    public void Set(int fromIndex, int toIndex, bool value)
    {
        if (value)
            Set(fromIndex, toIndex);
        else
            Clear(fromIndex, toIndex);
    }

    /// <summary>
    /// Sets the bits from the specified <paramref name="fromIndex"/> (inclusive) to the
    /// specified <paramref name="toIndex"/> (exclusive) to <c>false</c>.
    /// </summary>
    /// <param name="fromIndex">Index of the first bit to be cleared.</param>
    /// <param name="toIndex">Index after the last bit to be cleared.</param>
    /// <exception cref="IndexOutOfRangeException">If <paramref name="fromIndex"/> is negative,
    /// or <paramref name="toIndex"/> is negative, or <paramref name="fromIndex"/> is
    /// larger than <paramref name="toIndex"/>.</exception>
    public void Clear(int fromIndex, int toIndex)
    {
        CheckRange(fromIndex, toIndex);

        if (fromIndex == toIndex)
            return;

        int startWordIndex = WordIndex(fromIndex);
        if (startWordIndex >= wordsInUse)
            return;

        int endWordIndex = WordIndex(toIndex - 1);
        if (endWordIndex >= wordsInUse)
        {
            toIndex = Length();
            endWordIndex = wordsInUse - 1;
        }

        ulong firstWordMask = WORD_MASK << fromIndex;
        ulong lastWordMask = WORD_MASK >>> -toIndex;
        if (startWordIndex == endWordIndex)
        {
            // Case 1: One word
            words[startWordIndex] &= ~(firstWordMask & lastWordMask);
        }
        else
        {
            // Case 2: Multiple words
            // Handle first word
            words[startWordIndex] &= ~firstWordMask;

            // Handle intermediate words, if any
            for (int i = startWordIndex + 1; i < endWordIndex; i++)
                words[i] = 0;

            // Handle last word
            words[endWordIndex] &= ~lastWordMask;
        }

        RecalculateWordsInUse();
        CheckInvariants();
    }

    /// <summary>
    /// Sets each bit from the specified <paramref name="fromIndex"/> (inclusive) to the
    /// specified <paramref name="toIndex"/> (exclusive) to the complement of its current
    /// value.
    /// </summary>
    /// <param name="fromIndex">Index of the first bit to flip.</param>
    /// <param name="toIndex">Index after the last bit to flip.</param>
    /// <exception cref="IndexOutOfRangeException">If <paramref name="fromIndex"/> is negative,
    /// or <paramref name="toIndex"/> is negative, or <paramref name="fromIndex"/> is
    /// larger than <paramref name="toIndex"/>.</exception>
    public void Flip(int fromIndex, int toIndex)
    {
        CheckRange(fromIndex, toIndex);

        if (fromIndex == toIndex)
            return;

        int startWordIndex = WordIndex(fromIndex);
        int endWordIndex = WordIndex(toIndex - 1);
        ExpandTo(endWordIndex);

        ulong firstWordMask = WORD_MASK << fromIndex;
        ulong lastWordMask = WORD_MASK >>> -toIndex;
        if (startWordIndex == endWordIndex)
        {
            // Case 1: One word
            words[startWordIndex] ^= (firstWordMask & lastWordMask);
        }
        else
        {
            // Case 2: Multiple words
            // Handle first word
            words[startWordIndex] ^= firstWordMask;

            // Handle intermediate words, if any
            for (int i = startWordIndex + 1; i < endWordIndex; i++)
                words[i] ^= WORD_MASK;

            // Handle last word
            words[endWordIndex] ^= lastWordMask;
        }

        RecalculateWordsInUse();
        CheckInvariants();
    }

    #endregion

    #region Serialization

    /// <summary>
    /// Returns a new bit set containing all the bits in the given ulong array.
    /// </summary>
    /// <param name="longs">A ulong array containing a little-endian representation
    /// of a sequence of bits to be used as the initial bits of the new bit set.</param>
    /// <returns>A new <see cref="BitSet"/> containing all the bits in the given ulong array.</returns>
    public static BitSet Create(ReadOnlySpan<ulong> longs)
    {
        if (longs.IsEmpty)
            return new BitSet();

        var lastUsedIndex = longs.LastIndexOfAnyExcept(0ul);
        var newWords = longs.Slice(0, lastUsedIndex + 1).ToArray();
        return new BitSet(newWords);
    }

    /// <summary>
    /// Returns a new bit set containing all the bits in the given byte array.
    /// </summary>
    /// <param name="bytes">A byte array containing a little-endian representation
    /// of a sequence of bits to be used as the initial bits of the new bit set.</param>
    /// <returns>A new <see cref="BitSet"/> containing all the bits in the given byte array.</returns>
    public static BitSet Create(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length % 8 != 0)
            throw new ArgumentException("byte array length must be a multiple of 8", nameof(bytes));
        return Create(MemoryMarshal.Cast<byte, ulong>(bytes));
    }

    /// <summary>
    /// Returns a new byte array containing all the bits in this bit set.
    /// </summary>
    /// <returns>A byte array containing a little-endian representation
    /// of all the bits in this bit set.</returns>
    public byte[] ToByteArray()
    {
        var bytes = new byte[wordsInUse * 8];
        WriteTo(MemoryMarshal.Cast<byte, ulong>(bytes));
        return bytes;
    }

    /// <summary>
    /// Returns a new long array containing all the bits in this bit set.
    /// </summary>
    /// <returns>A long array containing a little-endian representation
    /// of all the bits in this bit set.</returns>
    public ulong[] ToLongArray()
    {
        var newWords = words.AsSpan(0, wordsInUse).ToArray();
        return newWords;
    }

    /// <summary>
    /// Writes the bits in this bit set to the specified span of ulongs.
    /// </summary>
    /// <param name="longs">A span of ulongs to write the bits to.</param>
    /// <exception cref="ArgumentException">Thrown if the length of the span is less than the number of words in use.</exception>
    public void WriteTo(Span<ulong> longs)
    {
        if (longs.Length < wordsInUse)
            throw new ArgumentException("byte array length is too small", nameof(longs));

        words.AsSpan(0, wordsInUse).CopyTo(longs);
    }   

    #endregion

    #region Logical BitSet Operations

    /// <summary>
    /// Returns true if the specified <see cref="BitSet"/> has any bits set to
    /// <c>true</c> that are also set to <c>true</c> in this <see cref="BitSet"/>.
    /// </summary>
    /// <param name="set"><see cref="BitSet"/> to intersect with.</param>
    /// <returns>Boolean indicating whether this <see cref="BitSet"/> intersects
    /// the specified <see cref="BitSet"/>.</returns>
    public bool Intersects(BitSet set)
    {
        // TODO: Optimize using Vector<T>
        for (int i = Math.Min(wordsInUse, set.wordsInUse) - 1; i >= 0; i--)
            if ((words[i] & set.words[i]) != 0)
                return true;
        return false;
    }

    /// <summary>
    /// Returns the number of bits set to <c>true</c> in this <see cref="BitSet"/>.
    /// </summary>
    /// <returns>The number of bits set to <c>true</c> in this <see cref="BitSet"/>.</returns>
    public int Cardinality()
    {
        // TODO: Optimize using Vector<T>
        int sum = 0;
        for (int i = 0; i < wordsInUse; i++)
            sum += BitOperations.PopCount(words[i]);
        return sum;
    }

    /// <summary>
    /// Performs a logical <b>AND</b> of this target bit set with the
    /// argument bit set. This bit set is modified so that each bit in it
    /// has the value <c>true</c> if and only if it both initially
    /// had the value <c>true</c> and the corresponding bit in the
    /// bit set argument also had the value <c>true</c>.
    /// </summary>
    /// <param name="set">A bit set.</param>
    public void And(BitSet set)
    {
        if (this == set)
            return;

        words.AsSpan(set.wordsInUse, wordsInUse - set.wordsInUse).Clear();

        // TODO: Optimize using Vector<T>
        // Perform logical AND on words in common
        for (int i = 0; i < wordsInUse; i++)
            words[i] &= set.words[i];

        RecalculateWordsInUse();
        CheckInvariants();
    }

    /// <summary>
    /// Performs a logical <b>OR</b> of this bit set with the bit set
    /// argument. This bit set is modified so that a bit in it has the
    /// value <c>true</c> if and only if it either already had the
    /// value <c>true</c> or the corresponding bit in the bit set
    /// argument has the value <c>true</c>.
    /// </summary>
    /// <param name="set">A bit set.</param>
    public void Or(BitSet set)
    {
        if (this == set)
            return;

        int wordsInCommon = Math.Min(wordsInUse, set.wordsInUse);

        if (wordsInUse < set.wordsInUse)
        {
            EnsureCapacity(set.wordsInUse);
            wordsInUse = set.wordsInUse;
        }

        // TODO: Optimize using Vector<T>
        // Perform logical OR on words in common
        for (int i = 0; i < wordsInCommon; i++)
            words[i] |= set.words[i];

        // Copy any remaining words
        if (wordsInCommon < set.wordsInUse)
            Array.Copy(set.words, wordsInCommon, words, wordsInCommon, set.wordsInUse - wordsInCommon);

        // RecalculateWordsInUse() is unnecessary
        CheckInvariants();
    }

    /// <summary>
    /// Performs a logical <b>XOR</b> of this bit set with the bit set
    /// argument. This bit set is modified so that a bit in it has the
    /// value <c>true</c> if and only if one of the following
    /// statements holds:
    /// <list type="bullet">
    /// <item><description>The bit initially has the value <c>true</c>, and the
    /// corresponding bit in the argument has the value <c>false</c>.</description></item>
    /// <item><description>The bit initially has the value <c>false</c>, and the
    /// corresponding bit in the argument has the value <c>true</c>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="set">A bit set.</param>
    public void Xor(BitSet set)
    {
        int wordsInCommon = Math.Min(wordsInUse, set.wordsInUse);

        if (wordsInUse < set.wordsInUse)
        {
            EnsureCapacity(set.wordsInUse);
            wordsInUse = set.wordsInUse;
        }

        // TODO: Optimize using Vector<T>
        // Perform logical XOR on words in common
        for (int i = 0; i < wordsInCommon; i++)
            words[i] ^= set.words[i];

        // Copy any remaining words
        if (wordsInCommon < set.wordsInUse)
            Array.Copy(set.words, wordsInCommon, words, wordsInCommon, set.wordsInUse - wordsInCommon);

        RecalculateWordsInUse();
        CheckInvariants();
    }

    /// <summary>
    /// Clears all of the bits in this <see cref="BitSet"/> whose corresponding
    /// bit is set in the specified <see cref="BitSet"/>.
    /// </summary>
    /// <param name="set">The <see cref="BitSet"/> with which to mask this
    /// <see cref="BitSet"/>.</param>
    public void AndNot(BitSet set)
    {
        // TODO: Optimize using Vector<T>
        // Perform logical (a & !b) on words in common
        for (int i = Math.Min(wordsInUse, set.wordsInUse) - 1; i >= 0; i--)
            words[i] &= ~set.words[i];

        RecalculateWordsInUse();
        CheckInvariants();
    }

    #endregion

    #region HashCode, Equals, Clone and ToString

    /// <summary>
    /// Returns the hash code value for this bit set. The hash code depends
    /// only on which bits are set within this <see cref="BitSet"/>.
    /// </summary>
    /// <returns>The hash code value for this bit set.</returns>
    public override int GetHashCode()
    {
        ulong h = 1234;
        // TODO: Optimize using Vector<T>
        for (int i = wordsInUse; --i >= 0;)
            h ^= words[i] * (ulong)(i + 1);

        return (int)((h >> 32) ^ h);
    }

    /// <summary>
    /// <see cref="Equals(object?)"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(BitSet? other)
    {
        if (other == null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        CheckInvariants();
        other.CheckInvariants();

        if (wordsInUse != other.wordsInUse)
            return false;

        // Check words in use by both BitSets
        return words.AsSpan(0, wordsInUse).SequenceEqual(other.words.AsSpan(0, wordsInUse));
    }

    /// <summary>
    /// Compares this object against the specified object.
    /// The result is <c>true</c> if and only if the argument is
    /// not <c>null</c> and is a <see cref="BitSet"/> object that has
    /// exactly the same set of bits set to <c>true</c> as this bit
    /// set.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the objects are the same; <c>false</c> otherwise.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not BitSet other)
            return false;

        return Equals(other);
    }

    /// <summary>
    /// Cloning this <see cref="BitSet"/> produces a new <see cref="BitSet"/>
    /// that is equal to it.
    /// The clone of the bit set is another bit set that has exactly the
    /// same bits set to <c>true</c> as this bit set.
    /// </summary>
    /// <returns>A clone of this bit set.</returns>
    public BitSet CloneTyped()
    {
        if (!sizeIsSticky)
            TrimToSize();

        try
        {
            BitSet result = (BitSet)MemberwiseClone();
            result.words = (ulong[])words.Clone();
            result.CheckInvariants();
            return result;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Clone not supported", e);
        }
    }

    /// <summary>
    /// <see cref="CloneTyped"/>
    /// </summary>
    public object Clone()
    {
        return CloneTyped();
    }

    /// <summary>
    /// Returns a string representation of this bit set. For every index
    /// for which this <see cref="BitSet"/> contains a bit in the set
    /// state, the decimal representation of that index is included in
    /// the result. Such indices are listed in order from lowest to
    /// highest, separated by ", " (a comma and a space) and
    /// surrounded by braces, resulting in the usual mathematical
    /// notation for a set of integers.
    /// </summary>
    /// <returns>A string representation of this bit set.</returns>
    public override string ToString()
    {
        CheckInvariants();

        int numBits = (wordsInUse > 128) ? Cardinality() : wordsInUse * BITS_PER_WORD;
        StringBuilder b = new StringBuilder(6 * numBits + 2);
        b.Append('{');

        int i = NextSetBit(0);
        if (i != -1)
        {
            b.Append(i);
            for (i = NextSetBit(i + 1); i >= 0; i = NextSetBit(i + 1))
            {
                int endOfRun = NextClearBit(i);
                do { b.Append(", ").Append(i); }
                while (++i < endOfRun);
            }
        }

        b.Append('}');
        return b.ToString();
    }

    #endregion

    /// <summary>
    /// Returns the number of bits of space actually in use by this
    /// <c>BitSet</c> to represent bit values.
    /// The maximum element in the set is the size - 1st element.
    /// </summary>
    /// <returns>The number of bits currently in this bit set.</returns>
    public int Size() => words.Length * BITS_PER_WORD;

    /// <summary>
    /// Attempts to reduce internal storage used for the bits in this bit set.
    /// Calling this method may, but is not required to, affect the value
    /// returned by a subsequent call to the <see cref="Size"/> method.
    /// </summary>
    private void TrimToSize()
    {
        if (wordsInUse != words.Length)
        {
            Array.Resize(ref words, wordsInUse);
            CheckInvariants();
        }
    }

    /// <summary>
    /// Returns the "logical size" of this <c>BitSet</c>: the index of
    /// the highest set bit in the <c>BitSet</c> plus one. Returns zero
    /// if the <c>BitSet</c> contains no set bits.
    /// </summary>
    /// <returns>The logical size of this <c>BitSet</c>.</returns>
    public int Length()
    {
        if (wordsInUse == 0) return 0;

        return BITS_PER_WORD * (wordsInUse - 1) + (BITS_PER_WORD - BitOperations.LeadingZeroCount(words[wordsInUse - 1]));
    }

    /// <summary>
    /// Returns true if this <c>BitSet</c> contains no bits that are set
    /// to <c>true</c>.
    /// </summary>
    /// <returns>Boolean indicating whether this <c>BitSet</c> is empty.</returns>
    public bool IsEmpty() => wordsInUse == 0;
}
