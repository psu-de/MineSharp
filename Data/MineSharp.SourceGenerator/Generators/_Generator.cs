using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class Generator(string dataKey, Func<JToken, string> selector, string className, string ns)
{
    public async Task Generate(MinecraftDataWrapper wrapper)
    {
        var set = new HashSet<string>();

        foreach (var version in Config.IncludedVersions)
        {
            var array = (JArray)await wrapper.Parse(version, dataKey);
            foreach (var token in array)
            {
                var name = selector(token);
                // TODO: add minecraft tags issue #62
                if (name.ToLower().StartsWith("tagkey"))
                    continue;
                
                set.Add(name);
            }
        }

        var outDir  = DirectoryUtils.GetSourceDirectory(Path.Join("Common", ns));
        var counter = 0;
        await new EnumGenerator()
        {
            ClassName = className,
            Namespace = $"MineSharp.Core.Common.{ns}",
            Outfile   = Path.Join(outDir, className + ".cs"),
            Entries   = set.ToDictionary(x => x, _ => counter++)
        }.Write();
    }
}
