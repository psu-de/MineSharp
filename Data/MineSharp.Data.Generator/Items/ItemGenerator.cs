using MineSharp.Core.Types;
using System.Globalization;
using System.Text;

namespace MineSharp.Data.Generator.Items
{
    internal class ItemGenerator : Generator
    {
        public ItemGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {}

        public string[] GetUsings()
        {
            return new[] {
                "MineSharp.Core.Types", "fNbt"
            };
        }

        public override void WriteCode(CodeGenerator codeGenerator)
        {

            var itemData = this.Wrapper.LoadJson<ItemJsonInfo[]>(this.Version, "items");

            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            codeGenerator.CommentBlock($"Generated Item Data for Minecraft Version {this.Version}");

            foreach (var ns in this.GetUsings())
                codeGenerator.WriteLine($"using {ns};");

            var infoGeneratorTemplate = new InfoGeneratorTemplate<ItemJsonInfo>()
            {
                Data= itemData,
                Name = "Item",
                Namespace = "MineSharp.Data.Items",
                Indexer = (i) => i.Id,
                NameGenerator= (i) => this.Wrapper.GetCSharpName(i.Name),
                Stringifiers = new Dictionary<string, Func<object, string>>()
                {
                    { "Id", (i) => InfoGenerator<int>.StringifyDefaults(i) },
                    { "DisplayName", (i) => InfoGenerator<int>.StringifyDefaults(i) },
                    { "Name", (i) => InfoGenerator<int>.StringifyDefaults(i) },
                    { "StackSize", (i) => InfoGenerator<int>.StringifyDefaults(i) },
                    { "MaxDurability", (i) => i.ToString() ?? "null" },
                    { "EnchantCategories", (i) => this.WriteStringArray((string[]?)i) },
                    { "RepairWith", (i) => this.WriteStringArray((string[]?)i) },
                }
            };
            var infoGenerator = new InfoGenerator<ItemJsonInfo>(infoGeneratorTemplate);
            infoGenerator.GenerateInfos(codeGenerator);
        }

        private string WriteStringArray(string[]? arr)
        {
            if (arr == null) return "null";
            var sb = new StringBuilder();
            sb.Append("new string[] {");
            sb.Append(string.Join(", ", arr.Select(x => '"' + x + '"')));
            sb.Append("}");
            return sb.ToString();
        }
    }
}
