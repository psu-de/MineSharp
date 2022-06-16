using fNbt;
using ICSharpCode.SharpZipLib.Zip.Compression;
using MineSharp.Core.Types;
using MineSharp.Data.Items;
using System.Text;

namespace MineSharp.Protocol.Packets {
    public class PacketBuffer {

        public static PacketBuffer Decompress(byte[] buffer, int length) {
            if (length == 0) return new PacketBuffer(buffer);

            var inflater = new Inflater();

            inflater.SetInput(buffer);
            byte[] abyte1 = new byte[length];
            inflater.Inflate(abyte1);
            inflater.Reset();
            return new PacketBuffer(abyte1);
        }


        public static PacketBuffer Compress(PacketBuffer input, int compressionThreshold) {
            PacketBuffer output = new PacketBuffer();
            if (input.Size < compressionThreshold) {
                output.WriteVarInt(0);
                output.WriteRaw(input.ToArray());
                return output;
            }

            byte[] buffer = input.ToArray();
            output.WriteVarInt(buffer.Length);

            var deflater = new Deflater();
            deflater.SetInput(buffer);
            deflater.Finish();

            byte[] deflateBuf = new byte[8192];
            while (!deflater.IsFinished) {
                int j = deflater.Deflate(deflateBuf);
                output.WriteRaw(deflateBuf, 0, j);
            }
            deflater.Reset();
            return output;
        }





        private MemoryStream _buffer;

        public long Size => this._buffer.Length;
        public long ReadableBytes => this._buffer.Length - this._buffer.Position;
        public long Position => this._buffer.Position;


        public PacketBuffer() {
            this._buffer = new MemoryStream();
        }

        public PacketBuffer(byte[] buffer) {
            this._buffer = new MemoryStream(buffer);
        }

        public byte[] ToArray() {
            return this._buffer.ToArray();
        }

        public string HexDump() {
            return string.Join(" ", this.ToArray().Select(x => x.ToString("X2")));
        }

        #region Writing

        public void WriteRaw(byte[] buffer) {
            this._buffer.Write(buffer, 0, buffer.Length);
        }

        public void WriteRaw(byte[] buffer, int offset, int length) {
            this._buffer.Write(buffer, offset, length);
        }

        public void WriteLong(long value) {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            this.WriteRaw(bytes);
        }

        public void WriteByte(byte value) {
            this._buffer.WriteByte(value);
        }

        public void WriteVarInt(int value) {
            while ((value & -128) != 0) {
                this.WriteByte((byte)(value & 127 | 128));
                value >>= 7;
            }

            this.WriteByte((byte)value);
        }

        public void WriteVarLong(long value) {
            while (true) {
                if ((value & ~0x7F) == 0) {
                    this.WriteByte((byte)value);
                    return;
                }

                this.WriteByte((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
        }

        public void WriteString(string value) {
            byte[] encoded = Encoding.UTF8.GetBytes(value);
            this.WriteVarInt(encoded.Length);
            this._buffer.Write(encoded, 0, encoded.Length);
        }

        public void WriteShort(short value) {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            this.WriteRaw(bytes);
        }

        public void WriteUShort(ushort value) {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            this.WriteRaw(bytes);
        }

        public void WriteChat(Chat value) {
            this.WriteString(value.JSON);
        }

        public void WriteByteArray(byte[] bytes) {
            this.WriteVarInt(bytes.Length);
            this.WriteRaw(bytes);
        }

        public void WriteUUID(UUID value) {
            this.WriteLong(value.MostSignificantBits);
            this.WriteLong(value.LeastSignificantBits);
        }

        public void WriteDouble(double value) {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            this.WriteRaw(bytes);
        }

        public void WriteFloat(float value) {
            byte[] bytes = BitConverter.GetBytes((float)value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            this.WriteRaw(bytes);
        }


        public void WriteInt(int value) {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            this.WriteRaw(bytes);
        }

        public void WriteNBTCompound(NbtCompound? value) {
            if (value == null) {
                this.WriteByte(0);
                return;
            }

            NbtFile f = new NbtFile(value) { BigEndian = true };
            f.SaveToStream(this._buffer, NbtCompression.None);
        }

        public void WriteBoolean(bool value) {
            this.WriteByte((byte)(value ? 1 : 0));
        }

        public void WriteIdentifier(Identifier value) {
            this.WriteString(value.ToString());
        }

        public void WriteIdentifierArray(Identifier[] value) {
            this.WriteVarInt(value.Length);
            for (int i = 0; i < value.Length; i++) this.WriteString(value[i]);
        }

        public void WriteSlot(Slot? value) {
            this.WriteBoolean(value != null && value.Item != null);
            if (value == null || value.Item == null)
                return;

            WriteVarInt(value.Item!.Id);
            WriteByte(value.Item!.Count);
            WriteNBTCompound(value.Item!.Metadata);
        }

        public void WriteSlotArray(Slot[] value) {
            if (value == null) {
                this.WriteVarInt(0);
                return;
            }

            this.WriteVarInt(value.Length);
            for (int i = 0; i < value.Length; i++) {
                this.WriteShort(value[i].SlotNumber);
                this.WriteSlot(value[i]);
            }
        }

        public void WriteULong(ulong value) {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            this.WriteRaw(bytes);
        }

        public void WritePosition(Position value) {
            this.WriteULong(value.ToULong());
        }

        public void WriteVarLongArray(long[] value) {
            this.WriteVarInt(value.Length);
            for (int i = 0; i < value.Length; i++) this.WriteVarLong(value[i]);
        }

        public void WriteAngle(Angle value) {
            this.WriteByte(value.ToByte());
        }

        public void WriteVarIntArray(int[] value) {
            this.WriteVarInt(value.Length);
            for (int i = 0; i < value.Length; i++) this.WriteVarInt(value[i]);
        }

        public void WriteBitSet(BitSet value) {
            this.WriteVarInt(value.Values.Length);
            for (int i = 0; i < value.Values.Length; i++) this.WriteLong(value.Values[i]);
        }

        #endregion


        #region Reading 

        public byte[] ReadRaw(int length) {
            byte[] buf = new byte[length];
            this._buffer.Read(buf, 0, length);
            return buf;
        }

        public long ReadLong() {
            byte[] raw = this.ReadRaw(8);
            if (BitConverter.IsLittleEndian) Array.Reverse(raw);

            return BitConverter.ToInt64(raw);
        }

        public int ReadInt() {
            byte[] raw = this.ReadRaw(4);
            if (BitConverter.IsLittleEndian) Array.Reverse(raw);

            return BitConverter.ToInt32(raw);
        }

        public byte ReadByte() {
            int val = this._buffer.ReadByte();
            if (val == -1) throw new EndOfStreamException();
            return (byte)val;
        }

        public int[] ReadVarIntArray() {
            int length = this.ReadVarInt();
            int[] array = new int[length];
            for (int i = 0; i < length; i++) array[i] = this.ReadVarInt();
            return array;
        }

        public string ReadString() {
            int length = this.ReadVarInt();
            byte[] encoded = this.ReadRaw(length);
            return Encoding.UTF8.GetString(encoded);
        }

        public ushort ReadUShort() {
            byte[] bytes = this.ReadRaw(2);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public short ReadShort() {
            byte[] bytes = this.ReadRaw(2);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        public Chat ReadChat() {
            return new Chat(this.ReadString());
        }

        public byte[] ReadByteArray() {
            int length = this.ReadVarInt();
            return this.ReadRaw(length);
        }

        public UUID ReadUUID() {
            long l1 = this.ReadLong();
            long l2 = this.ReadLong();
            return new UUID(l1, l2);
        }

        public double ReadDouble() {
            byte[] bytes = this.ReadRaw(8);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }

        public float ReadFloat() {
            byte[] bytes = this.ReadRaw(4);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        public int ReadVarInt() {
            int value = 0;
            int length = 0;
            byte currentByte;

            while (true) {
                currentByte = this.ReadByte();
                value |= (currentByte & 0x7F) << (length * 7);

                length += 1;
                if (length > 5) {
                    throw new Exception("VarInt is too big");
                }

                if ((currentByte & 0x80) != 0x80) {
                    break;
                }
            }
            return value;
        }

        public NbtCompound? ReadNBTCompound() {
            NbtTagType t = (NbtTagType)ReadByte();
            if (t != NbtTagType.Compound) return null;
            this._buffer.Position--;

            NbtFile file = new NbtFile() { BigEndian = true };

            file.LoadFromStream(this._buffer, NbtCompression.None);

            return (NbtCompound)file.RootTag;
        }

        public bool ReadBoolean() {
            return this.ReadByte() == 1;
        }

        public Identifier ReadIdentifier() {
            return new Identifier(this.ReadString());
        }

        public Identifier[] ReadIdentifierArray() {
            int len = this.ReadVarInt();
            Identifier[] result = new Identifier[len];
            for (int i = 0; i < len; i++) result[i] = this.ReadIdentifier();
            return result;
        }

        public Slot ReadSlot() {
            bool present = this.ReadBoolean();
            if (!present) return new Slot(null, -2);

            int id = ReadVarInt();
            byte count = 0;
            short damage = 0;
            NbtCompound? nbt = null;


            count = (byte)ReadByte();
            //	damage = ReadShort();
            nbt = this.ReadNBTCompound();

            var itemType = ItemPalette.GetItemTypeById(id);
            var item = ItemPalette.CreateItem(id, count, damage, nbt);
            Slot slot = new Slot(item, -2);
            return slot;
        }

        public ulong ReadULong() {
            byte[] bytes = this.ReadRaw(8);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public Position ReadPosition() {
            return new Position(this.ReadULong());
        }

        public long ReadVarLong() {
            long value = 0;
            int length = 0;
            byte currentByte;

            while (true) {
                currentByte = this.ReadByte();
                value |= (long)(currentByte & 0x7F) << (length * 7);

                length += 1;
                if (length > 10) {
                    throw new Exception("VarLong is too big");
                }

                if ((currentByte & 0x80) != 0x80) {
                    break;
                }
            }
            return value;
        }

        public long[] ReadVarLongArray() {
            int length = this.ReadVarInt();
            long[] array = new long[length];
            for (int i = 0; i < length; i++) array[i] = this.ReadVarLong();
            return array;
        }

        public Angle ReadAngle() {
            return Angle.FromByte(this.ReadByte());
        }

        public Slot[] ReadSlotArray() {
            int slotCount = this.ReadVarInt();
            Slot[] slots = new Slot[slotCount];

            for (int i = 0; i < slotCount; i++) { slots[i] = this.ReadSlot(); slots[i].SlotNumber = (short)i; }
            return slots;
        }

        public BitSet ReadBitSet() {
            int length = this.ReadVarInt();
            long[] values = new long[length];
            for (int i = 0; i < length; i++) values[i] = this.ReadLong();
            return new BitSet(values);
        }

        public string[] ReadStringArray() {
            int length = this.ReadVarInt();
            string[] array = new string[length];
            for (var i = 0; i < length; i++) array[i] = this.ReadString();
            return array;
        }

        #endregion
    }
}
