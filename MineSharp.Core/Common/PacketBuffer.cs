using System.Text;
using System.Text.RegularExpressions;
using fNbt;
using MineSharp.Core.Common.Blocks;

namespace MineSharp.Core.Common;

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
    /// Set the internal buffer position
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(int position)
    {
        buffer.Position = position;
    }

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

    public sbyte ReadSByte()
    {
        return (sbyte)ReadByte();
    }

    public bool ReadBool()
    {
        return ReadByte() == 1;
    }

    public ushort ReadUShort()
    {
        EnsureEnoughReadableBytes(2);

        var b0 = buffer.ReadByte();
        var b1 = buffer.ReadByte();

        if (BitConverter.IsLittleEndian)
        {
            return (ushort)((b0 << 8) | b1);
        }

        return (ushort)(b0 | (b1 << 8));
    }

    public short ReadShort()
    {
        return (short)ReadUShort();
    }

    public uint ReadUInt()
    {
        EnsureEnoughReadableBytes(4);

        var b0 = buffer.ReadByte();
        var b1 = buffer.ReadByte();
        var b2 = buffer.ReadByte();
        var b3 = buffer.ReadByte();

        if (BitConverter.IsLittleEndian)
        {
            return (uint)((b0 << 24) | (b1 << 16) | (b2 << 8) | b3);
        }

        return (uint)(b0 | (b1 << 8) | (b2 << 16) | (b3 << 24));
    }

    public int ReadInt()
    {
        return (int)ReadUInt();
    }

    public ulong ReadULong()
    {
        EnsureEnoughReadableBytes(8);

        long b0 = buffer.ReadByte();
        long b1 = buffer.ReadByte();
        long b2 = buffer.ReadByte();
        long b3 = buffer.ReadByte();
        long b4 = buffer.ReadByte();
        long b5 = buffer.ReadByte();
        long b6 = buffer.ReadByte();
        long b7 = buffer.ReadByte();

        if (BitConverter.IsLittleEndian)
        {
            return (ulong)((b0 << 56) | (b1 << 48) | (b2 << 40) | (b3 << 32) | (b4 << 24) | (b5 << 16) | (b6 << 8) |
                b7);
        }

        return (ulong)(b0 | (b1 << 8) | (b2 << 16) | (b3 << 24) | (b4 << 32) | (b5 << 40) | (b6 << 48) | (b7 << 56));
    }

    public long ReadLong()
    {
        return (long)ReadULong();
    }

    public float ReadFloat()
    {
        EnsureEnoughReadableBytes(4);

        Span<byte> bytes = stackalloc byte[sizeof(float)];
        ReadBytes(bytes);

        if (BitConverter.IsLittleEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToSingle(bytes);
    }

    public double ReadDouble()
    {
        EnsureEnoughReadableBytes(8);

        Span<byte> bytes = stackalloc byte[sizeof(double)];
        ReadBytes(bytes);

        if (BitConverter.IsLittleEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToDouble(bytes);
    }

    public int ReadVarInt()
    {
        var value = 0;
        var shift = 0;

        while (true)
        {
            var b = ReadByte();
            value |= (b & 0x7f) << shift;
            if ((b & 0x80) == 0x00)
            {
                break;
            }

            shift += 7;
            if (shift >= 32)
            {
                throw new("varint is too big");
            }
        }

        return value;
    }

    public long ReadVarLong()
    {
        long value = 0;
        var shift = 0;

        while (true)
        {
            var b = ReadByte();
            value |= (b & (long)0x7f) << shift;
            if ((b & 0x80) == 0x00)
            {
                break;
            }

            shift += 7;
            if (shift >= 64)
            {
                throw new("varlong is too big");
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

    public Uuid ReadUuid()
    {
        var l1 = ReadLong();
        var l2 = ReadLong();
        return new(l1, l2);
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
        var x = (byte)((packedXz >> 4) & 0xF);
        var z = (byte)(packedXz & 0xF);
        var y = ReadShort();
        var type = ReadVarInt();
        var nbt = ReadOptionalNbtCompound();

        return new(x, y, z, type, nbt);
    }

    public T? ReadOptional<T>() where T : class
    {
        var available = ReadBool();
        if (!available)
        {
            return null;
        }

        return Read<T>();
    }

    public T? ReadOptional<T>(bool _ = false) where T : struct
    {
        var available = ReadBool();
        if (!available)
        {
            return null;
        }

        return Read<T>();
    }


    public T Read<T>()
    {
        var type = Type.GetTypeCode(typeof(T));

        object value = type switch
        {
            TypeCode.Boolean => ReadBool(),
            TypeCode.SByte => ReadSByte(),
            TypeCode.Byte => ReadByte(),
            TypeCode.Int16 => ReadShort(),
            TypeCode.Int32 => ReadInt(),
            TypeCode.Int64 => ReadLong(),

            TypeCode.UInt16 => ReadUShort(),
            TypeCode.UInt32 => ReadUInt(),
            TypeCode.UInt64 => ReadULong(),

            TypeCode.String => ReadString(),
            TypeCode.Single => ReadFloat(),
            TypeCode.Double => ReadBool(),
            _ => throw new NotSupportedException()
        };
        return (T)value;
    }

    #endregion


    #region Writing

    public void WriteBytes(Span<byte> bytes)
    {
        buffer.Write(bytes);
    }

    public void WriteBool(bool value)
    {
        WriteByte((byte)(value ? 1 : 0));
    }

    public void WriteByte(byte b)
    {
        buffer.WriteByte(b);
    }

    public void WriteSByte(sbyte b)
    {
        WriteByte((byte)b);
    }

    public void WriteUShort(ushort value)
    {
        if (BitConverter.IsLittleEndian)
        {
            WriteByte((byte)((value >> 8) & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return;
        }

        WriteByte((byte)(value & 0xFF));
        WriteByte((byte)((value >> 8) & 0xFF));
    }

    public void WriteShort(short value)
    {
        WriteUShort((ushort)value);
    }

    public void WriteUInt(uint value)
    {
        if (BitConverter.IsLittleEndian)
        {
            WriteByte((byte)((value >> 24) & 0xFF));
            WriteByte((byte)((value >> 16) & 0xFF));
            WriteByte((byte)((value >> 8) & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return;
        }

        WriteByte((byte)(value & 0xFF));
        WriteByte((byte)((value >> 8) & 0xFF));
        WriteByte((byte)((value >> 16) & 0xFF));
        WriteByte((byte)((value >> 24) & 0xFF));
    }

    public void WriteInt(int value)
    {
        WriteUInt((uint)value);
    }

    public void WriteULong(ulong value)
    {
        if (BitConverter.IsLittleEndian)
        {
            WriteByte((byte)((value >> 56) & 0xFF));
            WriteByte((byte)((value >> 48) & 0xFF));
            WriteByte((byte)((value >> 40) & 0xFF));
            WriteByte((byte)((value >> 32) & 0xFF));
            WriteByte((byte)((value >> 24) & 0xFF));
            WriteByte((byte)((value >> 16) & 0xFF));
            WriteByte((byte)((value >> 8) & 0xFF));
            WriteByte((byte)(value & 0xFF));
            return;
        }

        WriteByte((byte)(value & 0xFF));
        WriteByte((byte)((value >> 8) & 0xFF));
        WriteByte((byte)((value >> 16) & 0xFF));
        WriteByte((byte)((value >> 24) & 0xFF));
        WriteByte((byte)((value >> 32) & 0xFF));
        WriteByte((byte)((value >> 40) & 0xFF));
        WriteByte((byte)((value >> 48) & 0xFF));
        WriteByte((byte)((value >> 56) & 0xFF));
    }

    public void WriteLong(long value)
    {
        WriteULong((ulong)value);
    }

    public void WriteFloat(float value)
    {
        var val = BitConverter.SingleToUInt32Bits(value);
        WriteUInt(val);
    }

    public void WriteDouble(double value)
    {
        var val = BitConverter.DoubleToUInt64Bits(value);
        WriteULong(val);
    }

    public void WriteVarInt(int value)
    {
        while (true)
        {
            if ((value & ~0x7F) == 0)
            {
                buffer.WriteByte((byte)value);
                return;
            }

            buffer.WriteByte((byte)((value & 0x7F) | 0x80));
            value >>>= 7;
        }
    }

    public void WriteVarLong(int value)
    {
        while ((value & ~0x7F) != 0x00)
        {
            buffer.WriteByte((byte)((value & 0xFF) | 0x80));
            value >>>= 7;
        }

        buffer.WriteByte((byte)value);
    }

    public void WriteString(string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        Span<byte> bytes = stackalloc byte[encoding.GetByteCount(value)];
        var length = encoding.GetBytes(value, bytes);

        WriteVarInt(length);
        WriteBytes(bytes);
    }

    public void WriteUuid(Uuid value)
    {
        WriteLong(value.MostSignificantBits);
        WriteLong(value.LeastSignificantBits);
    }

    public void WriteVarIntArray<T>(ICollection<T> collection, Action<PacketBuffer, T> writer)
    {
        WriteVarInt(collection.Count);

        foreach (var element in collection)
        {
            writer(this, element);
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
        var packedXz = (byte)(((entity.X << 4) & 0xF) | (entity.Z & 0xF));
        WriteByte(packedXz);
        WriteShort(entity.Y);
        WriteVarInt(entity.Type);
        WriteOptionalNbt(entity.Data);
    }

    #endregion

#pragma warning restore CS1591
}
