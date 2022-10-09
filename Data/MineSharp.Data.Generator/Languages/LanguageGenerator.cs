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
                var val = kvp.Value
                    .Replace("\n", "\\n")
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");
                codeGenerator.WriteLine($"{{ \"{kvp.Key}\", \"{val}\" }},");
            }
            
            codeGenerator.Finish(semicolon: true);
            
            codeGenerator.WriteBlock(@"
public static string? GetRule(string name) {
    if (Rules.TryGetValue(name, out var rule)) {
        return rule;
    } else return null;
}");
            
            codeGenerator.Finish();
            codeGenerator.Finish();
        }
    }
}
