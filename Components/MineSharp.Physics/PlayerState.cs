using MineSharp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Physics {
    public class PlayerState {

        public bool IsInWater { get; set; } = false;
        public bool IsInLava { get; set; } = false;
        public bool IsInWeb { get; set; } = false;
        public bool IsCollidedHorizontally { get; set; } = false;
        public bool IsCollidedVertically { get; set; } = false;
        public int JumpTicks { get; set; } = 0;
        public bool JumpQueued { get; set; } = false;

        private Player Player;

        public int SlowFalling => Player.GetEffectLevel(Data.Effects.EffectType.SlowFalling) ?? 0;
        public int Levitation => Player.GetEffectLevel(Data.Effects.EffectType.Levitation) ?? 0;
        public int DolphinsGrace => Player.GetEffectLevel(Data.Effects.EffectType.DolphinsGrace) ?? 0;
        public int JumpBoost => Player.GetEffectLevel(Data.Effects.EffectType.JumpBoost) ?? 0;

        //TODO: Depth Strider
        public int DepthStrider => 0;

        public PlayerState(Player player) {
            this.Player = player;
        }

    }
}
