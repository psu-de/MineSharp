using MineSharp.Core.Types;
using System.Globalization;
using System.Text;

namespace MineSharp.Data.Generator.Blocks
{
    internal class BlockGenerator : Generator
    {
        public BlockGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {}

        public string[] GetUsings()
        {
            return new[] {
                "MineSharp.Core.Types", "System.Collections.Generic"
            };
        }

        public override void WriteCode(CodeGenerator codeGenerator)
        {
            var blockData = this.Wrapper.LoadJson<BlockJsonInfo[]>(this.Version, "blocks");
            var blockCollisionData = this.Wrapper.LoadJson<BlockCollisionShapeJson>(this.Version, "blockCollisionShapes");

            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            codeGenerator.CommentBlock($"Generated Block Data for Minecraft Version {this.Version}");

            foreach (var ns in this.GetUsings())
                codeGenerator.WriteLine($"using {ns};");

            codeGenerator.Begin("namespace MineSharp.Data.Blocks");
            codeGenerator.Begin("public static class BlockShapePalette");
            codeGenerator.Begin("public static readonly Dictionary<int, float[][]> AllBlockShapes = new Dictionary<int, float[][]>()");

            foreach (var shape in blockCollisionData.Shapes)
                codeGenerator.WriteLine($"{{ {shape.Key}, new float[][] {{ {string.Join(", ", shape.Value.Select(x => $"new float[] {{ {string.Join(", ", x.Select(x => x.ToString(nfi) + "F"))} }}"))} }} }},");

            codeGenerator.Finish(semicolon: true);
            codeGenerator.Finish();
            codeGenerator.Finish();


            var infoGeneratorTemplate = new InfoGeneratorTemplate<BlockJsonInfo>() {
                Data = blockData,
                Name = "Block",
                Namespace = "MineSharp.Data.Blocks",
                NameGenerator = (t) => this.Wrapper.GetCSharpName(t.Name),
                Indexer = (t) => t.Id,
                Stringifiers = new Dictionary<string, Func<object, string>>() {
                    { "Id", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Name", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "DisplayName", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Hardness", x => InfoGenerator<int>.StringifyDefaults(x ?? float.MaxValue) },
                    { "Resistance", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Diggable", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Transparent", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "FilterLight", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "EmitLight", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "BoundingBox", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "StackSize", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Material", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "DefaultState", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "MinStateId", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "MaxStateId", x => InfoGenerator<int>.StringifyDefaults(x) },
                    { "HarvestTools", x => x == null ? "null" : $"new int[] {{ {string.Join(", ", (x as Dictionary<string, bool>)!.Keys)} }}" },
                    { "States",  x => $@"new BlockProperties(new BlockStateProperty[] {{ {string.Join("", (x as BlockStateJsonInfo[])!.Select(x => this.GetProperty(x)))} }})"}
                },
                AdditionalInfoArgs = new Func<BlockJsonInfo, string>[] {
                    (t) => $@"new int[] {{ {string.Join(", ", BlockCollisionShapeJson.GetShapeIndices(blockCollisionData.Blocks[t.Name]))} }}",
                },
                GenerateCodeBlock = (data, codeGenerator) => {
                    codeGenerator.Begin("public static int GetBlockIdByState(int state) => state switch");
                    foreach (var block in data)
                        codeGenerator.WriteLine($"(>= {block.MinStateId}) and (<= {block.MaxStateId}) => {block.Id},");
                    codeGenerator.WriteLine("_ => throw new ArgumentException($\"Block with state {state} not found!\")");
                    codeGenerator.Finish(semicolon: true);
                }
            };
            var infoGenerator = new InfoGenerator<BlockJsonInfo>(infoGeneratorTemplate);
            infoGenerator.GenerateInfos(codeGenerator);
        }

        private string GetProperty(BlockStateJsonInfo info)
        {
            var sb = new StringBuilder();

            sb.Append($"new BlockStateProperty(\"{info.Name}\", BlockStateProperty.BlockStatePropertyType.{info.Type[0].ToString().ToUpper() + info.Type.Substring(1)}, {info.NumValues}, ");
            if (info.Values == null)
                sb.Append("null");
            else
                sb.Append($"new string[] {{ {string.Join(", ", info.Values.Select(x => '"' + x + '"'))} }}");
            sb.Append("),");
            return sb.ToString();
        }
    }
}
