using MineSharp.Data.Generator;
using MineSharp.Data.Generator.Biomes;
using MineSharp.Data.Generator.Blocks;
using MineSharp.Data.Generator.Effects;
using MineSharp.Data.Generator.Enchantments;
using MineSharp.Data.Generator.Entities;
using MineSharp.Data.Generator.Items;
using MineSharp.Data.Generator.Protocol;
using Spectre.Console;
using System.Reflection;

string projectPath = "";
try {
    projectPath = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent!.Parent!.Parent!.Parent!
        .FullName;
} catch {
    AnsiConsole.MarkupLine("[red]Please execute this program from the Project folder[/]");
    Environment.Exit(0);
}

string outputProjPath = Path.Join(projectPath, "..", "MineSharp.Data");

string minecraftDataPath = Path.Join(projectPath, "minecraft-data");

var dataHelper = new MinecraftDataHelper(minecraftDataPath);

AnsiConsole.Write(new Rule("MineSharp.Data Generator"));
string version = AnsiConsole.Ask<string>("Minecraft Version: ", "1.18.1");
if (!dataHelper.GetAvailableVersions().Contains(version)) {
    AnsiConsole.MarkupLine($"[red]Version {version} not supported![/]");
    AnsiConsole.MarkupLine($"[red]Available versions: [/]");
    foreach (var availableVersion in dataHelper.GetAvailableVersions().GroupBy(x => (!x.Contains(".") ? "Snapshots" : string.Join(".", x.Split(".").Take(2)))))
        AnsiConsole.MarkupLine($"[red]  - {availableVersion.Key} ({string.Join(", ", availableVersion)})[/]");
    Environment.Exit(0);
}

Dictionary<string, Generator> dataGenerators = new Dictionary<string, Generator>() {
    { "Biomes", new BiomeGenerator(dataHelper, version) },
    { "Blocks", new BlockGenerator(dataHelper, version) },
    { "Effects", new EffectGenerator(dataHelper, version) },
    { "Enchantments", new EnchantmentGenerator(dataHelper, version) },
    { "Entities", new EntityGenerator(dataHelper, version) },
    { "Items", new ItemGenerator(dataHelper, version) },
    { "Protocol", new ProtocolGenerator(dataHelper, version) },
    { "MinecraftData", new MinecraftDataGenerator(dataHelper, version) }
};

foreach (var dataGen in dataGenerators) {
    CodeGenerator codeGenerator = new CodeGenerator();
    dataGen.Value.WriteCode(codeGenerator);

    string outputPath = Path.Join(outputProjPath, dataGen.Key + ".cs");
    File.WriteAllText(outputPath, codeGenerator.ToString());
}