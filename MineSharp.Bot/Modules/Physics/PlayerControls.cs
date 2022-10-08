using MineSharp.Bot.Enums;
using MineSharp.Data.Protocol.Play.Serverbound;
using MineSharp.Physics;
namespace MineSharp.Bot.Modules.Physics
{
    public class PlayerControls
    {
        private bool _isSneaking;

        private bool _isSprinting;
        private bool _jumpQueued;
        private bool _walkBackward;
        private bool _walkForward;
        private bool _walkLeft;
        private bool _walkRight;


        public PlayerControls(MinecraftBot bot)
        {
            this.Bot = bot;
        }
        public MinecraftBot Bot { get; set; }

        public bool IsSprinting {
            get => this._isSprinting;
            set {
                if (value == this._isSprinting) return;

                if (value)
                {
                    this.StartSprinting().Wait();
                } else
                {
                    this.StopSprinting().Wait();
                }
            }
        }
        public bool IsSneaking {
            get => this._isSneaking;
            set {
                if (value == this._isSneaking) return;

                if (value)
                {
                    this.StartSneaking().Wait();
                } else
                {
                    this.StopSneaking().Wait();
                }
            }
        }

        public bool IsWalkingForward {
            get => this._walkForward;
            set {
                if (this._walkForward == value) return;
                if (value) this.Walk(WalkDirection.Forward);
                else this.StopWalk(WalkDirection.Forward);
            }
        }
        public bool IsWalkingBackward {
            get => this._walkBackward;
            set {
                if (this._walkBackward == value) return;
                if (value) this.Walk(WalkDirection.Backward);
                else this.StopWalk(WalkDirection.Backward);
            }
        }
        public bool IsWalkingRight {
            get => this._walkRight;
            set {
                if (this._walkRight == value) return;
                if (value) this.Walk(WalkDirection.Right);
                else this.StopWalk(WalkDirection.Right);
            }
        }
        public bool IsWalkingLeft {
            get => this._walkLeft;
            set {
                if (this._walkLeft == value) return;
                if (value) this.Walk(WalkDirection.Left);
                else this.StopWalk(WalkDirection.Left);
            }
        }

        public async Task Reset()
        {
            this._walkBackward = false;
            this._walkForward = false;
            this._walkLeft = false;
            this._walkRight = false;

            await Task.WhenAll(this.StopSprinting(), this.StopSneaking());
        }

        public async Task StartSprinting(CancellationToken? cancellation = null)
        {
            if (this._isSprinting) return;
            var packet = new PacketEntityAction(this.Bot.BotEntity!.ServerId, (byte)EntityAction.StartSprinting, 0);
            await this.Bot.Client.SendPacket(packet, cancellation);
            this._isSprinting = true;
        }

        public async Task StopSprinting(CancellationToken? cancellation = null)
        {
            if (!this._isSprinting) return;
            var packet = new PacketEntityAction(this.Bot.BotEntity!.ServerId, (byte)EntityAction.StopSprinting, 0);
            await this.Bot.Client.SendPacket(packet, cancellation);
            this._isSprinting = false;
        }


        public async Task StartSneaking(CancellationToken? cancellation = null)
        {
            var packet = new PacketEntityAction(this.Bot.BotEntity!.ServerId, (byte)EntityAction.StartSneaking, 0);
            await this.Bot.Client.SendPacket(packet, cancellation);
            this._isSneaking = true;
        }

        public async Task StopSneaking(CancellationToken? cancellation = null)
        {
            var packet = new PacketEntityAction(this.Bot.BotEntity!.ServerId, (byte)EntityAction.StopSneaking, 0);
            await this.Bot.Client.SendPacket(packet, cancellation);
            this._isSneaking = false;
        }


        public void Walk(WalkDirection dir)
        {
            switch (dir)
            {
                case WalkDirection.Backward:
                    this._walkBackward = true;
                    break;
                case WalkDirection.Forward:
                    this._walkForward = true;
                    break;
                case WalkDirection.Left:
                    this._walkLeft = true;
                    break;
                case WalkDirection.Right:
                    this._walkRight = true;
                    break;
            }
        }

        public void StopWalk(WalkDirection dir)
        {
            switch (dir)
            {
                case WalkDirection.Backward:
                    this._walkBackward = false;
                    break;
                case WalkDirection.Forward:
                    this._walkForward = false;
                    break;
                case WalkDirection.Left:
                    this._walkLeft = false;
                    break;
                case WalkDirection.Right:
                    this._walkRight = false;
                    break;
            }
        }

        public void Jump()
        {
            this._jumpQueued = true;
        }

        internal MovementControls PrepareForPhysicsTick()
        {
            var controls = new MovementControls {
                Back = this._walkBackward,
                Forward = this._walkForward,
                Right = this._walkRight,
                Left = this._walkLeft,
                Sprint = this._isSprinting,
                Sneak = this._isSneaking,
                Jump = this._jumpQueued
            };

            this._jumpQueued = false;

            return controls;
        }
    }
}
