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

            codeGenerator.Begin("namespace MineSharp.Data.Entities");

            codeGenerator.Begin("public static class EntityPalette");

            codeGenerator.Begin("public static Type GetEntityTypeById(int id) => id switch");
            foreach (var entity in entityData)
                codeGenerator.WriteLine($"{entity.Id} => typeof({this.Wrapper.GetCSharpName(entity.Name)}),");
            codeGenerator.WriteLine($"_ => throw new ArgumentException($\"Entity with id {{id}} not found!\")");
            codeGenerator.Finish(semicolon: true);
            codeGenerator.Finish();

            codeGenerator.Begin("public enum EntityCategory");
            List<string>? categories = entityData.Select(x => $"{this.Wrapper.GetCSharpName(x.Category)}").Distinct().ToList();
            foreach (var category in categories)
                codeGenerator.WriteLine($"{category} = {categories.IndexOf(category)},");
            codeGenerator.Finish();

            foreach (var entity in entityData)
            {

                codeGenerator.Begin($"public class {this.Wrapper.GetCSharpName(entity.Name)} : Entity");
                codeGenerator.WriteBlock(
                    $@"public const int EntityId = {entity.Id};
public const string EntityName = "" {entity.Name}"";
public const string EntityDisplayName = ""{entity.DisplayName}"";

public const float EntityWidth = {entity.Width.ToString(nfi)}F;
public const float EntityHeight = {entity.Height.ToString(nfi)}F;
public const int EntityCategory = {categories.IndexOf(this.Wrapper.GetCSharpName(entity.Category))};


public {this.Wrapper.GetCSharpName(entity.Name)} (int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects) : base(serverId, position, pitch, yaw, velocity, isOnGround, effects, EntityId, EntityName, EntityDisplayName, EntityWidth, EntityHeight, EntityCategory) {{}}");
                codeGenerator.Finish();

            }

            codeGenerator.Begin("public enum EntityType");
            foreach (var entity in entityData)
                codeGenerator.WriteLine($"{this.Wrapper.GetCSharpName(entity.Name)} = {entity.Id},");
            codeGenerator.Finish();
            codeGenerator.Finish();
        }
    }
}
