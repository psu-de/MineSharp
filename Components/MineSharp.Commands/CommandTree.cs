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
}
