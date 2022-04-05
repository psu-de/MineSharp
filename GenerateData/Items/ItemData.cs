using GenerateData.Generators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData.Items {
    public class ItemData {

        public static void Generate(string itemJson, string outDir) {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<ItemJsonInfo>? Items = JsonConvert.DeserializeObject<List<ItemJsonInfo>>(File.ReadAllText(itemJson));

            if (Items == null) {
                throw new Exception("Could not load items!");
            }


            Directory.CreateDirectory(Path.Join(outDir, "Items"));

            ClassGenerator itemData = new ClassGenerator();
            EnumGenerator itemTypes = new EnumGenerator();

            string GetIfNotNull(object? value) {

                if (value != null && value is List<string>) {
                    List<string> array = value as List<string> ?? new List<string>();
                    array = array.Select(x => $"\"{x}\"").ToList();
                    string result = $"new string[] {{ {string.Join(", ", array)} }}";
                    return result;
                }

                return value?.ToString() ?? "null";
            }

            Dictionary<string, int> nameDuplications = new Dictionary<string, int>();
            itemTypes.EnumAddValue("NoItem", 0);
            itemData.AddLoaderExpression($"Register(ItemType.NoItem, null, null, 0, null, null, null);");
            int lastId = 1;
            foreach (var item in Items) {
                if (item.Id != lastId++) throw new Exception("last id must be currentid - 1");
                string name = Generator.MakeCSharpSafe(item.DisplayName.Replace(" ", ""));
                if (nameDuplications.ContainsKey(name)) {
                    nameDuplications[name]++;
                    name += nameDuplications[name].ToString();
                }
                nameDuplications.Add(name, 1);


                itemTypes.EnumAddValue(name, item.Id);
                itemData.AddLoaderExpression($"Register(ItemType.{name}, \"{item.DisplayName}\", \"{item.Name}\", {item.StackSize}, {GetIfNotNull(item.MaxDurability)}, {GetIfNotNull(item.EnchantCategories)}, {GetIfNotNull(item.RepairWith)});");
            }

            itemData.AddClassVariable("public static List<ItemInfo> Items = new List<ItemInfo>();");
            itemData.RegisterFunctionBlock = @"
private static void Register(ItemType type, string displayName, string name, int stackSize, int? maxDurability, string[]? enchantCategories, string[]? repairWith) {

    Items.Add(new ItemInfo(type, displayName, name, stackSize, maxDurability, enchantCategories, repairWith));

}
";

            itemTypes.WithName("ItemType").WithNamespace("Data.Items").Write(Path.Join(outDir, "Items", "ItemType.cs"));
            itemData.WithName("ItemData").WithNamespace("Data.Items").Write(Path.Join(outDir, "Items", "ItemData.cs"));
        }

    }
}
