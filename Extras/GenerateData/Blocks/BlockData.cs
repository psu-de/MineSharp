using GenerateData.Generators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData.Blocks {
    internal static class BlockData {

        private static string NewBlockInfoTemplate = "new BlockInfo () { Id=%id%, DisplayName=\"%displayname%\", Name=\"%name%\", Hardness=%hardness%, Resistance=%resistance%, Diggable=%diggable%, Transparent=%transparent%, FilterLight=%filterlight%, EmitLight=%emitlight%, BoundingBox=\"%boundingbox%\", StackSize=%stacksize%, Material=\"%material%\", DefaultState=%defaultstate%, MinStateId=%minSid%, MaxStateId=%maxSid% }";

        public static void Generate(string blockJsonData, string outDir) {

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<BlockJsonInfo>? Blocks = JsonConvert.DeserializeObject<List<BlockJsonInfo>>(File.ReadAllText(blockJsonData));

            if (Blocks == null) {
                throw new Exception("Could not load blocks!");
            }

            EnumGenerator BlockEnum = new EnumGenerator();
            ClassGenerator BlockData = new ClassGenerator();

            Directory.CreateDirectory(Path.Join(outDir, "Blocks"));

            string GetItemArray(Dictionary<string, bool>? dict) {
                if (dict != null) {

                    return $"new Items.ItemType[] {{ { string.Join(", ", dict.Keys.Select(x => "(Items.ItemType)" + x)) } }}";

                }
                return "null";
            }

            string GetProperties(BlockJsonInfo jsonData) {
                string GetValues(string[]? values) {
                    return values == null ? "null" : $"new string[] {{ {string.Join(", ", values.Select(x => $"\"{x}\""))} }}";
                }

                return $"new BlockProperties(new BlockStateProperty[] {{ { string.Join(", ", jsonData.States.Select(x => $"new BlockStateProperty(\"{x.Name}\", BlockStateProperty.BlockStatePropertyType.{x.Type[0].ToString().ToUpper() + x.Type.Substring(1)}, {x.NumValues}, { GetValues(x.Values) })")) } }}, { jsonData.DefaultState - jsonData.MinStateId })";
            }

            int lastId = 0;
            foreach (var blockInfo in Blocks) {

                if (blockInfo.Id != lastId++) throw new Exception("Last id should be currentid-1");
                var props = GetProperties(blockInfo);

                string staticName = Generator.MakeCSharpSafe(blockInfo.DisplayName.Replace(" ", ""));
                BlockEnum.EnumAddValue(staticName, blockInfo.Id);
                BlockData.AddLoaderExpression($"Register(BlockType.{staticName}, \"{blockInfo.DisplayName}\", \"{blockInfo.Name}\", {(blockInfo.Hardness ?? float.MaxValue).ToString(nfi)}f, {blockInfo.Resistance.ToString(nfi)}f, {blockInfo.Diggable.ToString().ToLower()}, {blockInfo.Transparent.ToString().ToLower()}, {blockInfo.FilterLight}, {blockInfo.EmitLight}, \"{blockInfo.BoundingBox}\", {blockInfo.StackSize}, \"{blockInfo.Material}\", {blockInfo.StackSize}, {blockInfo.MinStateId}, {blockInfo.MaxStateId}, {GetItemArray(blockInfo.HarvestTools)}, {GetProperties(blockInfo)});");
            }


            BlockData.WithRegisterFunction(@"private static void Register(BlockType type, string displayName, string name, float hardness, float resistance, bool diggable, bool transparent, int filterLight, int emitLight, string boundingbox, int stackSize, string material, int defaultState, int minState, int maxState, Items.ItemType[]? harvestTools, BlockProperties properties){ 
            BlockInfo info = new BlockInfo(type, displayName, name, hardness, resistance, diggable, transparent, filterLight, emitLight, boundingbox, stackSize, material, defaultState, minState, maxState, harvestTools, properties);
            Blocks.Add(info);
            for (int i = info.MinStateId; i <= info.MaxStateId; i++) {
                StateToBlockMap.Add(i, Blocks[(int)info.Id]);
            }
        }");

            BlockData.AddClassVariable("public static List<BlockInfo> Blocks = new List<BlockInfo>();")
                     .AddClassVariable("public static Dictionary<int, BlockInfo> StateToBlockMap = new Dictionary<int, BlockInfo>();")
                     .WithName("BlockData")
                     .WithNamespace("Data.Blocks")
                     .Write(Path.Join(outDir, "Blocks", "BlockData.cs"));
            BlockEnum.WithNamespace("Data.Blocks").WithName("BlockType").Write(Path.Join(outDir, "Blocks", "BlockType.cs"));
        }
    }
}
