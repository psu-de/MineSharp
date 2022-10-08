using MineSharp.Core.Logging;

namespace MineSharp.Bot.Modules
{
    public abstract class Module
    {
        protected Logger Logger = Logger.GetLogger();
        public bool IsLoaded { get; private set; } = false;
        public bool IsEnabled => this._enabled;

        private bool _enabled = false;

        public readonly MinecraftBot Bot;


        public Module(MinecraftBot bot)
        {
            this.Bot = bot;
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

            this._enabled = enabled;
        }
    }
}
