using System.Net.Sockets;
using ICSharpCode.SharpZipLib.Zip.Compression;
using MineSharp.Core.Common;
using MineSharp.Protocol.Cryptography;
using NLog;

namespace MineSharp.Protocol;

/// <summary>
/// Handles reading and writing packets.
/// Also handles encryption and compression.
/// This class is not thread-safe.
/// </summary>
internal class MinecraftStream
{
    private const int CompressionDisabled = -1;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly Deflater deflater = new();

    private readonly Inflater inflater = new();
    private readonly NetworkStream networkStream;

    private readonly int protocolVersion;
    private int compressionThreshold;
    private AesStream? encryptionStream;

    private Stream stream;

    public MinecraftStream(NetworkStream networkStream, int protocolVersion)
    {
        this.protocolVersion = protocolVersion;
        this.networkStream = networkStream;
        stream = this.networkStream;
        
        compressionThreshold = CompressionDisabled;
    }

    public void EnableEncryption(byte[] sharedSecret)
    {
        Logger.Debug("Enabling encryption.");
        encryptionStream = new(networkStream, sharedSecret);
        stream           = encryptionStream;
    }

    public void SetCompression(int threshold)
    {
        compressionThreshold = threshold;
    }

    public PacketBuffer ReadPacket()
    {
        var    uncompressedLength = 0;
        var    length = ReadVarInt(out _);

        if (compressionThreshold != CompressionDisabled)
        {
            uncompressedLength =  ReadVarInt(out var r);
            length             -= r;
        }

        var data = new byte[length];
            
        var read = 0;
        while (read < length)
        {
            read += stream.Read(data, read, length - read);
        }
        
        var packetBuffer = uncompressedLength switch
        {
            > 0 => DecompressBuffer(data, uncompressedLength),
            _   => new(data, protocolVersion)
        };

        return packetBuffer;
    }

    public void WritePacket(PacketBuffer buffer)
    {
        if (compressionThreshold > 0)
        {
            buffer = CompressBuffer(buffer);
        }
        
        WriteVarInt((int)buffer.Size);
        stream.Write(buffer.GetBuffer().AsSpan());
    }

    private PacketBuffer DecompressBuffer(byte[] buffer, int length)
    {
        if (length == 0)
        {
            return new(buffer, protocolVersion);
        }

        var buffer2 = new byte[length];
        inflater.SetInput(buffer);
        inflater.Inflate(buffer2);
        inflater.Reset();

        return new(buffer2, protocolVersion);
    }

    private PacketBuffer CompressBuffer(PacketBuffer input)
    {
        var output = new PacketBuffer(protocolVersion);
        if (input.Size < compressionThreshold)
        {
            output.WriteVarInt(0);
            output.WriteBytes(input.GetBuffer().AsSpan());
            return output;
        }

        var buffer = input.GetBuffer();
        output.WriteVarInt(buffer.Length);

        deflater.SetInput(buffer);
        deflater.Finish();

        var deflateBuf = new byte[8192];
        while (!deflater.IsFinished)
        {
            var j = deflater.Deflate(deflateBuf);
            output.WriteBytes(deflateBuf.AsSpan(0, j));
        }

        deflater.Reset();
        return output;
    }

    private int ReadVarInt(out int read)
    {
        var value = 0;
        var length = 0;
        byte currentByte;

        while (true)
        {
            currentByte = (byte)stream.ReadByte();
            value |= (currentByte & 0x7F) << (length * 7);

            length++;
            if (length > 5)
            {
                throw new("VarInt too big");
            }

            if ((currentByte & 0x80) != 0x80)
            {
                break;
            }
        }

        read = length;
        return value;
    }

    private void WriteVarInt(int value)
    {
        while (true)
        {
            if ((value & ~0x7F) == 0)
            {
                stream.WriteByte((byte)value);
                return;
            }

            stream.WriteByte((byte)((value & 0x7F) | 0x80));
            value >>= 7;
        }
    }


    public void Close()
    {
        networkStream.Close();
        encryptionStream?.Close();
    }
}
