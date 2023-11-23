using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Data;

namespace MineSharp.Physics
{
    public class PlayerState
    {

        private readonly MinecraftData _data;
        private readonly Entity _player;

        public PlayerState(MinecraftData data, Entity player)
        {
            this._data = data;
            this._player = player;
        }

        public bool IsInWater { get; set; } = false;
        public bool IsInLava { get; set; } = false;
        public bool IsInWeb { get; set; } = false;
        public bool IsCollidedHorizontally { get; set; } = false;
        public bool IsCollidedVertically { get; set; } = false;
        public int JumpTicks { get; set; } = 0;
        public bool JumpQueued { get; set; } = false;

        public int SlowFalling => this._player.GetEffectLevel(EffectType.SlowFalling) ?? 0;
        public int Levitation => this._player.GetEffectLevel(EffectType.Levitation) ?? 0;
        public int DolphinsGrace => this._player.GetEffectLevel(EffectType.DolphinsGrace) ?? 0;
        public int JumpBoost => this._player.GetEffectLevel(EffectType.JumpBoost) ?? 0;

        //TODO: Depth Strider
        public int DepthStrider => 0;
    }
}
