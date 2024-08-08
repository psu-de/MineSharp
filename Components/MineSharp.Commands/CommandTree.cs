using MineSharp.Commands.Parser;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands;

/*
 * Thanks to Minecraft-Console-Client
 * https://github.com/MCCTeam/Minecraft-Console-Client
 *
 * This Class uses a lot of code from MinecraftClient/Protocol/Handlers/Packet/s2c/DeclareCommands.cs from MCC.
 */

public class CommandTree
{
    public readonly CommandNode[] Nodes;
    public readonly int RootIndex;

    public CommandTree(int rootIndex, CommandNode[] nodes)
    {
        RootIndex = rootIndex;
        Nodes = nodes;
    }

    public CommandNode RootNode => Nodes[RootIndex];

    public string[] ExtractRootCommands()
    {
        return RootNode
              .Children
              .Select(child => Nodes[child].Name)
              .Where(name => name != null)
              .ToArray()!;
    }


    public static CommandTree Parse(PacketBuffer buffer, MinecraftData data)
    {
        var nodeCount = buffer.ReadVarInt();
        var nodes = new CommandNode[nodeCount];

        for (var i = 0; i < nodes.Length; i++)
        {
            nodes[i] = ReadNode(buffer, data);
        }

        var rootIndex = buffer.ReadVarInt();
        return new(rootIndex, nodes);
    }

    private static CommandNode ReadNode(PacketBuffer buffer, MinecraftData data)
    {
        var flags = buffer.ReadByte();
        var childCount = buffer.ReadVarInt();
        var children = new int[childCount];
        for (var j = 0; j < childCount; j++)
        {
            children[j] = buffer.ReadVarInt();
        }

        var redirectNode = (flags & 0x08) != 0 ? buffer.ReadVarInt() : -1;
        var name = (flags & 0x03) != 0 ? buffer.ReadString() : null;
        var parser = ReadParser(flags, buffer, data);
        var suggestionsType = (flags & 0x10) != 0 ? buffer.ReadString() : null;

        return new(flags, children, redirectNode, name, parser, suggestionsType);
    }

    private static IParser? ReadParser(byte flags, PacketBuffer buffer, MinecraftData data)
    {
        if ((flags & 0x02) == 0)
        {
            return null;
        }

        Identifier name;

        if (data.Version.Protocol < ProtocolVersion.V_1_19)
        {
            // in 1.18.x, the parser was specified by its name.
            name = buffer.ReadIdentifier();
        }
        else
        {
            var parserId = buffer.ReadVarInt();
            name = ParserRegistry.GetParserNameById(parserId, data);
        }

        var parser = ParserRegistry.GetParserByName(name);
        if ((flags & 0x02) != 0)
        {
            parser.ReadProperties(buffer, data);
        }

        return parser;
    }
}
