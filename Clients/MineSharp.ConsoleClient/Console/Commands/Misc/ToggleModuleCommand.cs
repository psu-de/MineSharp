using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;
namespace MineSharp.ConsoleClient.Console.Commands.Misc
{
    internal class ToggleModuleCommand : Command
    {
        private readonly BoolArgument EnabledArg = new BoolArgument("enabled");

        private readonly StringArgument ModuleArg = new StringArgument("module", false, BotClient.Bot!.Modules.Select(x => x.GetType().Name).ToArray());

        public ToggleModuleCommand()
        {
            var desc = "Enables or disables the Physics engine";
            this.Initialize("toggleModule", desc, CColor.MiscCommand, this.ModuleArg, this.EnabledArg);

        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {
            var module = BotClient.Bot!.Modules.FirstOrDefault(x => x.GetType().Name == this.ModuleArg.GetValue(argv[0]));
            if (module == null)
            {
                AnsiConsole.MarkupLine($"[red]Module {this.ModuleArg.GetValue(argv[0])} not found [/]");
                return;
            }
            var enabled = this.EnabledArg.GetValue(argv[1]);
            module.SetEnabled(enabled).GetAwaiter().GetResult();
        }
    }
}
