using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MineSharp.Protocol.Cryptography
{
    public class AesStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly BufferedBlockCipher _decryptCipher;
        private readonly BufferedBlockCipher _encryptCipher;

        public AesStream(Stream stream, byte[] key)
        {
            this._encryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
            this._encryptCipher.Init(true, new ParametersWithIV(new KeyParameter(key), key, 0, 16));

            this._decryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
            this._decryptCipher.Init(false, new ParametersWithIV(new KeyParameter(key), key, 0, 16));

            this._baseStream = new CipherStream(stream, this._decryptCipher, this._encryptCipher);
        }

        public override bool CanRead => this._baseStream.CanRead;

        public override bool CanSeek => this._baseStream.CanSeek;

        public override bool CanWrite => this._baseStream.CanWrite;

        public override long Length => this._baseStream.Length;

        public override long Position { get => this._baseStream.Position; set => throw new NotSupportedException(); }

        public override void Flush()
        {
            this._baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) => this._baseStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => this._baseStream.Seek(offset, origin);

        public override void SetLength(long value)
        {
            this._baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this._baseStream.Write(buffer, offset, count);
        }
    }
}
