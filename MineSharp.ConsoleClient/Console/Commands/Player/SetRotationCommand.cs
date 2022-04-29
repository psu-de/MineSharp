using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Player {
    internal class SetRotationCommand : Command {

        FloatArgument PitchArg;
        FloatArgument YawArg;

        public SetRotationCommand() {
            PitchArg = new FloatArgument("pitch");
            YawArg = new FloatArgument("yaw");

            var desc = $"Sets the rotation of the player to [{PitchArg.Color}]Pitch / Yaw[/]";
            this.Initialize("setRotation", desc, CColor.PlayerCommand, PitchArg, YawArg);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            float? pitch = PitchArg.GetValue(argv[0]);
            float? yaw = YawArg.GetValue(argv[1]);

            if (pitch == null) {
                AnsiConsole.MarkupLine($"[{CColor.Error}]Invalid pitch![/]");
            }
            if (yaw == null) {
                AnsiConsole.MarkupLine($"[{CColor.Error}]Invalid yaw![/]");
            }

            BotClient.Bot.ForceSetRotation(yaw.Value, pitch.Value, cancellation).GetAwaiter().GetResult();
        }
    }
}
