using MineSharp.Bot.Enums;
using MineSharp.Data.Protocol.Play.Serverbound;
using MineSharp.Physics;

namespace MineSharp.Bot.Modules.Physics
{
    public class PlayerControls
    {
        public MinecraftBot Bot { get; set; }

        private bool _isSprinting = false;
        private bool _isSneaking = false;
        private bool _walkBackward = false;
        private bool _walkForward = false;
        private bool _walkRight = false;
        private bool _walkLeft = false;
        private bool _jumpQueued = false;

        public bool IsSprinting
        {
            get { return _isSprinting; }
            set {
                if (value == _isSprinting) return;

                if (value)
                {
                    StartSprinting().Wait();
                }
                else
                {
                    StopSprinting().Wait();
                }
            }
        }
        public bool IsSneaking
        {
            get { return _isSneaking; }
            set {
                if (value == _isSneaking) return;

                if (value)
                {
                    StartSneaking().Wait();
                }
                else
                {
                    StopSneaking().Wait();
                }
            }
        }

        public bool IsWalkingForward
        {
            get { return _walkForward; }
            set {
                if (_walkForward == value) return;
                if (value) Walk(WalkDirection.Forward);
                else StopWalk(WalkDirection.Forward);
            }
        }
        public bool IsWalkingBackward
        {
            get { return _walkBackward; }
            set {
                if (_walkBackward == value) return;
                if (value) Walk(WalkDirection.Backward);
                else StopWalk(WalkDirection.Backward);
            }
        }
        public bool IsWalkingRight
        {
            get { return _walkRight; }
            set {
                if (_walkRight == value) return;
                if (value) Walk(WalkDirection.Right);
                else StopWalk(WalkDirection.Right);
            }
        }
        public bool IsWalkingLeft
        {
            get { return _walkLeft; }
            set {
                if (_walkLeft == value) return;
                if (value) Walk(WalkDirection.Left);
                else StopWalk(WalkDirection.Left);
            }
        }


        public PlayerControls(MinecraftBot bot)
        {
            this.Bot = bot;
        }

        public async Task Reset()
        {
            _walkBackward = false;
            _walkForward = false;
            _walkLeft = false;
            _walkRight = false;

            await Task.WhenAll(StopSprinting(), StopSneaking());
        }

        public async Task StartSprinting(CancellationToken? cancellation = null)
        {
            if (_isSprinting) return;
            var packet = new PacketEntityAction(this.Bot.BotEntity!.ServerId, (byte)EntityAction.StartSprinting, 0);
            await this.Bot.Client.SendPacket(packet, cancellation);
            _isSprinting = true;
        }

        public async Task StopSprinting(CancellationToken? cancellation = null)
        {
            if (!_isSprinting) return;
            var packet = new PacketEntityAction(this.Bot.BotEntity!.ServerId, (byte)EntityAction.StopSprinting, 0);
            await this.Bot.Client.SendPacket(packet, cancellation);
            _isSprinting = false;
        }


        public async Task StartSneaking(CancellationToken? cancellation = null)
        {
            var packet = new PacketEntityAction(this.Bot.BotEntity!.ServerId, (byte)EntityAction.StartSneaking, 0);
            await this.Bot.Client.SendPacket(packet, cancellation);
            _isSneaking = true;
        }

        public async Task StopSneaking(CancellationToken? cancellation = null)
        {
            var packet = new PacketEntityAction(this.Bot.BotEntity!.ServerId, (byte)EntityAction.StopSneaking, 0);
            await this.Bot.Client.SendPacket(packet, cancellation);
            _isSneaking = false;
        }


        public void Walk(WalkDirection dir)
        {
            switch (dir)
            {
                case WalkDirection.Backward: _walkBackward = true; break;
                case WalkDirection.Forward: _walkForward = true; break;
                case WalkDirection.Left: _walkLeft = true; break;
                case WalkDirection.Right: _walkRight = true; break;
            }
        }

        public void StopWalk(WalkDirection dir)
        {
            switch (dir)
            {
                case WalkDirection.Backward: _walkBackward = false; break;
                case WalkDirection.Forward: _walkForward = false; break;
                case WalkDirection.Left: _walkLeft = false; break;
                case WalkDirection.Right: _walkRight = false; break;
            }
        }

        public void Jump()
        {
            _jumpQueued = true;
        }

        internal MovementControls PrepareForPhysicsTick()
        {
            var controls = new MovementControls() {
                Back = _walkBackward,
                Forward = _walkForward,
                Right = _walkRight,
                Left = _walkLeft,
                Sprint = _isSprinting,
                Sneak = _isSneaking,
                Jump = _jumpQueued,
            };

            _jumpQueued = false;

            return controls;
        }
    }
}
