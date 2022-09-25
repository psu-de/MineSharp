using MineSharp.Data.Generator.Protocol.Datatypes;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Generator.Protocol
{
    internal class ProtocolGenerator : Generator {

        private ProtoCompiler Compiler;

        public ProtocolGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {
            Compiler = new ProtoCompiler();
            Compiler.AddType("UUID", new UUIDDatatypeGenerator(Compiler));
            Compiler.AddType("entityMetadataLoop", new EntityMetadataLoopGenerator(Compiler));
            Compiler.AddType("topBitSetTerminatedArray", new TopBitSetTerminatedArrayGenerator(Compiler));
            Compiler.AddType("restBuffer", new RestBufferGenerator(Compiler));
            Compiler.AddType("nbt", new NbtDatatypeGenerator(Compiler));
            Compiler.AddType("optionalNbt", new OptionalNbtGenerator(Compiler));
        }


        public string[] GetUsings() {
            return new[] { "System.IO", "System.Collections.Generic", "System.Linq" }.Concat(Compiler.UsedNamespaces.Distinct()).ToArray();
        }

        public override void WriteCode(CodeGenerator codeGenerator) {
            var data = JObject.Parse(File.ReadAllText(Wrapper.GetJsonPath(Version, "protocol")));
            codeGenerator.CommentBlock($"Generated Protocol Data for Minecraft Version {Version}");
            foreach (var ns in GetUsings())
                codeGenerator.WriteLine($"using {ns};");
            Compiler.WriteCode(codeGenerator, data, "MineSharp.Data.Protocol");
        }
    }
}
