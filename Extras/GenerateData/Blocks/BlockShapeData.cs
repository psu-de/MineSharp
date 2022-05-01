using GenerateData.Generators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData.Blocks {
    internal class BlockShapeData {

        public static void Generate(string blockShapeJsonFile, string outDir) {

            BlockShapeJsonInfo? blockShapes = JsonConvert.DeserializeObject<BlockShapeJsonInfo>(File.ReadAllText(blockShapeJsonFile));
            if (blockShapes == null) {
                Console.WriteLine("Could not load blockShapes");
                return;
            }
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            ClassGenerator BlockShapeData = new ClassGenerator();

            Directory.CreateDirectory(Path.Join(outDir, "Blocks"));

            string getShapeArrayStr(List<List<float>> shapeList) {
                return $"new float[][] {{ {string.Join(", ", shapeList.Select(x => $"new [] {{ {string.Join(", ", x.Select(y => y.ToString(nfi) + "f"))} }}"))} }}";
            }
            
            string getShapeMapArrayStr(object shapeMap) {
                if (shapeMap.GetType() == typeof(JArray)) {
                    var arr = (JArray)shapeMap;
                    return $"new int[] {{ {string.Join(", ", arr)} }}";
                } else {
                    return $"new int[] {{ {(long)shapeMap} }}";
                }
            }

            BlockShapeData.AddClassVariable($"public static Dictionary<int, float[][]> Shapes = new Dictionary<int, float[][]>() {{ " +
                $"{string.Join(",\n", blockShapes.Shapes.Select(x => $"{{ {Convert.ToInt32(x.Key)}, { getShapeArrayStr(x.Value) } }}")) } }};");

            BlockShapeData.AddClassVariable($"public static Dictionary<string, int[]> BlockToShapeMapping = new Dictionary<string, int[]>() {{ " +
                $"{string.Join(",\n", blockShapes.Blocks.Select(x => $"{{ \"{x.Key}\", {getShapeMapArrayStr(x.Value)} }}"))} }};");

            BlockShapeData.WithName("BlockShapes")
                        .WithNamespace("Data.Blocks")
                        .Write(Path.Join(outDir, "Blocks", "BlockShapeData.cs"));

        }

    }
}
