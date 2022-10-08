using MineSharp.Data.Generator.Protocol.Datatypes;
using Newtonsoft.Json.Linq;
namespace MineSharp.Data.Generator.Protocol
{
    internal class ProtocolGenerator : Generator
    {

        private readonly ProtoCompiler Compiler;

        public ProtocolGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version)
        {
            this.Compiler = new ProtoCompiler();
            this.Compiler.AddType("UUID", new UUIDDatatypeGenerator(this.Compiler));
            this.Compiler.AddType("entityMetadataLoop", new EntityMetadataLoopGenerator(this.Compiler));
            this.Compiler.AddType("topBitSetTerminatedArray", new TopBitSetTerminatedArrayGenerator(this.Compiler));
            this.Compiler.AddType("restBuffer", new RestBufferGenerator(this.Compiler));
            this.Compiler.AddType("nbt", new NbtDatatypeGenerator(this.Compiler));
            this.Compiler.AddType("optionalNbt", new OptionalNbtGenerator(this.Compiler));
        }


        public string[] GetUsings()
        {
            return new[] {
                "System.IO",
                "System.Collections.Generic",
                "System.Linq"
            }.Concat(this.Compiler.UsedNamespaces.Distinct()).ToArray();
        }

        public override void WriteCode(CodeGenerator codeGenerator)
        {
            var data = JObject.Parse(File.ReadAllText(this.Wrapper.GetJsonPath(this.Version, "protocol")));
            codeGenerator.CommentBlock($"Generated Protocol Data for Minecraft Version {this.Version}");
            foreach (var ns in this.GetUsings())
                codeGenerator.WriteLine($"using {ns};");
            this.Compiler.WriteCode(codeGenerator, data, "MineSharp.Data.Protocol");
        }
    }
}
