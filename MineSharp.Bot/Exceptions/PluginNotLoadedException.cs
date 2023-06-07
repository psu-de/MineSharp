using MineSharp.Core.Exceptions;

namespace MineSharp.Bot.Exceptions;

public class PluginNotLoadedException : MineSharpException
{

    public PluginNotLoadedException(string message) : base(message)
    { }
}
