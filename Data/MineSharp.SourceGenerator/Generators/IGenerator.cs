using MineSharp.SourceGenerator.Utils;

namespace MineSharp.SourceGenerator.Generators;

public interface IGenerator
{
    string Name { get; }
    Task Run(MinecraftDataWrapper wrapper);
}
