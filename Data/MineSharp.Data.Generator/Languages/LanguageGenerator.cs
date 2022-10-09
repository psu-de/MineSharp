namespace MineSharp.Data.Generator.Languages
{
    internal class LanguageGenerator : Generator
    {

        public LanguageGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {}
        
        public override void WriteCode(CodeGenerator codeGenerator)
        {
            var rules = this.Wrapper.LoadJson<Dictionary<string, string>>(this.Version, "language");
            
            codeGenerator.CommentBlock($"Generated Language Data for Minecraft Version {this.Version}");
            
            codeGenerator.Begin("namespace MineSharp.Data.Languages");
            codeGenerator.Begin("public static class Language");
            
            codeGenerator.Begin("public static Dictionary<string, string> Rules = new ()");

            foreach (var kvp in rules)
            {
                codeGenerator.WriteLine($"{{ \"{kvp.Key}\", \"{kvp.Value.Replace("\n", "\\n")}\" }},");
            }
            
            codeGenerator.Finish();
            codeGenerator.Finish();
            codeGenerator.Finish();
        }
    }
}
