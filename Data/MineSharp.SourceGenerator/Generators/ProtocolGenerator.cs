using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MineSharp.SourceGenerator.Generators;

public class ProtocolGenerator : IGenerator
{
    public string Name => "Protocol";

    public async Task Run(MinecraftDataWrapper wrapper)
    {
        var packets = new Dictionary<string, int>();
        
        foreach (var version in Config.IncludedVersions)
        {
            var protocol = await wrapper.GetProtocol(version);

            foreach (var ns in ((JObject)protocol).Properties().Select(x => x.Name))
            {
                if (ns == "types")
                    continue;

                var state = Enum.Parse<GameState>(ns.Pascalize());
                foreach (var val in CollectPackets(protocol, ns, "toClient"))
                    packets.TryAdd(val, GetPacketValue(packets.Count, state, PacketFlow.Clientbound));
                
                foreach (var val in CollectPackets(protocol, ns, "toServer"))
                    packets.TryAdd(val, GetPacketValue(packets.Count, state, PacketFlow.Serverbound));
            }

            await GenerateVersion(wrapper, version);
        }
        
        await new EnumGenerator() {
            ClassName = "PacketType",
            Namespace = "MineSharp.Data.Protocol",
            Outfile = Path.Join(DirectoryUtils.GetDataSourceDirectory("Protocol"), "PacketType.cs"),
            Entries = packets
        }.Write();
    }

    private int GetPacketValue(int id, GameState state, PacketFlow flow)
    {
        return id & 0xFF | (byte)((sbyte)state & 0xFF) << 8 | (byte)flow << 16;
    }
    
    public async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        var path = wrapper.GetPath(version, "protocol");
        if (VersionMapGenerator.GetInstance().IsRegistered("protocol", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("protocol", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("protocol", version, path);
        
        var v = version.Replace(".", "_");
        var outdir = DirectoryUtils.GetDataSourceDirectory("Protocol\\Versions");
        var protocol = await wrapper.GetProtocol(version);
        
        var writer = new CodeWriter();
        writer.Disclaimer();
        writer.WriteLine("namespace MineSharp.Data.Protocol.Versions;");
        writer.WriteLine();
        writer.Begin($"internal class Protocol_{v} : ProtocolVersion");
        writer.Begin("public override Dictionary<PacketType, int> PacketIds { get; } = new()");
        foreach (var ns in ((JObject)protocol).Properties().Select(x => x.Name))
        {
            if (ns == "types")
                continue;
        
            var cbIdMapping = ((JObject)protocol.SelectToken($"{ns}.toClient.types.packet[1][0].type[1].mappings")!)
                .Properties()
                .ToDictionary(x => this.GetPacketName((string)x.Value!, "toClient", ns), x => x.Name);
        
            var sbIdMapping = ((JObject)protocol.SelectToken($"{ns}.toServer.types.packet[1][0].type[1].mappings")!)
                .Properties()
                .ToDictionary(x => this.GetPacketName((string)x.Value!, "toServer", ns), x => x.Name);
            
            foreach (var kvp in cbIdMapping)
            {
                var packetName = kvp.Key;
                writer.WriteLine($"{{ PacketType.{packetName}, {kvp.Value} }},");
            }
            
            foreach (var kvp in sbIdMapping)
            {
                var packetName = kvp.Key;
                writer.WriteLine($"{{ PacketType.{packetName}, {kvp.Value} }},");
            }
        }
        writer.Finish(semicolon: true);
        writer.Finish();

        await File.WriteAllTextAsync(Path.Join(outdir, $"Protocol_{v}.cs"), writer.ToString());
    }

    private string[] CollectPackets(JToken token, string @namespace, string direction)
    {
        var obj = (JObject)token.SelectToken($"{@namespace}.{direction}.types.packet[1][0].type[1].mappings")!;

        return obj.Properties()
            .Select(x => (string)x.Value!)
            .Select(x => GetPacketName(x, direction, @namespace))
            .ToArray();
    }

    private string GetPacketName(string name, string direction, string ns)
    {
        direction = direction == "toClient" ? "CB" : "SB";
        ns = ns == "handshaking" ? "Handshake" : ns.Pascalize();
        name = name.Pascalize()
            .Replace("Packet", "")
            .Replace("ConfiguationAcknowledged", "ConfigurationAcknowledged");

        return $"{direction}_{ns}_{name}";
    }
}

public enum GameState
{
    Handshaking = 0,
    Status = 1,
    Login = 2,
    Play = 3,
    Configuration = 4,
}


enum PacketFlow
{
    Clientbound,
    Serverbound
}