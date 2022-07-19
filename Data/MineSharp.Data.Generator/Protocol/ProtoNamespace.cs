using Newtonsoft.Json.Linq;
using MineSharp.Data.Generator.Protocol.Datatypes;

namespace MineSharp.Data.Generator.Protocol {
    internal class ProtoNamespace {

        public ProtoCompiler Compiler { get; set; }
        public ProtoNamespace? Parent;
        public Dictionary<string, DatatypeGenerator> UsedNativeTypes = new Dictionary<string, DatatypeGenerator>();
        public Dictionary<string, DatatypeGenerator> AllNativeTypes => 
            new[] { UsedNativeTypes, Parent?.AllNativeTypes }
                    .Where(x => x != null)
                    .SelectMany(dict => dict!)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

        public Dictionary<string, Datatype> Types;
        public Dictionary<string, Datatype> AllTypes => 
            new[] { Types, Parent?.AllTypes }
                    .Where(x => x != null)
                    .SelectMany(dict => dict!)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
        public string Namespace { get; set; }
        internal JObject Token { get; set; }

        public ProtoNamespace(ProtoCompiler compiler, ProtoNamespace? parent, string @namespace, JObject token) {
            Compiler = compiler;
            Parent = parent;
            Types = new Dictionary<string, Datatype>();
            Namespace = @namespace;
            Token = token;
        }

        public ProtoNamespace[] Parse() {

            var typesToken = (JObject?)Token.GetValue("types");
            
            if (typesToken != null) {

                foreach (var prop in typesToken!.Properties()) {
                    string name = prop.Name;

                    if (prop.Value.Type == JTokenType.String) {
                        var typeRef = (string)prop.Value!;
                        if (typeRef == "native") {
                            UsedNativeTypes.Add(name, Compiler.NativeTypes[name]);
                        } else {
                            if (!UsedNativeTypes.ContainsKey(typeRef)) {
                                Types.Add(name, Datatype.Parse(Compiler, prop.Value, name, null, null));
                            } else {
                                UsedNativeTypes.Add(name, UsedNativeTypes[typeRef]);
                            }
                        }
                    } else {
                        var datatype = Datatype.Parse(Compiler, prop.Value, name, null, null);
                        Types.Add(name, datatype);
                    }
                }
            }

            List<ProtoNamespace> innerNamespaces = new List<ProtoNamespace>();
            foreach (var prop in Token.Properties().Where(x => x.Name != "types"))
            {
                var ns = Compiler.GetCSharpName(prop.Name)
                    .Replace("ToClient", "Clientbound")
                    .Replace("ToServer", "Serverbound");
                var innerNamespace = new ProtoNamespace(Compiler, this, Namespace + "." + ns, (JObject)prop.Value!);
                innerNamespaces.Add(innerNamespace);
            }
            return innerNamespaces.ToArray();
        }

        private void WritePacketBuffer(CodeGenerator codeGenerator, string baseNamespace) {
            codeGenerator.Begin($"namespace {baseNamespace}");
            codeGenerator.Begin($"public partial class PacketBuffer");

            codeGenerator.WriteLine("#region Reading");
            codeGenerator.WriteLine();
            foreach (var generator in UsedNativeTypes.Select(x => x.Value).DistinctBy(x => x.TypeName)) 
                generator.WriteClassReader(codeGenerator);
            codeGenerator.WriteLine();
            codeGenerator.WriteLine("#endregion");

            codeGenerator.WriteLine("#region Writing");
            codeGenerator.WriteLine();
            foreach (var generator in UsedNativeTypes.Select(x => x.Value).DistinctBy(x => x.TypeName)) 
                generator.WriteClassWriter(codeGenerator);
            codeGenerator.WriteLine();
            codeGenerator.WriteLine("#endregion");

            codeGenerator.Finish();
            codeGenerator.Finish();
        }

        public void WriteCode (CodeGenerator codeGenerator, string baseNamespace) {


            WritePacketBuffer(codeGenerator, baseNamespace);

            codeGenerator.Begin($"namespace {Namespace}");

            foreach (var type in this.Types) {
                type.Value.DoWriteStructure(codeGenerator, null);
            }

            if (this.Types.ContainsKey("packet")) {
                var namespaces = Namespace.Split(".");
                codeGenerator.Begin($"public class {Compiler.GetCSharpName(namespaces[namespaces.Length - 2])}PacketFactory : IPacketFactory");

                codeGenerator.Begin($@"public IPacket ReadPacket(PacketBuffer buffer)");
                codeGenerator.WriteLine($"return {Namespace}.Packet.Read(buffer);");
                codeGenerator.Finish();

                var packetType = (ContainerDatatype)this.Types["packet"];
                var @params = (SwitchDatatype)packetType.FieldMap!["params"].Type;

                codeGenerator.Begin($@"public void WritePacket(PacketBuffer buffer, IPacketPayload packet)");
                codeGenerator.Begin($"switch (packet)");
                int i = 0;
                foreach (var type in @params.SwitchMap!) {
                    codeGenerator.WriteLine(@$"case {type.Value.CSharpType} p_0x{i.ToString("X2")}: new {Namespace}.Packet(""{type.Key}"", p_0x{i.ToString("X2")}!).Write(buffer); break;");
                    i++;
                }
                codeGenerator.WriteLine(@$"default: throw new Exception(""{Compiler.GetCSharpName(namespaces[namespaces.Length - 2])} cannot write packet of type {{typeof(packet).FullName}}"");");
                codeGenerator.Finish();
                codeGenerator.Finish();
                codeGenerator.Finish();
            }

            codeGenerator.Finish();
        }
    }
}
