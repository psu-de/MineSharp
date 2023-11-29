using MineSharp.Commands.Parser;
using MineSharp.Core.Common;
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
    public readonly int RootIndex;
    public readonly CommandNode[] Nodes;
    
    public CommandNode RootNode => this.Nodes[this.RootIndex];

    public CommandTree(int rootIndex, CommandNode[] nodes)
    {
        this.RootIndex = rootIndex;
        this.Nodes = nodes;
    }
    
    public string[] ExtractRootCommands()
    {
        return this.RootNode
            .Children
            .Select(child => this.Nodes[child].Name)
            .Where(name => name != null)
            .ToArray()!;
    }


    public static CommandTree Parse(PacketBuffer buffer, MinecraftData data)
    {
        int nodeCount = buffer.ReadVarInt();
        var nodes = new CommandNode[nodeCount];

        for (int i = 0; i < nodes.Length; i++)
        {
            byte flags = buffer.ReadByte();
            int childCount = buffer.ReadVarInt();
            int[] children = new int[childCount];
            for (int j = 0; j < childCount; ++j)
                children[j] = buffer.ReadVarInt();

            int redirectNode = ((flags & 0x08) == 0x08) ? buffer.ReadVarInt() : -1;

            string? name = ((flags & 0x03) == 1 || (flags & 0x03) == 2) ? buffer.ReadString() : null;

            int parserId = ((flags & 0x03) == 2) ? buffer.ReadVarInt() : -1;
            IParser? parser = CommandParserFactory.ReadParser(parserId, data, buffer);

            string? suggestionsType = ((flags & 0x10) == 0x10) ? buffer.ReadString() : null;
            nodes[i] = new CommandNode(flags, children, redirectNode, name, parser, suggestionsType);
        }
        var rootIndex = buffer.ReadVarInt();
        return new CommandTree(rootIndex, nodes);
    }
}
