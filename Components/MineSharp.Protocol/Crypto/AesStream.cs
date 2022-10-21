using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MineSharp.Protocol.Crypto
{
    public class AesStream : Stream
    {
        private readonly Stream BaseStream;
        private readonly BufferedBlockCipher DecryptCipher;

        private readonly BufferedBlockCipher EncryptCipher;

        public AesStream(Stream stream, byte[] key)
        {
            this.EncryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
            this.EncryptCipher.Init(true, new ParametersWithIV(new KeyParameter(key), key, 0, 16));

            this.DecryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
            this.DecryptCipher.Init(false, new ParametersWithIV(new KeyParameter(key), key, 0, 16));

            this.BaseStream = new CipherStream(stream, this.DecryptCipher, this.EncryptCipher);
        }

        public override bool CanRead => this.BaseStream.CanRead;

        public override bool CanSeek => this.BaseStream.CanSeek;

        public override bool CanWrite => this.BaseStream.CanWrite;

        public override long Length => this.BaseStream.Length;

        public override long Position { get => this.BaseStream.Position; set => throw new NotImplementedException(); }

        public override void Flush()
        {
            this.BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) => this.BaseStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => this.BaseStream.Seek(offset, origin);

        public override void SetLength(long value)
        {
            this.BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.BaseStream.Write(buffer, offset, count);
        }
    }
}
