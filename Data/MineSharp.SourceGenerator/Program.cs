using MineSharp.SourceGenerator.Generators;
using MineSharp.SourceGenerator.Utils;
using Spectre.Console;

AnsiConsole.Write(new FigletText("SourceGenerator").Color(Color.Aqua));

var data = new MinecraftDataWrapper(DirectoryUtils.GetMinecraftDataDirectory());

var generators = new IGenerator[] {
    new BiomeGenerator(),
    new BlockGenerator(),
    new BlockCollisionShapesGenerator(),
    new EffectGenerator(),
    new EnchantmentGenerator(),
    new EntityGenerator(),
    new ItemGenerator(),
    new ProtocolGenerator(),
    new MaterialGenerator(),
    
    VersionMapGenerator.GetInstance()
};

if (Directory.Exists(DirectoryUtils.GetDataSourceDirectory()))
    Directory.Delete(DirectoryUtils.GetDataSourceDirectory(), true);

if (Directory.Exists(DirectoryUtils.GetCoreSourceDirectory()))
    Directory.Delete(DirectoryUtils.GetCoreSourceDirectory(), true);

foreach (var generator in generators)
{
    await AnsiConsole.Status()
        .StartAsync($" Generating {generator.Name}...", async ctx =>
        {
            await generator.Run(data);
            AnsiConsole.MarkupLine($" ✅ Generated {generator.Name}");
        });
}

void RecursiveCopy(string source, string target)
{
    foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(dirPath.Replace(source, target));
    }

    foreach (string newPath in Directory.GetFiles(source, "*.*",SearchOption.AllDirectories))
    {
        File.Copy(newPath, newPath.Replace(source, target), true);
    }
}

RecursiveCopy(DirectoryUtils.GetDataSourceDirectory(), DirectoryUtils.GetMineSharpDataProjectDirectory());
RecursiveCopy(DirectoryUtils.GetCoreSourceDirectory(), DirectoryUtils.GetMineSharpCoreProjectDirectory());