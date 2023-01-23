using MineSharp.Core.Types;
using MineSharp.Data.Effects;
using MineSharp.Data.Entities;

namespace MineSharp.Physics
{
    public class PlayerState
    {

        private readonly Entity Player;

        public PlayerState(Entity player)
        {
            this.Player = player;
        }

        public bool IsInWater { get; set; } = false;
        public bool IsInLava { get; set; } = false;
        public bool IsInWeb { get; set; } = false;
        public bool IsCollidedHorizontally { get; set; } = false;
        public bool IsCollidedVertically { get; set; } = false;
        public int JumpTicks { get; set; } = 0;
        public bool JumpQueued { get; set; } = false;

        public int SlowFalling => this.Player.GetEffectLevel((int)EffectType.SlowfallingEffect) ?? 0;
        public int Levitation => this.Player.GetEffectLevel((int)EffectType.LevitationEffect) ?? 0;
        public int DolphinsGrace => this.Player.GetEffectLevel((int)EffectType.DolphinsgraceEffect) ?? 0;
        public int JumpBoost => this.Player.GetEffectLevel((int)EffectType.JumpboostEffect) ?? 0;

        //TODO: Depth Strider
        public int DepthStrider => 0;
    }
}
