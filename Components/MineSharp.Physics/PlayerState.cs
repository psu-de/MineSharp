using MineSharp.Core.Common.Entities;
using MineSharp.Data;

namespace MineSharp.Physics
{
    public class PlayerState
    {

        private readonly MinecraftData _data;
        private readonly Entity _player;

        private readonly int _jumpBoostEffect;
        private readonly int _levitationEffect;
        private readonly int _dolpinsGraceEffect;
        private readonly int _slowfallingEffect;

        public PlayerState(MinecraftData data, Entity player)
        {
            this._data = data;
            this._player = player;

            this._jumpBoostEffect = data.Effects.GetByName("JumpBoost").Id;
            this._levitationEffect = data.Effects.GetByName("Levitation").Id;
            this._dolpinsGraceEffect = data.Effects.GetByName("DolphinsGrace").Id;
            this._slowfallingEffect = data.Effects.GetByName("SlowFalling").Id;
        }

        public bool IsInWater { get; set; } = false;
        public bool IsInLava { get; set; } = false;
        public bool IsInWeb { get; set; } = false;
        public bool IsCollidedHorizontally { get; set; } = false;
        public bool IsCollidedVertically { get; set; } = false;
        public int JumpTicks { get; set; } = 0;
        public bool JumpQueued { get; set; } = false;

        public int SlowFalling => this._player.GetEffectLevel(this._slowfallingEffect) ?? 0;
        public int Levitation => this._player.GetEffectLevel(this._levitationEffect) ?? 0;
        public int DolphinsGrace => this._player.GetEffectLevel(this._dolpinsGraceEffect) ?? 0;
        public int JumpBoost => this._player.GetEffectLevel(this._jumpBoostEffect) ?? 0;

        //TODO: Depth Strider
        public int DepthStrider => 0;
    }
}
