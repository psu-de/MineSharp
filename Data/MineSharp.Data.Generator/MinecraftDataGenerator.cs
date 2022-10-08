using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Generator
{
    internal class MinecraftDataGenerator : Generator
    {
        public MinecraftDataGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {}

        public override void WriteCode(CodeGenerator codeGenerator)
        {

            var data = JObject.Parse(File.ReadAllText(this.Wrapper.GetJsonPath(this.Version, "version")));

            codeGenerator.CommentBlock("Generated MinecraftData for version " + this.Version);

            codeGenerator.Begin("namespace MineSharp.Data");
            codeGenerator.Begin("public static class MinecraftData");
            codeGenerator.WriteLine($"public static string MinecraftVersion = \"{this.Version}\";");
            codeGenerator.WriteLine($"public static int ProtocolVersion = {(int)data.GetValue("version")!};");
            codeGenerator.Finish();
            codeGenerator.Finish();
        }
    }
}
