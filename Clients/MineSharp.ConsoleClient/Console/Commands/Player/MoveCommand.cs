using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;

namespace MineSharp.ConsoleClient.Console.Commands.Player {
    internal class MoveCommand : Command {

        EnumArgument<MoveOptions> DirectionArg;

        public MoveCommand() {
            DirectionArg = new EnumArgument<MoveOptions>("direction");
            var desc = $"Moves the bot in a [{DirectionArg.Color}]direction[/]";
            this.Initialize("move", desc, CColor.PlayerCommand, DirectionArg);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            var dir = DirectionArg.GetValue(argv[0]);
            switch (dir) {
                case MoveOptions.Forward:
                    BotClient.Bot.MovementControls.Forward = true;
                    break;
                case MoveOptions.Backward:
                    BotClient.Bot.MovementControls.Back = true;
                    break;
                case MoveOptions.Left:
                    BotClient.Bot.MovementControls.Left = true;
                    break;
                case MoveOptions.Right:
                    BotClient.Bot.MovementControls.Right = true;
                    break;
                case MoveOptions.Jump:
                    BotClient.Bot.Physics.PlayerState.JumpQueued = true;
                    BotClient.Bot.MovementControls.Jump = true;
                    break;
                case MoveOptions.Reset:
                    BotClient.Bot.MovementControls.Forward = false;
                    BotClient.Bot.MovementControls.Back = false;
                    BotClient.Bot.MovementControls.Right = false;
                    BotClient.Bot.MovementControls.Left = false;
                    BotClient.Bot.MovementControls.Jump = false;
                    break;

            }

        }


        enum MoveOptions {
            Forward,
            Backward,
            Left,
            Right,
            Jump,
            Reset
        }
    }
}
