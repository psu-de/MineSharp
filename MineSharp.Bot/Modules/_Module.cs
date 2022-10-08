using MineSharp.Core.Logging;
namespace MineSharp.Bot.Modules
{
    public abstract class Module
    {

        public readonly MinecraftBot Bot;

        protected Logger Logger = Logger.GetLogger();


        public Module(MinecraftBot bot)
        {
            this.Bot = bot;
        }
        public bool IsLoaded { get; private set; }
        public bool IsEnabled {
            get;
            private set;
        }


        protected abstract Task Load();
        protected virtual Task EnablePlugin() => Task.CompletedTask;
        protected virtual Task DisablePlugin() => Task.CompletedTask;

        public async Task Initialize()
        {
            if (this.IsLoaded)
                return;

            await this.Load();

            this.IsLoaded = true;
        }

        public async Task SetEnabled(bool enabled)
        {
            if (this.IsEnabled == enabled) return;

            if (enabled) await this.EnablePlugin();
            else await this.DisablePlugin();

            this.IsEnabled = enabled;
        }
    }
}
