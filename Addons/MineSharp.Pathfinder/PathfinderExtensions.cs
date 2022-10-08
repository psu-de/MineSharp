using MineSharp.Bot;
using MineSharp.Pathfinding.Goals;

namespace MineSharp.Pathfinding
{
    public static class PathfinderExtensions
    {

        public static Task GoTo(this MinecraftBot bot, Goal goal, Movements? movements = null, double? timeout = null)
        {
            var module = bot.GetModule<PathfinderModule>();
            if (module == null)
            {
                throw new InvalidOperationException(
                    "Pathfinder module not loaded. Use bot.LoadPlugin(new PathfinderModule());");
            }

            return module.GoTo(goal, movements, timeout);
        }

    }
}
