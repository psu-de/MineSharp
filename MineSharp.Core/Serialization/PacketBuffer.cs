using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using fNbt;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Exceptions;
using MineSharp.Core.Geometry;

namespace MineSharp.Core.Serialization;

/// <summary>
///     Read and write values from and to a byte buffer.
/// </summary>
public class PacketBuffer : IDisposable, IAsyncDisposable
{
    private readonly MemoryStream buffer;

    /// <summary>
    ///     Create a new empty, writeable PacketBuffer
    /// </summary>
    /// <param name="protocolVersion"></param>
    public PacketBuffer(int protocolVersion)
    {
        ProtocolVersion = protocolVersion;
        buffer = new();
    }

    /// <summary>
    ///     Create a new readable PacketBuffer with <paramref name="bytes" /> as input.
    /// </summary>
    /// <param name="protocolVersion"></param>
    /// <param name="bytes"></param>
    public PacketBuffer(byte[] bytes, int protocolVersion)
    {
        ProtocolVersion = protocolVersion;
        buffer = new(bytes);
    }

    /// <summary>
    ///     The total size of the buffer
    /// </summary>
    public long Size => buffer.Length;

    /// <summary>
    ///     The number of readable bytes in the buffer
    /// </summary>
    public long ReadableBytes => buffer.Length - buffer.Position;

    /// <summary>
    ///     The position in the buffer.
    /// </summary>
    public long Position => buffer.Position;

    /// <summary>
    ///     Whether to use anonymous nbt compounds. This is used since
    ///     Minecraft Java 1.20.2
    /// </summary>
    public bool UseAnonymousNbt => ProtocolVersion >= Core.ProtocolVersion.V_1_20_2;

    /// <summary>
    ///     The protocol version this packet buffer uses
    /// </summary>
    public int ProtocolVersion { get; }

    /// <summary>
    ///     Disposes the underlying <see cref="MemoryStream" />
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await buffer.DisposeAsync();
    }

    /// <summary>
    ///     Disposes the underlying <see cref="MemoryStream" />
    /// </summary>
    public void Dispose()
    {
        buffer.Dispose();
    }

    /// <summary>
    ///     Return the buffer's byte array
    /// </summary>
    /// <returns></returns>
    public byte[] GetBuffer()
    {
        return buffer.ToArray();
    }

    /// <summary>
    ///     Returns the buffer as hex values as a string
    /// </summary>
    /// <param name="cutToPosition"></param>
    /// <returns></returns>
    public string HexDump(bool cutToPosition = false)
    {
        var hex = Convert.ToHexString(GetBuffer().Skip(cutToPosition ? (int)Position : 0).ToArray());
        return Regex.Replace(hex, ".{2}", "$0 ").TrimEnd();
    }

    private void EnsureEnoughReadableBytes(int count)
    {
        if (ReadableBytes < count)
        {
            throw new EndOfStreamException();
        }
    }

#pragma warning disable CS1591

    #region Reading

    /// <summary>
    ///     Read an unmanaged value from the buffer.
    ///     This works great for signed and unsigned integer primitives.
    ///     For other value types or custom structs this will likely not work (on some systems).
    ///     Because this method just reverses the bytes if the system is little endian.
    /// </summary>
    /// <typeparam name="T">The unmanaged type to read.</typeparam>
    /// <returns></returns>
    public T Read<T>()
        where T : unmanaged
    {
        Unsafe.SkipInit(out T value);
        var typedSpan = MemoryMarshal.CreateSpan(ref value, 1);
        var byteSpan = MemoryMarshal.AsBytes(typedSpan);

        ReadBytes(byteSpan);

        if (BitConverter.IsLittleEndian)
        {
            byteSpan.Reverse();
        }

        return value;
    }

    public int ReadBytes(Span<byte> bytes)
    {
        EnsureEnoughReadableBytes(bytes.Length);

        return buffer.Read(bytes);
    }

    public byte[] ReadBytes(int count)
    {
        EnsureEnoughReadableBytes(count);

        var read = 0;
        var bytes = new byte[count];
        while (read < count)
        {
            read += buffer.Read(bytes, read, count - read);
        }

        return bytes;
    }

    public byte Peek()
    {
        EnsureEnoughReadableBytes(1);

        var peek = (byte)buffer.ReadByte();
        buffer.Position -= 1;
        return peek;
    }

    public byte ReadByte()
    {
        EnsureEnoughReadableBytes(1);

        return (byte)buffer.ReadByte();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByte()
    {
        return (sbyte)ReadByte();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool()
    {
        return ReadByte() == 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUShort()
    {
        return Read<ushort>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadShort()
    {
        return Read<short>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt()
    {
        return Read<uint>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt()
    {
        return Read<int>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadULong()
    {
        return Read<ulong>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadLong()
    {
        return Read<long>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat()
    {
        var bits = Read<uint>();
        return BitConverter.UInt32BitsToSingle(bits);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble()
    {
        var bits = Read<ulong>();
        return BitConverter.UInt64BitsToDouble(bits);
    }

    private const int VarIntSegmentBits = 0x7F;
    private const int VarIntContinueBit = 0x80;

    // This method is also used by MinecraftStream
    public static int ReadVarInt(Stream stream, out int byteCount)
    {
        var value = 0;
        var shift = 0;
        byteCount = 0;

        while (true)
        {
            var b = stream.ReadByte();
            if (b == -1)
            {
                throw new EndOfStreamException();
            }

            byteCount++;
            value |= (b & VarIntSegmentBits) << shift;
            if ((b & VarIntContinueBit) == 0)
            {
                break;
            }

            shift += 7;
            if (shift >= 32)
            {
                throw new SerializationException("VarInt is too big.");
            }
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadVarInt()
    {
        return ReadVarInt(buffer, out _);
    }

    public long ReadVarLong()
    {
        long value = 0;
        var shift = 0;

        while (true)
        {
            var b = ReadByte();
            value |= (long)(b & VarIntSegmentBits) << shift;
            if ((b & VarIntContinueBit) == 0)
            {
                break;
            }

            shift += 7;
            if (shift >= 64)
            {
                throw new SerializationException("VarLong is too big.");
            }
        }

        return value;
    }

    public string ReadString(Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        var length = ReadVarInt();
        Span<byte> bytes = stackalloc byte[length];
        ReadBytes(bytes);

        return encoding.GetString(bytes);
    }

    public Identifier ReadIdentifier()
    {
        var str = ReadString();
        // if the string is not a valid identifier, it will throw an exception
        return Identifier.Parse(str);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Uuid ReadUuid()
    {
        Span<byte> bytes = stackalloc byte[16];
        ReadBytes(bytes);
        return new(bytes);
    }

    public BitSet ReadBitSet()
    {
        var longs = ReadLongArray();
        return BitSet.Create(MemoryMarshal.Cast<long, ulong>(longs));
    }

    public T[] ReadVarIntArray<T>(Func<PacketBuffer, T> reader)
    {
        var array = new T[ReadVarInt()];

        for (var i = 0; i < array.Length; i++)
        {
            array[i] = reader(this);
        }

        return array;
    }

    public long[] ReadLongArray()
    {
        var length = ReadVarInt();
        var array = new long[length];

        for (var i = 0; i < array.Length; i++)
        {
            array[i] = Read<long>();
        }

        return array;
    }

    public byte[] RestBuffer()
    {
        var bytes = new byte[ReadableBytes];
        _ = buffer.Read(bytes, 0, bytes.Length);
        return bytes;
    }

    public NbtTag ReadNbt()
    {
        var file = new NbtFile { BigEndian = true, Anonymous = UseAnonymousNbt };
        file.LoadFromStream(buffer, NbtCompression.None);

        return file.RootTag;
    }

    public NbtCompound ReadNbtCompound()
    {
        return (NbtCompound)ReadNbt();
    }

    public NbtCompound? ReadOptionalNbtCompound()
    {
        return (NbtCompound?)ReadOptionalNbt();
    }

    public NbtTag? ReadOptionalNbt()
    {
        var t = (NbtTagType)ReadByte();
        if (t == NbtTagType.End)
        {
            return null;
        }

        buffer.Position--;

        return ReadNbt();
    }

    public BlockEntity ReadBlockEntity()
    {
        var packedXz = ReadByte();
        var x = (byte)(packedXz >> 4 & 0xF);
        var z = (byte)(packedXz & 0xF);
        var y = ReadShort();
        var type = ReadVarInt();
        var nbt = ReadOptionalNbtCompound();

        return new(x, y, z, type, nbt);
    }

    public Position ReadPosition()
    {
        return new Position(ReadULong());
    }

    public T? ReadOptional<T>()
        where T : class
    {
        var available = ReadBool();
        if (!available)
        {
            return null;
        }

        return ReadObject<T>();
    }

    public T? ReadOptional<T>(bool _ = false)
        where T : unmanaged
    {
        var available = ReadBool();
        if (!available)
        {
            return null;
        }

        return Read<T>();
    }

    public T ReadObject<T>()
        where T : class
    {
        var type = Type.GetTypeCode(typeof(T));

        object value = type switch
        {
            TypeCode.String => ReadString(),
            _ => throw new NotSupportedException()
        };
        return (T)value;
    }

    #endregion


    #region Writing

    /// <summary>
    ///     Writes an unmanaged value to the buffer.
    ///     This works great for signed and unsigned integer primitives.
    ///     For other value types or custom structs this will likely not work (on some systems).
    ///     Because this method just reverses the bytes if the system is little endian.
    /// </summary>
    /// <typeparam name="T">The unmanaged type to write.</typeparam>
    /// <returns></returns>
    public void Write<T>(T value)
        where T : unmanaged
    {
        var typedSpan = MemoryMarshal.CreateSpan(ref value, 1);
        var byteSpan = MemoryMarshal.AsBytes(typedSpan);

        if (BitConverter.IsLittleEndian)
        {
            byteSpan.Reverse();
        }

        WriteBytes(byteSpan);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        buffer.Write(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBool(bool value)
    {
        WriteByte((byte)(value ? 1 : 0));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte b)
    {
        buffer.WriteByte(b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSByte(sbyte b)
    {
        WriteByte((byte)b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUShort(ushort value)
    {
        Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteShort(short value)
    {
        Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt(uint value)
    {
        Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt(int value)
    {
        Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteULong(ulong value)
    {
        Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLong(long value)
    {
        Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFloat(float value)
    {
        var val = BitConverter.SingleToUInt32Bits(value);
        Write(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDouble(double value)
    {
        var val = BitConverter.DoubleToUInt64Bits(value);
        Write(val);
    }

    // Is also used by MinecraftStream
    public static void WriteVarInt(Stream stream, int value)
    {
        while (true)
        {
            if ((value & ~VarIntSegmentBits) == 0)
            {
                stream.WriteByte((byte)value);
                return;
            }

            stream.WriteByte((byte)(value & VarIntSegmentBits | VarIntContinueBit));
            value >>>= 7;
        }
    }

    public void WriteVarInt(int value)
    {
        WriteVarInt(buffer, value);
    }

    public void WriteVarLong(long value)
    {
        while (true)
        {
            if ((value & ~VarIntSegmentBits) == 0)
            {
                WriteByte((byte)value);
                return;
            }

            WriteByte((byte)(value & VarIntSegmentBits | VarIntContinueBit));
            value >>>= 7;
        }
    }

    public void WriteString(string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        Span<byte> bytes = stackalloc byte[encoding.GetByteCount(value)];
        var length = encoding.GetBytes(value, bytes);

        WriteVarInt(length);
        WriteBytes(bytes);
    }

    public void WriteIdentifier(Identifier identifier)
    {
        var str = identifier.ToString();
        WriteString(str);
    }

    public void WriteUuid(Uuid value)
    {
        Span<byte> bytes = stackalloc byte[16];
        value.WriteTo(bytes);
        WriteBytes(bytes);
    }

    public void WriteBitSet(BitSet bitSet)
    {
        var longs = bitSet.ToLongArray();
        WriteLongArray(MemoryMarshal.Cast<ulong, long>(longs));
    }

    public void WriteVarIntArray<T>(ICollection<T> collection, Action<PacketBuffer, T> writer)
    {
        WriteVarInt(collection.Count);

        foreach (var element in collection)
        {
            writer(this, element);
        }
    }

    public void WriteLongArray(ReadOnlySpan<long> array)
    {
        WriteVarInt(array.Length);
        foreach (var l in array)
        {
            WriteLong(l);
        }
    }

    public void WriteNbt(NbtTag tag)
    {
        var f = new NbtFile(tag) { BigEndian = true, Anonymous = UseAnonymousNbt };
        f.SaveToStream(buffer, NbtCompression.None);
    }

    public void WriteOptionalNbt(NbtTag? tag)
    {
        if (tag is null or NbtCompound { Count: 0 })
        {
            buffer.WriteByte((byte)NbtTagType.End);
            return;
        }

        WriteNbt(tag);
    }

    public void WriteBlockEntity(BlockEntity entity)
    {
        var packedXz = (byte)(entity.X << 4 & 0xF | entity.Z & 0xF);
        WriteByte(packedXz);
        WriteShort(entity.Y);
        WriteVarInt(entity.Type);
        WriteOptionalNbt(entity.Data);
    }

    public void WritePosition(Position position)
    {
        WriteULong(position.ToULong());
    }

    #endregion

#pragma warning restore CS1591
}
