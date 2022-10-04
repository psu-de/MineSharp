using MineSharp.Core.Types;
using MineSharp.Pathfinding.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;
using MineSharp.Data.Protocol.Play.Clientbound;

namespace MineSharp.Pathfinding.Moves
{
    public class DirectMove : Move
    {
        private static readonly Logger Logger = Logger.GetLogger();
        private Vector3 _direction;
        public override Vector3 MoveVector => _direction;

        private TaskCompletionSource TSC;
        private Vector3 Target;

        internal DirectMove(Movements movements, Vector3 direction) : base(movements)
        {
            this._direction = direction;
        }

        public override async Task PerformMove(MinecraftBot bot)
        {
            this.Target = bot.BotEntity!.Position.Floored()
                .Plus(new Vector3(0.5d, 0, 0.5d))
                .Plus(this.MoveVector);
            Logger.Debug($"DirectMove: Target={this.Target}");
            this.TSC = new TaskCompletionSource();            
            
            bot.PhysicsTick += OnTick;
            await this.TSC.Task;
            bot.PhysicsTick -= OnTick;

            await bot.PlayerControls.Reset();
        }

        private const double THRESHOLD = 0.04d;
        
        private double? _previousDistance = null; 
        private void OnTick (MinecraftBot sender)
        {
            if (this._previousDistance != null && this._previousDistance <= THRESHOLD)
            {
                return;
            }
            
            var delta = sender.BotEntity!.Position.Minus(this.Target);
            var length = delta.Length();
            Logger.Debug($"Distance to target: {delta.Length()}");

            if (length <= THRESHOLD)
            {
                Logger.Debug($"Reached target");
                this.TSC.SetResult();
                return;
            }

            if (this._previousDistance != null && length > this._previousDistance)
            {
                Logger.Warning($"Distance greater than previous distance");
            }

            var yaw = Math.Atan2(delta.X, -delta.Z) * (180 / Math.PI);
            sender.ForceSetRotation((float)yaw, 0);
            
            sender.PlayerControls.Walk(WalkDirection.Forward);
            this._previousDistance = length;
        }
    }
}
