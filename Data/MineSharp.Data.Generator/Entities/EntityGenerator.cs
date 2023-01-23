using System.Globalization;

namespace MineSharp.Data.Generator.Entities
{
    internal class EntityGenerator : Generator
    {
        public EntityGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {}

        public string[] GetUsings()
        {
            return new[] {
                "MineSharp.Core.Types", "System.Collections.Generic"
            };
        }

        public override void WriteCode(CodeGenerator codeGenerator)
        {

            var entityData = this.Wrapper.LoadJson<EntityJsonInfo[]>(this.Version, "entities");

            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            codeGenerator.CommentBlock($"Generated Entity Data for Minecraft Version {this.Version}");

            foreach (var ns in this.GetUsings())
                codeGenerator.WriteLine($"using {ns};");

            var infoTemplateGenerator = new InfoGeneratorTemplate<EntityJsonInfo>()
            {
                Name = "Entity",
                Namespace = "MineSharp.Data.Entities",
                Data = entityData,
                Indexer = (t) => t.Id,
                NameGenerator = (t) => this.Wrapper.GetCSharpName(t.Name),
                Stringifiers = new Dictionary<string, Func<object, string>>()
                {
                    { "Id", (e) => InfoGenerator<int>.StringifyDefaults(e) },
                    { "Name", (e) => InfoGenerator<int>.StringifyDefaults(e) },
                    { "DisplayName", (e) => InfoGenerator<int>.StringifyDefaults(e) },
                    { "Width", (e) => InfoGenerator<int>.StringifyDefaults(e) },
                    { "Height", (e) => InfoGenerator<int>.StringifyDefaults(e) },
                    { "Category", (x) => $"(int)EntityCategoryType.{this.Wrapper.GetCSharpName(x as string)}" },
                },
                Indexers = new EnumGenerator<EntityJsonInfo>[] 
                {
                    new EnumGenerator<EntityJsonInfo>()
                    {
                        GetName= (x) => this.Wrapper.GetCSharpName(x.Category),
                        Name = "EntityCategory",
                    }
                }
            };

            var infoGenerator = new InfoGenerator<EntityJsonInfo>(infoTemplateGenerator);
            infoGenerator.GenerateInfos(codeGenerator);
        }
    }
}
