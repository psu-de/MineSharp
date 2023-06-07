namespace MineSharp.Commands.Parser;

public class StringParser : IParser
{
    public readonly StringType Type;
    
    public StringParser(StringType type)
    {
        this.Type = type;
    }
    
    public string GetName() => "brigadier:string";
    public int GetArgumentCount() => 1;
}

public enum StringType
{
    SINGLE_WORD, 
    QUOTABLE_PHRASE, 
    GREEDY_PHRASE
}