using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class ProtocolGenerator
{
    public async Task Run(MinecraftDataWrapper wrapper)
    {
        var set = new HashSet<string>();

        foreach (var version in Config.IncludedVersions)
        {
            var obj = (JObject)await wrapper.GetProtocol(version);
            AddPackets(obj, ref set);
        }
    }

    private void AddPackets(JObject protocol, ref HashSet<string> set)
    {
        foreach (var ns in protocol.Properties())
        {
            if (ns.Name == "types")
                continue;

            foreach (var packet in CollectPackets(protocol, ns.Name, "toClient"))
            {
                set.Add(packet);
            }

            foreach (var packet in CollectPackets(protocol, ns.Name, "toServer"))
            {
                set.Add(packet);
            }
        }
    }

    private IEnumerable<string> CollectPackets(JObject protocol, string ns, string direction)
    {
        var obj = (JObject)protocol.SelectToken($"{ns}.{direction}.types.packet[1][0].type[1].mappings")!;

        foreach (var prop in obj.Properties())
        {
            yield return NameUtils.GetPacketName((string)prop.Value!, direction, ns);
        }
    }
}
