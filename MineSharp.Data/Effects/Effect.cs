using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Effects {
    public class Effect {

        public EffectInfo Info { get; set; }
        public byte Amplifier { get; set; }
        public int Duration { get; set; }
        public byte Flags { get; set; }

        public Effect(EffectInfo info, byte amplifier, int duration, byte flags) {
            this.Info = info;
            this.Amplifier = amplifier;
            this.Duration = duration;
            this.Flags = flags;
        }

    }
}
