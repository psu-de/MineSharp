using MineSharp.Data.Effects;
using MineSharp.Data.Entities;

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

        public int SlowFalling => Player.GetEffectLevel(SlowfallingEffect.EffectId) ?? 0;
        public int Levitation => Player.GetEffectLevel(LevitationEffect.EffectId) ?? 0;
        public int DolphinsGrace => Player.GetEffectLevel(DolphinsgraceEffect.EffectId) ?? 0;
        public int JumpBoost => Player.GetEffectLevel(JumpboostEffect.EffectId) ?? 0;

        //TODO: Depth Strider
        public int DepthStrider => 0;

        public PlayerState(Player player) {
            this.Player = player;
        }

    }
}
