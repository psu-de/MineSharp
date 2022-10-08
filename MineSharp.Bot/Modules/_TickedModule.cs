namespace MineSharp.Bot.Modules
{
    public abstract class TickedModule : Module
    {
        protected TickedModule(MinecraftBot bot) : base(bot) {}

        public abstract Task Tick();
    }
}
