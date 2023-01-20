using MineSharp.Core.Logging;
using MineSharp.Core.Types.Enums;
using ICSharpCode.SharpZipLib.Zip.Compression;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Crypto;
using System.Net.Sockets;

namespace MineSharp.Protocol
{
    internal class MinecraftStream : Stream
    {

        private static Logger Logger = Logger.GetLogger();

        private readonly NetworkStream _networkStream;

        private object _streamLock;
        private Stream _baseStream;
        private AesStream? _encryptionStream;
        private int _compressionThreshold;

        public MinecraftStream(NetworkStream stream)
        {
            this._networkStream = stream;
            this._baseStream = this._networkStream;
            this._compressionThreshold = -1;
            this._streamLock = new object();
        }

        public override bool CanRead => this._baseStream.CanRead;
        public override bool CanSeek => this._baseStream.CanSeek;
        public override bool CanWrite => this._baseStream.CanWrite;
        public override long Length => this._baseStream.Length;
        public override long Position { get => this._baseStream.Position; set => this._baseStream.Position = value; }

        public bool IsAvailable => this._networkStream.DataAvailable;

        public void EnableEncryption(byte[] sharedSecret)
        {
            lock (this._streamLock)
            {
                var oldStream = this._baseStream;
                this._encryptionStream = new AesStream(oldStream, sharedSecret);
                this._baseStream = this._encryptionStream;
            }
        }

        public void SetCompressionThreshold(int compressionThreshold)
        {
            lock (this._streamLock)
            {
                this._compressionThreshold = compressionThreshold;
            }
        }

        public override void Flush()
        {
            this._baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) => this._baseStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => this._baseStream.Seek(offset, origin);

        public override void SetLength(long value) => this._baseStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => this._baseStream.Write(buffer, offset, count);

        public byte[] Read(int length)
        {
            var buffer = new byte[length];
            this._baseStream.Read(buffer, 0, length);
            return buffer;
        }

        public void Write(byte[] buffer)
        {
            this._baseStream.Write(buffer, 0, buffer.Length);
        }

        private void WriteVarInt(int value)
        {
            while (true)
            {
                if ((value & ~0x7F) == 0)
                {
                    this._baseStream.WriteByte((byte)value);
                    return;
                }
                this._baseStream.WriteByte((byte)(value & 0x7F | 0x80));
                value >>= 7;
            }
        }

        private int ReadVarInt(out int read)
        {
            var value = 0;
            var length = 0;
            byte currentByte;

            while (true)
            {
                currentByte = (byte)this._baseStream.ReadByte();
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

        private int ReadVarInt()
        {
            var value = 0;
            var length = 0;
            byte currentByte;

            while (true)
            {
                currentByte = (byte)this._baseStream.ReadByte();
                value |= (currentByte & 0x7F) << length * 7;

                length++;
                if (length > 5) throw new Exception("VarInt too big");

                if ((currentByte & 0x80) != 0x80)
                {
                    break;
                }
            }
            return value;
        }

        public PacketBuffer ReadPacket()
        {
            lock (this._streamLock)
            {
                var length = this.ReadVarInt();
                var uncompressedLength = 0;

                if (this._compressionThreshold > 0)
                {
                    var r = 0;
                    uncompressedLength = this.ReadVarInt(out r);
                    length -= r;
                }
                var data = this.Read(length);

                PacketBuffer packetBuffer;
                if (uncompressedLength > 0)
                {
                    packetBuffer = this.DecompressBuffer(data, uncompressedLength);
                } else
                {
                    packetBuffer = new PacketBuffer(data);
                }
                
                return packetBuffer;
            }
        }

        public void WritePacket(PacketBuffer buffer)
        {
            lock (this._streamLock)
            {
                if (this._compressionThreshold > 0)
                {
                    buffer = this.CompressBuffer(buffer, this._compressionThreshold);
                }
                this.WriteVarInt((int)buffer.Size);
                this.Write(buffer.ToArray());
            }
        }
        
        private PacketBuffer DecompressBuffer(byte[] buffer, int length)
        {
            if (length == 0) return new PacketBuffer(buffer);

            var inflater = new Inflater();

            inflater.SetInput(buffer);
            var abyte1 = new byte[length];
            inflater.Inflate(abyte1);
            inflater.Reset();
            return new PacketBuffer(abyte1);
        }

        private PacketBuffer CompressBuffer(PacketBuffer input, int compressionThreshold)
        {
            var output = new PacketBuffer();
            if (input.Size < compressionThreshold)
            {
                output.WriteVarInt(0);
                output.WriteRaw(input.ToArray());
                return output;
            }

            var buffer = input.ToArray();
            output.WriteVarInt(buffer.Length);

            var deflater = new Deflater();
            deflater.SetInput(buffer);
            deflater.Finish();

            var deflateBuf = new byte[8192];
            while (!deflater.IsFinished)
            {
                var j = deflater.Deflate(deflateBuf);
                output.WriteRaw(deflateBuf, 0, j);
            }
            deflater.Reset();
            return output;
        }
        
        protected override void Dispose(bool disposing)
        {
            // network stream
            this._encryptionStream?.Dispose();
            base.Dispose(disposing);
        }
    }
}
