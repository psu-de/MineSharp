using MineSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Effects {
    public class Effect {

        public EffectInfo Info { get; set; }
        public byte Amplifier { get; set; }

        private DateTime lastUpdated;
        private int _duration;

        /// <summary>
        /// Effect duration in Minecraft ticks
        /// </summary>
        public int Duration { get { 
                _duration -= (int)((DateTime.Now - lastUpdated).TotalMilliseconds / MinecraftConst.TickMs);
                return _duration;
            } set { 
                _duration = value; 
                lastUpdated = DateTime.Now; 
            } }
        public byte Flags { get; set; }

        public Effect(EffectInfo info, byte amplifier, int duration, byte flags) {
            this.Info = info;
            this.Amplifier = amplifier;
            this._duration = duration;
            this.lastUpdated = DateTime.Now;
            this.Flags = flags;
        }

    }
}
