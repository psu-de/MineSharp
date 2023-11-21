using MineSharp.Commands.Parser;
using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands;

public static class CommandParserFactory
{
    public delegate IParser ParserFactory(PacketBuffer buffer);
    
    private static ParserFactory ReadFloatParser = buffer =>
    {
        var flags = buffer.ReadByte();
        var min = (flags & 0x01) > 0 ? buffer.ReadFloat() : float.MinValue;
        var max = (flags & 0x02) > 0 ? buffer.ReadFloat() : float.MaxValue;
        return new FloatParser(min, max);
    };
    
    private static ParserFactory ReadDoubleParser = buffer =>
    {
        var flags = buffer.ReadByte();
        var min = (flags & 0x01) > 0 ? buffer.ReadDouble() : double.MinValue;
        var max = (flags & 0x02) > 0 ? buffer.ReadDouble() : double.MaxValue;
        return new DoubleParser(min, max);
    };
    
    private static ParserFactory ReadIntegerParser = buffer =>
    {
        var flags = buffer.ReadByte();
        var min = (flags & 0x01) > 0 ? buffer.ReadInt() : int.MinValue;
        var max = (flags & 0x02) > 0 ? buffer.ReadInt() : int.MaxValue;
        return new IntegerParser(min, max);
    };
    
    private static ParserFactory ReadLongParser = buffer =>
    {
        var flags = buffer.ReadByte();
        var min = (flags & 0x01) > 0 ? buffer.ReadLong() : long.MinValue;
        var max = (flags & 0x02) > 0 ? buffer.ReadLong() : long.MaxValue;
        return new LongParser(min, max);
    };

    private static ParserFactory ReadStringParser = buffer 
        => new StringParser((StringType)buffer.ReadVarInt());

    private static ParserFactory ReadEntityParser = buffer
        => new EntityParser(buffer.ReadByte());

    private static ParserFactory ReadBlockPositionParser = buffer
        => new BlockPositionParser();
    
    private static ParserFactory ReadColumnPositionParser = buffer
        => new ColumnPosParser();

    private static ParserFactory ReadVector3Parser = buffer
        => new Vec3Parser();
    
    private static ParserFactory ReadVector2Parser = buffer
        => new Vec2Parser();

    private static ParserFactory ReadRotationParser = buffer
        => new RotationParser();

    private static ParserFactory ReadMessageParser = buffer
        => new MessageParser();

    private static ParserFactory ReadScoreHolderParser = buffer
        => new ScoreHolderParser(buffer.ReadByte());
    
    private static ParserFactory ReadRangeParser = buffer
        => new RangeParser(buffer.ReadBool());
    
    private static ParserFactory ReadResourceOrTagParser = buffer
        => new ResourceOrTagParser(buffer.ReadString());

    private static ParserFactory ReadResourceParser = buffer
        => new ResourceParser(buffer.ReadString());

    private static ParserFactory ReadTimeParser = buffer
        => new TimeParser(buffer.ReadInt());

    private static ParserFactory ReadEmptyParser = buffer 
        => new EmptyParser(); 

    private static Dictionary<int, ParserFactory> Mapping_1_19_2 = new Dictionary<int, ParserFactory>()
    {
        { 1, ReadFloatParser },
        { 2, ReadDoubleParser },
        { 3, ReadIntegerParser }, 
        { 4, ReadLongParser },
        { 5, ReadStringParser },
        { 6, ReadEntityParser },
        { 8, ReadBlockPositionParser },
        { 9, ReadColumnPositionParser },
        { 10, ReadVector3Parser },
        { 11, ReadVector2Parser },
        { 18, ReadMessageParser },
        { 27, ReadRotationParser },
        { 29, ReadScoreHolderParser },
        { 43, ReadResourceOrTagParser },
        { 44, ReadResourceParser },
    };
    
    private static Dictionary<int, ParserFactory> Mapping_1_19_3 = new Dictionary<int, ParserFactory>()
    {
        { 1, ReadFloatParser },
        { 2, ReadDoubleParser },
        { 3, ReadIntegerParser },
        { 4, ReadLongParser },
        { 5, ReadStringParser },
        { 6, ReadEntityParser },
        { 8, ReadBlockPositionParser },
        { 9, ReadColumnPositionParser },
        { 10, ReadVector3Parser },
        { 11, ReadVector2Parser },
        { 18, ReadMessageParser },
        { 27, ReadRotationParser },
        { 29, ReadScoreHolderParser },
        { 41, ReadResourceOrTagParser },
        { 42, ReadResourceOrTagParser },
        { 43, ReadResourceParser },
        { 44, ReadResourceParser },
    };
    
    private static Dictionary<int, ParserFactory> Mapping_1_19_4 = new Dictionary<int, ParserFactory>() 
    {
        { 1, ReadFloatParser },
        { 2, ReadDoubleParser },
        { 3, ReadIntegerParser },
        { 4, ReadLongParser },
        { 5, ReadStringParser },
        { 6, ReadEntityParser },
        { 8, ReadBlockPositionParser },
        { 9, ReadColumnPositionParser },
        { 10, ReadVector3Parser },
        { 11, ReadVector2Parser },
        { 18, ReadMessageParser },
        { 27, ReadRotationParser },
        { 29, ReadScoreHolderParser },
        { 40, ReadTimeParser },
        { 41, ReadResourceOrTagParser },
        { 42, ReadResourceOrTagParser },
        { 43, ReadResourceParser },
        { 44, ReadResourceParser },  
    };

    private static readonly MinecraftVersion V1_19_3 = new MinecraftVersion("1.19.3", -1);
    private static readonly MinecraftVersion V1_19_4 = new MinecraftVersion("1.19.4", -1);
    
    public static IParser ReadParser(int parserId, MinecraftData data, PacketBuffer buffer)
    {
        var mapping = Mapping_1_19_2;
        
        if (data.Version >= V1_19_3)
            mapping = Mapping_1_19_3;

        if (data.Version >= V1_19_4)
            mapping = Mapping_1_19_4;

        if (mapping.TryGetValue(parserId, out var factory))
        {
            return factory(buffer);
        }
        
        return ReadEmptyParser(buffer);
    }
}
