using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators.Core;

public abstract class CommonGenerator : IGenerator
{
    public string Name => this.Singular;
    protected abstract string DataKey { get; }
    protected abstract string Namespace { get; }
    protected abstract string Singular { get; }
    protected virtual string[] ExtraUsings { get; } = Array.Empty<string>();

    public async Task Run(MinecraftDataWrapper wrapper)
    {
        await this.GenerateTypeEnum(wrapper);

        foreach (var version in Config.IncludedVersions)
        {
            await GenerateVersion(wrapper, version);
        }

        await this.WriteAdditionalItems(wrapper);
    }

    protected virtual async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        var path = wrapper.GetPath(version, this.DataKey);
        var versionMap = VersionMapGenerator.GetInstance();
        if (versionMap.IsRegistered(this.DataKey, path))
        {
            versionMap.RegisterVersion(this.DataKey, version, path);
            return;
        }
        versionMap.RegisterVersion(this.DataKey, version, path);

        var outdir = DirectoryUtils.GetDataSourceDirectory($"{this.Namespace}\\Versions");
        var v = path.Replace("pc/", "").Replace(".", "_");
        var data = await wrapper.Parse(version, this.DataKey);
        
        await new DataVersionGenerator() {
            Namespace = $"MineSharp.Data.{this.Namespace}.Versions",
            ClassName = $"{this.Namespace}_{v}",
            EnumName = $"{this.Singular}Type",
            InfoClass = $"{this.Singular}Info",
            Usings = new[] 
                { $"MineSharp.Core.Common.{this.Namespace}" }
                .Concat(this.ExtraUsings)
                .ToArray(),
            Outfile = Path.Join(outdir, $"{this.Namespace}_{v}.cs"),
            Properties = this.GetProperties(data),
            Stringify = this.Stringify,
            KeySelector = this.GetName
        }.Write();
    }

    private async Task GenerateTypeEnum(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory($"Common\\{this.Namespace}");
        var data = await wrapper.Parse(Config.LatestVersion, this.DataKey);

        var values = this.GetProperties(data)
            .ToDictionary(
                this.GetName, 
                value => (int)value.SelectToken("id")!);

        await new EnumGenerator() {
            Namespace = $"MineSharp.Core.Common.{Namespace}",
            ClassName = $"{this.Singular}Type",
            Outfile = Path.Join(outdir, $"{this.Singular}Type.cs"),
            Entries = values
        }.Write();
    }

    protected virtual Task WriteAdditionalItems(MinecraftDataWrapper wrapper) 
        => Task.CompletedTask;

    protected abstract JToken[] GetProperties(JToken data);
    protected abstract string GetName(JToken token);
    protected abstract string Stringify(JToken token);
}
