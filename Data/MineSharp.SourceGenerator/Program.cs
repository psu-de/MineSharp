using MineSharp.SourceGenerator.Generators;
using MineSharp.SourceGenerator.Utils;
using Spectre.Console;

AnsiConsole.Write(new FigletText("SourceGenerator").Color(Color.Aqua));

var data = new MinecraftDataWrapper(DirectoryUtils.GetMinecraftDataDirectory());

var generators = new[]
{
    new BiomeGenerator().Run(data), new BlockGenerator().Run(data), new EffectGenerator().Run(data),
    new EnchantmentGenerator().Run(data), new EntityGenerator().Run(data), new ItemGenerator().Run(data),
    new ProtocolGenerator().Run(data), new ParticleGenerator().Run(data)
};

if (Directory.Exists(DirectoryUtils.GetSourceDirectory()))
{
    Directory.Delete(DirectoryUtils.GetSourceDirectory(), true);
}

await Task.WhenAll(generators);


void RecursiveCopy(string source, string target)
{
    foreach (var dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(dirPath.Replace(source, target));
    }

    foreach (var newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
    {
        File.Copy(newPath, newPath.Replace(source, target), true);
    }
}

RecursiveCopy(DirectoryUtils.GetSourceDirectory(), DirectoryUtils.GetMineSharpCoreProjectDirectory());
