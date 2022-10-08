using MineSharp.Core.Logging;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Crypto;
using System.Net.Sockets;
namespace MineSharp.Protocol
{
    internal class MinecraftStream : Stream
    {

        private static Logger Logger = Logger.GetLogger();

        private Stream _baseStream;
        private AesStream? _encryptionStream;

        private readonly NetworkStream _networkStream;

        public MinecraftStream(NetworkStream stream)
        {
            this._networkStream = stream;
            this._baseStream = this._networkStream;
        }

        public override bool CanRead => this._baseStream.CanRead;
        public override bool CanSeek => this._baseStream.CanSeek;
        public override bool CanWrite => this._baseStream.CanWrite;
        public override long Length => this._baseStream.Length;
        public override long Position { get => this._baseStream.Position; set => this._baseStream.Position = value; }

        public bool IsAvailable => this._networkStream.DataAvailable;

        public void EnableEncryption(byte[] sharedSecret)
        {
            var oldStream = this._baseStream;
            this._encryptionStream = new AesStream(oldStream, sharedSecret);
            this._baseStream = this._encryptionStream;
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

        public void DispatchPacket(PacketBuffer packetBuffer)
        {
            this.WriteVarInt((int)packetBuffer.Size);
            this.Write(packetBuffer.ToArray());
        }

        public void WriteVarInt(int value)
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

        public int ReadVarInt(out int read)
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

        public int ReadVarInt()
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
    }
}
