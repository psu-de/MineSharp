using MineSharp.Core.Types;
using MineSharp.Pathfinding.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;

namespace MineSharp.Pathfinding.Moves
{
    public class DirectMove : Move
    {
        private static readonly Logger Logger = Logger.GetLogger();
        private Vector3 _direction;
        public override Vector3 MoveVector => _direction;

        internal DirectMove(Movements movements, Vector3 direction) : base(movements)
        {
            this._direction = direction;
        }

        public override async Task PerformMove(MinecraftBot bot)
        {
            var target = bot.BotEntity!.Position.Plus(this.MoveVector);
            
            var yaw = Math.Atan2(-this.MoveVector.X, this.MoveVector.Z) * 57.2958d;
            Logger.Debug($"Performing direct move: {MoveVector} => {yaw}");
            Logger.Debug($"Target: {target}");
            bot.ForceSetRotation((float)yaw, 0);
            
            bot.PlayerControls.Walk(WalkDirection.Forward);
            await bot.WaitUntilReached(target);
        }
    }
}
