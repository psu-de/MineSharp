using System.Globalization;
using System.Text;

namespace MineSharp.Data.Generator.Blocks {
    internal class BlockGenerator : Generator {
        public BlockGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {
        }

        public string[] GetUsings() {
            return new[] { "MineSharp.Core.Types", "System.Collections.Generic" };
        }

        public override void WriteCode(CodeGenerator codeGenerator) {

            var blockData = Wrapper.LoadJson<BlockJsonInfo[]>(Version, "blocks");
            var blockCollisionData = Wrapper.LoadJson<BlockCollisionShapeJson>(Version, "blockCollisionShapes");

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            codeGenerator.CommentBlock($"Generated Block Data for Minecraft Version {Version}");

            foreach (var ns in GetUsings())
                codeGenerator.WriteLine($"using {ns};");

            codeGenerator.Begin("namespace MineSharp.Data.Blocks");
            codeGenerator.Begin("public static class BlockShapePalette");
            codeGenerator.Begin("public static readonly Dictionary<int, float[][]> AllBlockShapes = new Dictionary<int, float[][]>()");

            foreach (var shape in blockCollisionData.Shapes)
                codeGenerator.WriteLine($"{{ {shape.Key}, new float[][] {{ {string.Join(", ", shape.Value.Select(x => $"new float[] {{ {string.Join(", ", x.Select(x => x.ToString(nfi) + "F"))} }}"))} }} }},");

            codeGenerator.Finish(semicolon: true);
            codeGenerator.Finish();

            codeGenerator.Begin("public static class BlockPalette");
            codeGenerator.Begin("public static int GetBlockIdByState(int state) => state switch");
            foreach (var block in blockData)
                codeGenerator.WriteLine($"(>= {block.MinStateId}) and (<= {block.MaxStateId}) => {block.Id},");
            codeGenerator.WriteLine($"_ => throw new ArgumentException($\"Block with state {{state}} not found!\")");
            codeGenerator.Finish(semicolon: true);

            codeGenerator.Begin("public static Type GetBlockTypeById(int id) => id switch");
            foreach (var block in blockData)
                codeGenerator.WriteLine($"{block.Id} => typeof({Wrapper.GetCSharpName(block.Name)}),");
            codeGenerator.WriteLine($"_ => throw new ArgumentException($\"Block with id {{id}} not found!\")");
            codeGenerator.Finish(semicolon: true);
            codeGenerator.Finish();

            foreach (var block in blockData) {

                codeGenerator.Begin($"public class {Wrapper.GetCSharpName(block.Name)} : Block");
                codeGenerator.WriteBlock($@"
public const int BlockId = {block.Id};
public const string BlockName = ""{block.Name}"";
public const string BlockDisplayName = ""{block.DisplayName}"";

public const float BlockHardness = {(block.Hardness ?? float.MaxValue).ToString(nfi)}F;
public const float BlockResistance = {block.Resistance!.Value.ToString(nfi)}F;
public const bool BlockDiggable = {block.Diggable.ToString().ToLower()};
public const bool BlockTransparent = {block.Transparent.ToString().ToLower()};
public const int BlockFilterLight = {block.FilterLight};
public const int BlockEmitLight = {block.EmitLight};
public const string BlockBoundingBox = ""{block.BoundingBox}"";
public const int BlockStackSize = {block.StackSize};
public const string BlockMaterial = ""{block.Material}"";
public const int BlockDefaultState = {block.DefaultState};
public const int BlockMinStateId = {block.MinStateId};
public const int BlockMaxStateId = {block.MaxStateId};
public static readonly int[]? BlockHarvestTools = {(block.HarvestTools == null ? "null" : $"new int[] {{ {string.Join(", ", block.HarvestTools.Keys)} }}")};
internal static readonly int[] BlockShapeIndices = new int[] {{ {string.Join(", ", BlockCollisionShapeJson.GetShapeIndices(blockCollisionData.Blocks[block.Name])) } }};
public static readonly BlockProperties BlockProperties = new BlockProperties(new BlockStateProperty[] {{ {string.Join("", block.States!.Select(x => GetProperty(x)))} }}, {block.DefaultState - block.MinStateId});

public {Wrapper.GetCSharpName(block.Name)}() : base(BlockId, BlockName, BlockDisplayName, BlockHardness, BlockResistance, BlockDiggable, BlockTransparent, BlockFilterLight, BlockEmitLight, BlockBoundingBox, BlockStackSize, BlockMaterial, BlockDefaultState, BlockMinStateId, BlockMaxStateId, BlockHarvestTools, BlockProperties) {{}} 

public {Wrapper.GetCSharpName(block.Name)}(int state, Position pos) : base(state, pos, BlockId, BlockName, BlockDisplayName, BlockHardness, BlockResistance, BlockDiggable, BlockTransparent, BlockFilterLight, BlockEmitLight, BlockBoundingBox, BlockStackSize, BlockMaterial, BlockDefaultState, BlockMinStateId, BlockMaxStateId, BlockHarvestTools, BlockProperties) {{}}");
                codeGenerator.Finish();

            }

            codeGenerator.Begin("public enum BlockType");
            foreach (var block in blockData)
                codeGenerator.WriteLine($"{Wrapper.GetCSharpName(block.Name)} = {block.Id},");
            codeGenerator.Finish();
            codeGenerator.Finish();
        }

        private string GetProperty(BlockStateJsonInfo info) {
            StringBuilder sb = new StringBuilder();

            sb.Append($"new BlockStateProperty(\"{info.Name}\", BlockStateProperty.BlockStatePropertyType.{info.Type[0].ToString().ToUpper() + info.Type.Substring(1)}, { info.NumValues }, ");
            if (info.Values == null)
                sb.Append("null");
            else
                sb.Append($"new string[] {{ { string.Join(", ", info.Values.Select(x => '"' + x + '"'))} }}");
            sb.Append("),");
            return sb.ToString();
        }
    }
}
