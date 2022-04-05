using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Extensions {
    public static class StreamExtensions {


        public static int ReadVarInt(this Stream stream, out int read) {
            int value = 0;
            int length = 0;
            byte currentByte;

            while (true) {
                currentByte = (byte)stream.ReadByte();
                value |= (currentByte & 0x7F) << (length * 7);

                length++;
                if (length > 5) throw new Exception("VarInt too big");

                if ((currentByte & 0x80) != 0x80) {
                    break;
                }
            }
            read = length;
            return value;
        }

        public static int ReadVarInt(this Stream stream) {
            int value = 0;
            int length = 0;
            byte currentByte;

            while (true) {
                currentByte = (byte)stream.ReadByte();
                value |= (currentByte & 0x7F) << (length * 7);

                length++;
                if (length > 5) throw new Exception("VarInt too big");

                if ((currentByte & 0x80) != 0x80) {
                    break;
                }
            }
            return value;
        }


        public static void WriteVarInt(this Stream stream, int value) {
            while (true) {
                if ((value & ~0x7F) == 0) {
                    stream.WriteByte((byte)value);
                    return;
                }
                stream.WriteByte((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
        }
    }
}
