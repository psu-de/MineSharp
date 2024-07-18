using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MineSharp.Protocol.Cryptography;

/// <summary>
///     Encryption stream
/// </summary>
public class AesStream : Stream
{
    private readonly Stream baseStream;
    private readonly BufferedBlockCipher decryptCipher;
    private readonly BufferedBlockCipher encryptCipher;

    /// <summary>
    ///     Create a new instance of AesStream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="key"></param>
    public AesStream(Stream stream, byte[] key)
    {
        encryptCipher = new(new CfbBlockCipher(new AesEngine(), 8));
        encryptCipher.Init(true, new ParametersWithIV(new KeyParameter(key), key, 0, 16));

        decryptCipher = new(new CfbBlockCipher(new AesEngine(), 8));
        decryptCipher.Init(false, new ParametersWithIV(new KeyParameter(key), key, 0, 16));

        baseStream = new CipherStream(stream, decryptCipher, encryptCipher);
    }

    /// <inheritdoc />
    public override bool CanRead => baseStream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => baseStream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => baseStream.CanWrite;

    /// <inheritdoc />
    public override long Length => baseStream.Length;

    /// <inheritdoc />
    public override long Position { get => baseStream.Position; set => throw new NotSupportedException(); }

    /// <inheritdoc />
    public override void Flush()
    {
        baseStream.Flush();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        return baseStream.Read(buffer, offset, count);
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        return baseStream.Seek(offset, origin);
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        baseStream.SetLength(value);
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        baseStream.Write(buffer, offset, count);
    }
}
