using ICSharpCode.SharpZipLib.Zip.Compression;
using MineSharp.Core.Common;
using MineSharp.Protocol.Cryptography;
using NLog;
using System.Net.Sockets;

namespace MineSharp.Protocol;

internal class MinecraftStream
{
    private const int COMPRESSION_DISABLED = -1;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly Inflater Inflater = new Inflater();
    private readonly Deflater Deflater = new Deflater();

    private readonly bool          _useAnonymousNbt;
    private readonly object        _streamLock;
    private readonly NetworkStream _networkStream;
    private          AesStream?    _encryptionStream;

    private Stream _stream;
    private int    _compressionThreshold;

    public MinecraftStream(NetworkStream networkStream, bool useAnonymousNbt)
    {
        this._useAnonymousNbt = useAnonymousNbt;
        this._networkStream   = networkStream;
        this._stream          = this._networkStream;

        this._streamLock           = new object();
        this._compressionThreshold = COMPRESSION_DISABLED;
    }

    public void EnableEncryption(byte[] sharedSecret)
    {
        Logger.Debug("Enabling encryption.");

        lock (this._streamLock)
        {
            this._encryptionStream = new AesStream(this._networkStream, sharedSecret);
            this._stream           = this._encryptionStream;
        }
    }

    public void SetCompression(int threshold)
    {
        lock (this._streamLock)
        {
            this._compressionThreshold = threshold;
        }
    }

    public PacketBuffer ReadPacket()
    {
        lock (this._streamLock)
        {
            var length             = this.ReadVarInt(out _);
            var uncompressedLength = 0;

            if (this._compressionThreshold != COMPRESSION_DISABLED)
            {
                uncompressedLength =  this.ReadVarInt(out var r);
                length             -= r;
            }

            int    read = 0;
            byte[] data = new byte[length];
            while (read < length)
                read += this._stream.Read(data, read, length - read);

            PacketBuffer packetBuffer = uncompressedLength switch
            {
                > 0 => this.DecompressBuffer(data, uncompressedLength),
                _   => new PacketBuffer(data, this._useAnonymousNbt)
            };

            return packetBuffer;
        }
    }

    public void WritePacket(PacketBuffer buffer)
    {
        lock (this._streamLock)
        {
            if (this._compressionThreshold > 0)
            {
                buffer = this.CompressBuffer(buffer);
            }

            this.WriteVarInt((int)buffer.Size);
            this._stream.Write(buffer.GetBuffer().AsSpan());
        }
    }

    private PacketBuffer DecompressBuffer(byte[] buffer, int length)
    {
        if (length == 0)
        {
            return new PacketBuffer(buffer, this._useAnonymousNbt);
        }

        var buffer2 = new byte[length];
        Inflater.SetInput(buffer);
        Inflater.Inflate(buffer2);
        Inflater.Reset();

        return new PacketBuffer(buffer2, this._useAnonymousNbt);
    }

    private PacketBuffer CompressBuffer(PacketBuffer input)
    {
        var output = new PacketBuffer(this._useAnonymousNbt);
        if (input.Size < this._compressionThreshold)
        {
            output.WriteVarInt(0);
            output.WriteBytes(input.GetBuffer().AsSpan());
            return output;
        }

        var buffer = input.GetBuffer();
        output.WriteVarInt(buffer.Length);

        Deflater.SetInput(buffer);
        Deflater.Finish();

        var deflateBuf = new byte[8192];
        while (!Deflater.IsFinished)
        {
            var j = Deflater.Deflate(deflateBuf);
            output.WriteBytes(deflateBuf.AsSpan(0, j));
        }

        Deflater.Reset();
        return output;
    }

    private int ReadVarInt(out int read)
    {
        var  value  = 0;
        var  length = 0;
        byte currentByte;

        while (true)
        {
            currentByte =  (byte)this._stream.ReadByte();
            value       |= (currentByte & 0x7F) << length * 7;

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

    private void WriteVarInt(int value)
    {
        while (true)
        {
            if ((value & ~0x7F) == 0)
            {
                this._stream.WriteByte((byte)value);
                return;
            }

            this._stream.WriteByte((byte)(value & 0x7F | 0x80));
            value >>= 7;
        }
    }


    public void Close()
    {
        this._networkStream.Close();
        this._encryptionStream?.Close();
    }
}
