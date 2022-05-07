


using GenerateData;
using GenerateData.Biomes;
using GenerateData.Blocks;
using GenerateData.Effects;
using GenerateData.Enchantments;
using GenerateData.Items;
using Newtonsoft.Json;

string minecraftVersion = "";
string defaultVersion = "1.18.1";

string version = "2.112.0";
string url = $"https://github.com/PrismarineJS/minecraft-data/archive/refs/tags/{version}.zip";
string path = @"temp-mc-data";
string zipfile = @"MinecraftData.zip";

string outDir = @"out";

void RequestVersion () {
    Console.Write($"Version ({defaultVersion}): ");
    string? newVersion = Console.ReadLine ();
    if (!string.IsNullOrEmpty (newVersion)) {
        minecraftVersion = newVersion;
    } else minecraftVersion = defaultVersion;
}

void DownloadData () {
    Console.WriteLine("Downling Minecraft-Data....");
    Directory.CreateDirectory(path);


    using (var client = new System.Net.Http.HttpClient()) {
        var contents = client.GetByteArrayAsync(url).Result;
        System.IO.File.WriteAllBytes(zipfile, contents);
    }

    Console.WriteLine("Extracting repository");
    System.IO.Compression.ZipFile.ExtractToDirectory(zipfile, path);
}




void Main () {

    Console.WriteLine("Starting Generator");

    RequestVersion();

    string dataPath = Path.Join(path, $"minecraft-data-{version}", "data");
    if (!Directory.Exists(dataPath)) {
        Directory.Delete(path, true);
        DownloadData();
    }
    
    DataPaths dataPaths = JsonConvert.DeserializeObject<DataPaths>(File.ReadAllText(Path.Join(dataPath, "dataPaths.json")));


    if (Directory.Exists(outDir)) Directory.Delete(outDir, true);
    Directory.CreateDirectory(outDir);


    BlockData.Generate(Path.Join(dataPath, dataPaths.PCVersions[minecraftVersion].BlocksPath, "blocks.json"), outDir);
    BlockShapeData.Generate(Path.Join(dataPath, dataPaths.PCVersions[minecraftVersion].BlockCollisionShapesPath, "blockCollisionShapes.json"), outDir);
    BiomeData.Generate(Path.Join(dataPath, dataPaths.PCVersions[minecraftVersion].BiomesPath, "biomes.json"), outDir);
    ItemData.Generate (Path.Join(dataPath, dataPaths.PCVersions[minecraftVersion].ItemsPath, "items.json"), outDir);
    EffectData.Generate(Path.Join(dataPath, dataPaths.PCVersions[minecraftVersion].EffectsPath, "effects.json"), outDir);
    EnchantmentData.Generate(Path.Join(dataPath, dataPaths.PCVersions[minecraftVersion].EnchantmentsPath, "enchantments.json"), outDir);
}




Main();