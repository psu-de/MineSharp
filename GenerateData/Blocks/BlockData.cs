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

            int lastId = 0;
            foreach (var blockInfo in Blocks) {

                if (blockInfo.Id != lastId++) throw new Exception("Last id should be currentid-1");

                string staticName = Generator.MakeCSharpSafe(blockInfo.DisplayName.Replace(" ", ""));
                BlockEnum.EnumAddValue(staticName, blockInfo.Id);
                //string blockInfoStr = NewBlockInfoTemplate.Replace("%id%", blockInfo.Id.ToString())
                //                                 .Replace("%displayname%", blockInfo.DisplayName)
                //                                 .Replace("%name%", blockInfo.Name)
                //                                 .Replace("%hardness%", (blockInfo.Hardness ?? float.MaxValue).ToString(nfi) + "f")
                //                                 .Replace("%resistance%", blockInfo.Resistance.ToString(nfi) + "f")
                //                                 .Replace("%diggable%", blockInfo.Diggable.ToString().ToLower())
                //                                 .Replace("%transparent%", blockInfo.Transparent.ToString().ToLower())
                //                                 .Replace("%filterlight%", blockInfo.FilterLight.ToString())
                //                                 .Replace("%emitlight%", blockInfo.EmitLight.ToString())
                //                                 .Replace("%boundingbox%", blockInfo.BoundingBox)
                //                                 .Replace("%stacksize%", blockInfo.StackSize.ToString())
                //                                 .Replace("%material%", blockInfo.Material)
                //                                 .Replace("%defaultstate%", blockInfo.DefaultState.ToString())
                //                                 .Replace("%minSid%", blockInfo.MinStateId.ToString())
                //                                 .Replace("%maxSid%", blockInfo.MaxStateId.ToString());
                BlockData.AddLoaderExpression($"Register(BlockType.{staticName}, \"{blockInfo.DisplayName}\", \"{blockInfo.Name}\", {(blockInfo.Hardness ?? float.MaxValue).ToString(nfi)}f, {blockInfo.Resistance.ToString(nfi)}f, {blockInfo.Diggable.ToString().ToLower()}, {blockInfo.Transparent.ToString().ToLower()}, {blockInfo.FilterLight}, {blockInfo.EmitLight}, \"{blockInfo.BoundingBox}\", {blockInfo.StackSize}, \"{blockInfo.Material}\", {blockInfo.StackSize}, {blockInfo.MinStateId}, {blockInfo.MaxStateId}, {GetItemArray(blockInfo.HarvestTools)});");
            }


            BlockData.WithRegisterFunction(@"private static void Register(BlockType type, string displayName, string name, float hardness, float resistance, bool diggable, bool transparent, int filterLight, int emitLight, string boundingbox, int stackSize, string material, int defaultState, int minState, int maxState, Items.ItemType[]? harvestTools){ 
            BlockInfo info = new BlockInfo(type, displayName, name, hardness, resistance, diggable, transparent, filterLight, emitLight, boundingbox, stackSize, material, defaultState, minState, maxState, harvestTools);
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
