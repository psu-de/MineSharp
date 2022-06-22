namespace MineSharp.ConsoleClient.Console.Commands.Prompt {
    internal class ExitCommand : Command {

        public ExitCommand() {
            var desc = $"Exits the prompt.";
            this.Initialize("exit", desc, CColor.PromptCommand, new Commands.Arguments.Argument[] { });
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {
            Environment.Exit(0);
        }
    }
}
