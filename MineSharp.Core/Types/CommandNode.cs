namespace MineSharp.Core.Types {
    public class CommandNode {

        public byte Flags { get; private set; }
        public int[] Children { get; private set; }
        public int? RedirectNode { get; private set; }
        public string? Name { get; private set; }
        public Identifier? Parser { get; private set; }
        public object? Properties { get; private set; }
        public Identifier? SuggestionsType { get; private set; }

        public CommandNode(byte flags, int[] children, int? redirectNode, string? name, Identifier? parser, object? properties, Identifier? suggestionsType) {
            Flags = flags;
            Children = children;
            RedirectNode = redirectNode;
            Name = name;
            Parser = parser;
            Properties = properties;
            SuggestionsType = suggestionsType;
        }
    }
}
