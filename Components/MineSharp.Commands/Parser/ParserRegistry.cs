using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

internal static class ParserRegistry
{
    // registry available since snapshot 22w11a (after 1.18.2)
    private static readonly Identifier ArgumentParserRegistry = "minecraft:command_argument_type";
        
    public static Identifier GetParserNameById(int parserId, MinecraftData data)
    {
        return data.Registries[ArgumentParserRegistry].ByProtocolId(parserId);
    }

    public static IParser GetParserByName(Identifier name)
    {
        return name.ToString() switch
        {
            "brigadier:bool" => new EmptyParser(),
            "brigadier:float" => new FloatParser(),
            "brigadier:double" => new DoubleParser(),
            "brigadier:integer" => new IntegerParser(),
            "brigadier:long" => new LongParser(),
            "brigadier:string" => new StringParser(),
            "minecraft:entity" => new EntityParser(),
            "minecraft:game_profile" => new EmptyParser(),
            "minecraft:block_pos" => new BlockPositionParser(),
            "minecraft:column_pos" => new ColumnPosParser(),
            "minecraft:vec3" => new Vec3Parser(),
            "minecraft:vec2" => new Vec2Parser(),
            "minecraft:block_state" => new EmptyParser(),
            "minecraft:block_predicate" => new EmptyParser(),
            "minecraft:item_stack" => new EmptyParser(),
            "minecraft:item_predicate" => new EmptyParser(),
            "minecraft:color" => new EmptyParser(),
            "minecraft:component" => new EmptyParser(),
            "minecraft:message" => new MessageParser(),
            "minecraft:nbt" => new EmptyParser(),
            "minecraft:nbt_tag" => new EmptyParser(),
            "minecraft:nbt_path" => new EmptyParser(),
            "minecraft:objective" => new EmptyParser(),
            "minecraft:objective_criteria" => new EmptyParser(),
            "minecraft:operation" => new EmptyParser(),
            "minecraft:particle" => new EmptyParser(),
            "minecraft:angle" => new EmptyParser(),
            "minecraft:rotation" => new RotationParser(),
            "minecraft:scoreboard_slot" => new EmptyParser(),
            "minecraft:score_holder" => new ScoreHolderParser(),
            "minecraft:swizzle" => new EmptyParser(),
            "minecraft:team" => new EmptyParser(),
            "minecraft:item_slot" => new EmptyParser(),
            "minecraft:resource_location" => new EmptyParser(),
            "minecraft:mob_effect" => new EmptyParser(),
            "minecraft:function" => new EmptyParser(),
            "minecraft:entity_anchor" => new EmptyParser(),
            "minecraft:int_range" => new EmptyParser(),
            "minecraft:float_range" => new EmptyParser(),
            "minecraft:item_enchantment" => new EmptyParser(),
            "minecraft:entity_summon" => new EmptyParser(),
            "minecraft:dimension" => new EmptyParser(),
            "minecraft:time" => new TimeParser(),
            "minecraft:resource_or_tag" => new ResourceOrTagParser(),
            "minecraft:resource" => new ResourceParser(),
            "minecraft:template_mirror" => new EmptyParser(),
            "minecraft:template_rotation" => new EmptyParser(),
            "minecraft:uuid" => new EmptyParser(),
            "minecraft:gamemode" => new EmptyParser(),
            "minecraft:resource_or_tag_key" => new ResourceOrTagParser(),
            "minecraft:resource_key" => new ResourceParser(),
            "minecraft:heightmap" => new EmptyParser(),
            "minecraft:range" => new RangeParser(), // only in 1.18
            "minecraft:nbt_compound_tag" => new EmptyParser(),
            "minecraft:style" => new EmptyParser(), // since 1.20.3
            _ => throw new ArgumentException($"Unknown parser: {name}")
        };
    }
}
