using System.Net.Sockets;
using ICSharpCode.SharpZipLib.Zip.Compression;
using MineSharp.Core.Serialization;
using MineSharp.Protocol.Cryptography;
using NLog;

namespace MineSharp.Protocol;

/// <summary>
/// Handles reading and writing packets.
/// Also handles encryption and compression.
/// This class is thread-safe.
/// </summary>
internal class MinecraftStream
{
    private const int CompressionDisabled = -1;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly NetworkStream networkStream;

    private readonly int protocolVersion;

    // Always acquire the readLock before the writeLock if both are needed
    private readonly object readLock = new();
    private readonly object writeLock = new();

    private readonly ThreadLocal<Inflater> inflater = new(() => new());
    private readonly ThreadLocal<Deflater> deflater = new(() => new());

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
        stream = encryptionStream;
    }

    public void SetCompression(int threshold)
    {
        compressionThreshold = threshold;
    }

    public PacketBuffer ReadPacket()
    {
        var localCompressionThreshold = compressionThreshold;

        var uncompressedLength = 0;
        byte[] data = Array.Empty<byte>();
        lock (readLock)
        {
            var length = PacketBuffer.ReadVarInt(stream, out _);

            if (localCompressionThreshold != CompressionDisabled)
            {
                uncompressedLength = PacketBuffer.ReadVarInt(stream, out var r);
                length -= r;
            }

            data = new byte[length];

            var readRemaining = length;
            var readStart = 0;
            while (readRemaining > 0)
            {
                var read = stream.Read(data, readStart, readRemaining);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }
                readStart += read;
                readRemaining -= read;
            }
        }

        var packetBuffer = uncompressedLength switch
        {
            > 0 => DecompressBuffer(data, uncompressedLength),
            _ => new(data, protocolVersion)
        };

        return packetBuffer;
    }

    public void WritePacket(PacketBuffer buffer)
    {
        var localCompressionThreshold = compressionThreshold;
        if (localCompressionThreshold > 0)
        {
            buffer = CompressBuffer(buffer, localCompressionThreshold);
        }

        lock (writeLock)
        {
            PacketBuffer.WriteVarInt(stream, (int)buffer.Size);
            stream.Write(buffer.GetBuffer());
        }
    }

    private PacketBuffer DecompressBuffer(byte[] buffer, int length)
    {
        if (length == 0)
        {
            return new(buffer, protocolVersion);
        }

        var buffer2 = new byte[length];
        var localInflater = inflater.Value!;
        localInflater.SetInput(buffer);
        localInflater.Inflate(buffer2);
        localInflater.Reset();

        return new(buffer2, protocolVersion);
    }

    // compressionThreshold is given as a parameter to make it thread-safe
    private PacketBuffer CompressBuffer(PacketBuffer input, int compressionThreshold)
    {
        var output = new PacketBuffer(protocolVersion);
        var buffer = input.GetBuffer();

        if (input.Size < compressionThreshold)
        {
            output.WriteVarInt(0);
            output.WriteBytes(buffer);
            return output;
        }

        output.WriteVarInt(buffer.Length);

        var localDeflater = deflater.Value!;
        localDeflater.SetInput(buffer);
        localDeflater.Finish();

        var deflateBuf = new byte[8192];
        while (!localDeflater.IsFinished)
        {
            var j = localDeflater.Deflate(deflateBuf);
            output.WriteBytes(deflateBuf.AsSpan(0, j));
        }

        localDeflater.Reset();
        return output;
    }

    /// <summary>
    /// This method checks if the stream is still connected.
    /// This is method can be useful when you want to determine whether the stream is dead without reading or writing to it.
    /// This is because the stream does not throw an exception when it is dead until you read or write to it.
    /// </summary>
    public bool CheckStreamUseable()
    {
        var r = networkStream.Socket.Poll(0, SelectMode.SelectRead);

        if (r && !networkStream.DataAvailable)
        {
            // the socket got closed
            // we would only get the exception next time we read or write to it
            // so we do that now to get the exception
            // we can not do this always because doing so would change the stream or block
            var buffer = Array.Empty<byte>();
            networkStream.Socket.Receive(buffer, SocketFlags.Peek); // this will throw an exception
            return false;
        }
        return networkStream.Socket.Connected;
    }

    public void Close()
    {
        lock (readLock)
            lock (writeLock)
            {
                networkStream.Close();
                encryptionStream?.Close();
            }
    }
}
