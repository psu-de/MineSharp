using MineSharp.Commands.Parser;

namespace MineSharp.Commands;

/*
 * Thanks to Minecraft-Console-Client
 * https://github.com/MCCTeam/Minecraft-Console-Client
 * 
 * This Class uses a lot of code from MinecraftClient/Protocol/Handlers/Packet/s2c/DeclareCommands.cs from MCC.
 */
public class CommandNode
{
    public byte Flags { get; }
    public int[] Children { get; }
    public int RedirectNode { get; }
    public string? Name { get; }
    public IParser? Parser { get; }
    public string? SuggestionsType { get; }
    
    public CommandNode(byte flags, int[] children, int redirectNode = -1, string? name = null, IParser? parser = null, string? suggestionsType = null) {
        this.Flags = flags;
        this.Children = children;
        this.RedirectNode = redirectNode;
        this.Name = name;
        this.Parser = parser;
        this.SuggestionsType = suggestionsType;
    }
}
