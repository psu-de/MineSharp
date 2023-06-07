namespace MineSharp.Core.Extensions;

public static class StreamExtensions
{
    public static int ReadVarInt(this Stream stream)
        => stream.ReadVarInt(out _);
    
    public static int ReadVarInt(this Stream stream, out int read)
    {
        var value = 0;
        var length = 0;
        byte currentByte;

        while (true)
        {
            currentByte = (byte)stream.ReadByte();
            value |= (currentByte & 0x7F) << length * 7;

            length++;
            if (length > 5) throw new Exception("VarInt too big");

            if ((currentByte & 0x80) != 0x80)
            {
                break;
            }
        }
        read = length;
        return value;
    }
    
    private static void WriteVarInt(Stream stream, int value)
    {
        while (true)
        {
            if ((value & ~0x7F) == 0)
            {
                stream.WriteByte((byte)value);
                return;
            }
            stream.WriteByte((byte)(value & 0x7F | 0x80));
            value >>= 7;
        }
    }

    public static short ReadShort(this Stream stream)
    {
        int b0 = stream.ReadByte();
        int b1 = stream.ReadByte();
        
        if (BitConverter.IsLittleEndian)
            return (short)(b0 << 8 | b1);

        return (short)(b0 | b1 << 8);
    }

    public static int ReadInt(this Stream stream)
    {
        int b0 = stream.ReadByte();
        int b1 = stream.ReadByte();
        int b2 = stream.ReadByte();
        int b3 = stream.ReadByte();
        
        if (BitConverter.IsLittleEndian)
            return b0 << 24 | b1 << 16 | b2 << 8 | b3;

        return b0 | b1 << 8 | b2 << 16 | b3 << 24;
    }

    public static long ReadLong(this Stream stream)
    {
        long b0 = stream.ReadByte();
        long b1 = stream.ReadByte();
        long b2 = stream.ReadByte();
        long b3 = stream.ReadByte();
        long b4 = stream.ReadByte();
        long b5 = stream.ReadByte();
        long b6 = stream.ReadByte();
        long b7 = stream.ReadByte();
        
        if (BitConverter.IsLittleEndian)
            return b0 << 56 | b1 << 48 | b2 << 40 | b3 << 32 | b4 << 24 | b5 << 16 | b6 << 8 | b7;

        return b0 | b1 << 8 | b2 << 16 | b3 << 24 | b4 << 32 | b5 << 40 | b6 << 48 | b7 << 56;
    }
}
