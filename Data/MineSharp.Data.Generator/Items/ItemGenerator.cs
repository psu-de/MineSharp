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

  //          codeGenerator.Begin("namespace MineSharp.Data.Items");

  //          codeGenerator.Begin("public static class ItemPalette");

  //          codeGenerator.Begin("public static Type GetItemTypeById(int id) => id switch");
  //          foreach (var item in itemData)
  //              codeGenerator.WriteLine($"{item.Id} => typeof({this.Wrapper.GetCSharpName(item.Name)}Item),");
  //          codeGenerator.WriteLine("_ => throw new ArgumentException($\"Item with id {id} not found!\")");
  //          codeGenerator.Finish(semicolon: true);
  //          codeGenerator.Finish();

  //          foreach (var item in itemData)
  //          {

  //              codeGenerator.Begin($"public class {this.Wrapper.GetCSharpName(item.Name)}Item : Item");
  //              codeGenerator.WriteBlock(
  //                  $@"public const int ItemId = {item.Id};
		//public const string ItemName = "" {item.Name}"";
		//public const string ItemDisplayName = ""{item.DisplayName}"";

  //      public const byte ItemStackSize = {item.StackSize};
  //      public static readonly int? ItemMaxDurability = {item.MaxDurability.ToString() ?? "null"};
  //      public static readonly string[]? ItemEnchantCategories = {this.WriteStringArray(item.EnchantCategories)};
		//public static readonly string[]? ItemRepairWith = {this.WriteStringArray(item.RepairWith)};


  //      public {this.Wrapper.GetCSharpName(item.Name)}Item () : base (ItemId, ItemDisplayName, ItemName, ItemStackSize, ItemMaxDurability, ItemEnchantCategories, ItemRepairWith) {{}}
		//public {this.Wrapper.GetCSharpName(item.Name)}Item (byte count, int? damage, fNbt.NbtCompound? metadata) : base(count, damage, metadata, ItemId, ItemDisplayName, ItemName, ItemStackSize, ItemMaxDurability, ItemEnchantCategories, ItemRepairWith) {{}}");
  //              codeGenerator.Finish();

  //          }

  //          codeGenerator.Begin("public enum ItemType");
  //          foreach (var item in itemData)
  //              codeGenerator.WriteLine($"{this.Wrapper.GetCSharpName(item.Name)} = {item.Id},");
  //          codeGenerator.Finish();
  //          codeGenerator.Finish();
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
