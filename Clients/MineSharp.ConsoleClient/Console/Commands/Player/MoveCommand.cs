using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;

namespace MineSharp.ConsoleClient.Console.Commands.Player
{
    internal class MoveCommand : Command {

        EnumArgument<MoveOptions> DirectionArg;

        public MoveCommand() {
            DirectionArg = new EnumArgument<MoveOptions>("direction");
            var desc = $"Toggles the bot's movement controls for a [{DirectionArg.Color}]state[/]";
            this.Initialize("move", desc, CColor.PlayerCommand, DirectionArg);
        }

        public override async void DoAction(string[] argv, CancellationToken cancellation) {

            var dir = DirectionArg.GetValue(argv[0]);
            switch (dir) {
                case MoveOptions.Forward:
                    BotClient.Bot!.PlayerControls.IsWalkingForward = !BotClient.Bot.PlayerControls.IsWalkingForward;
                    break;
                case MoveOptions.Backward:
                    BotClient.Bot!.PlayerControls.IsWalkingBackward = !BotClient.Bot.PlayerControls.IsWalkingBackward;
                    break;
                case MoveOptions.Left:
                    BotClient.Bot!.PlayerControls.IsWalkingLeft = !BotClient.Bot.PlayerControls.IsWalkingLeft;
                    break;
                case MoveOptions.Right:
                    BotClient.Bot!.PlayerControls.IsWalkingRight = !BotClient.Bot.PlayerControls.IsWalkingRight;
                    break;
                case MoveOptions.Jump:
                    BotClient.Bot!.PlayerControls.Jump();
                    //BotClient.Bot!.Physics!.PlayerState.JumpQueued = true;
                    //BotClient.Bot.MovementControls.Jump = true;
                    break;
                case MoveOptions.Sprint:
                    if (BotClient.Bot!.PlayerControls.IsSprinting)
                    {
                        await BotClient.Bot!.PlayerControls.StopSprinting(cancellation);
                    } else
                    {
                        await BotClient.Bot!.PlayerControls.StartSprinting(cancellation);
                    }
                    break;
                case MoveOptions.Sneak:
                    if (BotClient.Bot!.PlayerControls.IsSneaking)
                    {
                        await BotClient.Bot!.PlayerControls.StopSneaking(cancellation);
                    } else
                    {
                        await BotClient.Bot!.PlayerControls.StartSneaking(cancellation);
                    }
                    break;
                case MoveOptions.Reset:
                    await BotClient.Bot!.PlayerControls.Reset();
                    break;

            }

        }


        enum MoveOptions {
            Forward,
            Backward,
            Left,
            Right,
            Jump,
            Sprint,
            Sneak,
            Reset
        }
    }
}
