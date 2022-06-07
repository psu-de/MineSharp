﻿using MineSharp.Core.Types;
using MineSharp.Protocol.Packets;
using MineSharp.World.PalettedContainer.Palettes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.World.PalettedContainer {
    public class BlockPalettedContainer : IPalettedContainer {

        public static BlockPalettedContainer Read(PacketBuffer buffer) {
            byte bitsPerEntry = buffer.ReadByte();
            var palette = GetPalette(bitsPerEntry);
            palette.Read(buffer);

            long[] data = new long[buffer.ReadVarInt()];
            for (int i = 0; i < data.Length; i++) data[i] = buffer.ReadLong();

            return new BlockPalettedContainer(palette, new IntBitArray(data, bitsPerEntry));
        }

        private static IPalette GetPalette(byte bitsPerEntry) => bitsPerEntry switch {
            0 => new SingleValuePalette(),
            <= IndirectPalette.BLOCK_MAX_BITS => new IndirectPalette(),
            _ => new DirectPalette()
        };

        public IPalette Palette { get; set; }
        public int Capacity => 16 * 16 * 16;
        public IntBitArray Data { get; set; }

        public BlockPalettedContainer(IPalette palette, IntBitArray data) { 
            Palette = palette;
            Data = data;
        }

        public int GetAt(int index) {
            if (index < 0 || index >= Capacity) 
                throw new ArgumentOutOfRangeException(nameof(index));

            if (this.Palette is SingleValuePalette)
                return this.Palette.Get(0);

            var value = Data.Get(index);
            return this.Palette.Get(value);
        }

        public void SetAt(int index, int state) {

            if (index < 0 || index >= Capacity)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (this.Palette.HasState(state, state)) {
                switch (this.Palette) {
                    case SingleValuePalette svp: break;
                    case IndirectPalette ip:
                        var mapIndex = ip.GetStateIndex(state);
                        this.Data.Set(index, mapIndex);
                        break;
                    case DirectPalette dp:
                        this.Data.Set(index, state);
                        break;
                }
            } else {

                switch (this.Palette) {
                    case SingleValuePalette svp:
                        this.Palette = svp.ConvertToIndirectPalette(state);
                        this.Data = new IntBitArray(Enumerable.Repeat(0L, this.Data.Capacity / (64 / IndirectPalette.BLOCK_MIN_BITS)).ToArray(), IndirectPalette.BLOCK_MIN_BITS);
                        this.Data.Set(index, 1);
                        break;
                    case IndirectPalette dp:
                        var newPalette = dp.AddState(state, false, out var newBitsPerEntry);
                        var newData = new IntBitArray(Enumerable.Repeat(0L, this.Data.Capacity / (64 / newBitsPerEntry)).ToArray(), newBitsPerEntry);
                        for (int i = 0; i < this.Data.Capacity; i++) {
                            if (newPalette is DirectPalette)
                                newData.Set(i, GetAt(i));
                            else if (newPalette is IndirectPalette)
                                newData.Set(i, ((IndirectPalette)newPalette).GetStateIndex(this.Data.Get(i)));
                        }
                        if (newPalette is DirectPalette)
                            newData.Set(index, state);
                        else 
                            newData.Set(index, ((IndirectPalette)newPalette).GetStateIndex(state));
                        break;
                }

            }

        }
    }
}
