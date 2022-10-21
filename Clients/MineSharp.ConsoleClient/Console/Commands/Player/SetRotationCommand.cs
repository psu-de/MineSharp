using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Player
{
    internal class SetRotationCommand : Command
    {

        private readonly FloatArgument PitchArg;
        private readonly FloatArgument YawArg;

        public SetRotationCommand()
        {
            this.PitchArg = new FloatArgument("pitch");
            this.YawArg = new FloatArgument("yaw");

            var desc = $"Sets the rotation of the player to [{this.PitchArg.Color}]Pitch / Yaw[/]";
            this.Initialize("setRotation", desc, CColor.PlayerCommand, this.PitchArg, this.YawArg);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {

            var pitch = this.PitchArg.GetValue(argv[0]);
            var yaw = this.YawArg.GetValue(argv[1]);

            if (pitch == null)
            {
                AnsiConsole.MarkupLine($"[{CColor.Error}]Invalid pitch![/]");
            }
            if (yaw == null)
            {
                AnsiConsole.MarkupLine($"[{CColor.Error}]Invalid yaw![/]");
            }

            BotClient.Bot!.ForceSetRotation(yaw!.Value, pitch!.Value);
        }
    }
}
