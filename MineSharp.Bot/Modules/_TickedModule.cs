using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot.Modules {
    public abstract class TickedModule : Module {
        protected TickedModule(MinecraftBot bot) : base(bot) {
        }

        public abstract Task Tick();

    }
}
