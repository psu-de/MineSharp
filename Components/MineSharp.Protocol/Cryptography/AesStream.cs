using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MineSharp.Protocol.Cryptography;

/// <summary>
/// Encryption stream
/// </summary>
public class AesStream : Stream
{
    private readonly Stream              _baseStream;
    private readonly BufferedBlockCipher _decryptCipher;
    private readonly BufferedBlockCipher _encryptCipher;

    /// <summary>
    /// Create a new instance of AesStream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="key"></param>
    public AesStream(Stream stream, byte[] key)
    {
        this._encryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        this._encryptCipher.Init(true, new ParametersWithIV(new KeyParameter(key), key, 0, 16));

        this._decryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        this._decryptCipher.Init(false, new ParametersWithIV(new KeyParameter(key), key, 0, 16));

        this._baseStream = new CipherStream(stream, this._decryptCipher, this._encryptCipher);
    }

    /// <inheritdoc />
    public override bool CanRead => this._baseStream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => this._baseStream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => this._baseStream.CanWrite;

    /// <inheritdoc />
    public override long Length => this._baseStream.Length;

    /// <inheritdoc />
    public override long Position { get => this._baseStream.Position; set => throw new NotSupportedException(); }

    /// <inheritdoc />
    public override void Flush()
    {
        this._baseStream.Flush();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count) => this._baseStream.Read(buffer, offset, count);

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin) => this._baseStream.Seek(offset, origin);

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        this._baseStream.SetLength(value);
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        this._baseStream.Write(buffer, offset, count);
    }
}
