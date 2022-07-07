using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Protocol {
    public class ProtocolWriter {

        private static JObject Data;

        static ProtocolWriter() {
            string data = File.ReadAllText(MinecraftData.GetJsonPath(MinecraftData.Version, "protocol"));
            Data = JObject.Parse(data);
        }

        private static ProtoType[] GetStructureTypes(ProtoType type) {
            switch (type) {
                case ProtoContainer pc:
                    return pc.Fields.Where(x => x.Type.IsStructure).Select(x => x.Type).ToArray();
                case ProtoMapper pm:
                    return pm.Map.Values.ToArray();
                case ProtoSwitch ps:
                    return ps.SwitchMap.Values.ToArray();
                case ProtoBitfield pb:
                    return new ProtoType[0];
                default:
                    throw new Exception(type.GetType() + " is not a structure");
            }
        }

        public static ProtoType[] GetNativeTypes() {
            List<ProtoType> types = new List<ProtoType>();
            foreach (var dir in new[] { "clientbound", "serverbound" }) {
                foreach (var ns in GetNamespaces()) {
                    foreach (var packet in GetPackets(ns, dir)) {
                        foreach (var field in packet.Fields) {
                            if (field.Type.IsStructure) {
                                types.AddRange(GetStructureTypes(field.Type));
                            } else {
                                types.Add(field.Type);
                            }
                        }
                    }
                }
            }

            types = types.DistinctBy(x => x.Name).ToList();
            var map = types.ToDictionary(x => x.Name);
            map.Remove("Array");

            map["Option"].IsStructure = true;
            map["Option"].StructureName = "T";
            map["Option"].Name = "Option<T>";

            map["TopBitSetTerminatedArray"].IsStructure = true;
            map["TopBitSetTerminatedArray"].StructureName = "TopBitSetTerminatedArray<T>";
            map["TopBitSetTerminatedArray"].Name = "TopBitSetTerminatedArray<T>";

            map.Remove("Void");

            return map.Values.ToArray();
        }

        public static string[] GetNamespaces() {
            return Data.Properties().Select(x => x.Name).Where(x => x != "types").ToArray();
        }

        public static PacketType[] GetPackets(string ns, string direction) {
            List<PacketType> packetTypes = new List<PacketType>();

            string path = $"{ns}.{((direction == "clientbound") ? "toClient" : "toServer")}.types";

            var packetMapToken = Data.SelectToken(path + ".packet");
            var packetMap = (ProtoContainer)ProtoType.Parse(packetMapToken!);
            var idMapper = (ProtoMapper)packetMap.Fields.First(x => x.Name == "Name").Type;
            var typeMapper = (ProtoSwitch)packetMap.Fields.First(x => x.Name == "Params").Type;

            foreach (var prop in ((JObject)Data.SelectToken(path)!).Properties().Where(x => x.Name != "packet")) {

                var packet = PacketType.Parse(((JArray)prop.Value)[1]);

                string packetType = MinecraftData.GetCSharpName(prop.Name);
                string packetName = MinecraftData.GetCSharpName(typeMapper.SwitchMap.First(x => x.Value.Name == packetType).Key);

                packet.StructureName = packetName + "Packet";
                packet.Id = idMapper.Map.First(x => x.Value.Name == packetName).Key;

                packetTypes.Add(packet);
            }

            return packetTypes.ToArray();
        }

        public class PacketType : ProtoContainer {
            public string Id { get; set; }

            public PacketType(ProtoField[] fields) : base(fields) { }

            public static new PacketType Parse(JToken opts) {
                JArray containerFields = (JArray)opts;
                List<ProtoField> fields = new List<ProtoField>();

                foreach (JObject elem in containerFields) {
                    var field = ProtoField.Parse(elem);
                    if (field.Type is ProtoSwitch) {
                        try {
                            ProtoType current;

                            string[] path = ((ProtoSwitch)field.Type).CompareTo.Split("/");
                            if (path[0] == "..") { //find parent node with jsonpath
                                string jpath = opts.Path;
                                while (true) {
                                    var sp = jpath.Split(".type");
                                    jpath = string.Join(".type", sp.Take(sp.Length - 1)) + ".type";
                                    var parent = ProtoType.Parse(Data.SelectToken(jpath)!);
                                    if (parent.Name == "Container") {
                                        current = ProtoType.Parse(Data.SelectToken(jpath)!);
                                        break;
                                    }
                                }
                            } else
                                current = fields.First(x => x.Name == path[0]).Type;

                            ((ProtoSwitch)field.Type).CompareToType = current;

                        } catch (Exception e) {
                            throw new Exception(opts.Path, e);
                        }

                    }
                    fields.Add(field);
                }
                return new PacketType(fields.ToArray());
            }
        }

        public class ProtoType {
            public string Name { get; set; }
            public bool IsStructure { get; set; }
            public string? StructureName { get; set; }

            public virtual string Reader => "buffer.Read" + (IsStructure ? $"<{StructureName}>" : Name) + $"({(IsStructure ? $"new {StructureName}()" : "")})";
            public virtual string Writer => "buffer.Write" + (IsStructure ? "" : Name) + $"(({GetCSharpName()})%value%!)";
            public virtual bool HasStructure { get; protected set; } = false;

            public ProtoType(string name, bool isStructure) {
                Name = name;
                IsStructure = isStructure;
            }

            public static ProtoType Parse(JToken token) {

                //if (token == null) throw new Exception();

                if (token.Type == JTokenType.String) {
                    return new ProtoType(MinecraftData.GetCSharpName((string)token!), false);
                } else if (token.Type == JTokenType.Array) {
                    JArray arr = (JArray)token!;
                    string name = (string)arr[0]!;
                    JToken opts = arr[1];
                    switch (name) {
                        case "container": return ProtoContainer.Parse(opts);
                        case "switch": return ProtoSwitch.Parse(opts);
                        case "mapper": return ProtoMapper.Parse(opts);
                        case "buffer": return ProtoBuffer.Parse(opts);
                        case "array": return ProtoArray.Parse(opts);
                        case "option": return ProtoOption.Parse(opts);
                        case "bitfield": return ProtoBitfield.Parse(opts);
                        case "particleData": return ProtoParticleData.Parse(opts);
                        case "topBitSetTerminatedArray": return ProtoTopBitSetTerminatedArray.Parse(opts);
                        default:
                            throw new Exception("Datatype not implemented: " + name);
                    }
                } else throw new Exception("Expected string or array: " + token.Path);


            }

            public string GetCSharpName() {
                if (this.IsStructure) {
                    return this.StructureName!;
                }

                return Name switch {
                    "I8" => "sbyte",
                    "I16" => "short",
                    "I32" => "int",
                    "I64" => "long",
                    "U8" => "byte",
                    "U16" => "ushort",
                    "U32" => "uint",
                    "U64" => "ulong",
                    "Varint" => "int",
                    "String" => "string",
                    "Buffer" => "byte[]",
                    "UUID" => "UUID",
                    "Restbuffer" => "byte[]",
                    "F32" => "float",
                    "F64" => "double",
                    "Slot" => "Slot",
                    "Void" => "object",
                    "Bool" => "bool",
                    "Nbt" => "NbtCompound",
                    "Optionalnbt" => "NbtCompound",
                    "Position" => "Position",
                    "Array" => ((ProtoArray)this).Type.GetCSharpName() + "[]",
                    "Chunkblockentity" => "Chunkblockentity",
                    "ParticleData" => "ParticleData",
                    "Option" => ((ProtoOption)this).OptionType.GetCSharpName(),
                    "Entitymetadata" => "Entitymetadata",
                    "TopBitSetTerminatedArray" => $"TopBitSetTerminatedArray<{((ProtoTopBitSetTerminatedArray)this).Type.GetCSharpName()}>",
                    "Tags" => "Tags",
                    "MinecraftSmeltingFormat" => "MinecraftSmeltingFormat",
                    "Ingredient" => "Ingredient",

                    _ => throw new Exception("cannot find native type " + Name),
                };
            }
        }

        #region Types with arguments

        public class ProtoContainer : ProtoType {

            public ProtoField[] Fields { get; }

            public override string Reader => $"buffer.Read(new {StructureName}())";
            //public override string Writer => "buffer.Write(%value%)";

            public ProtoContainer(ProtoField[] fields) : base("Container", true) {
                Fields = fields;

                this.StructureName = "Container";
            }

            public static new ProtoContainer Parse(JToken opts) {
                JArray containerFields = (JArray)opts;
                List<ProtoField> fields = new List<ProtoField>();

                foreach (JObject elem in containerFields) {
                    var field = ProtoField.Parse(elem);

                    if (field.Type is ProtoSwitch) {
                        ProtoType current;

                        string[] path = ((ProtoSwitch)field.Type).CompareTo.Split("/");
                        if (path[0] == "..") { //find parent node with jsonpath
                            current = ProtoType.Parse((JToken)"i32");
                            path = new string[0];
                        } else
                            current = fields.First(x => x.Name == path[0]).Type;

                        for (int i = 1; i < path.Length; i++) {
                            string v = MinecraftData.Uppercase(path[i]);

                            current = current switch {
                                ProtoContainer container => container.Fields.First(x => x.Name == v).Type,
                                ProtoBitfield bitfield => ProtoType.Parse((JToken)"i32"),
                                _ => throw new Exception(opts.Path)

                            };
                        }

                            ((ProtoSwitch)field.Type).CompareToType = current;


                    }
                    fields.Add(field);
                }
                return new ProtoContainer(fields.ToArray());
            }
        }

        public class ProtoSwitch : ProtoType {

            public string CompareTo { get; set; }
            public ProtoType? CompareToType { get; set; }
            public Dictionary<string, ProtoType> SwitchMap = new Dictionary<string, ProtoType>();
            public ProtoType? DefaultType { get; set; }

            public override string Reader => $"buffer.Read<{StructureName}>(new {StructureName}(({CompareToType!.GetCSharpName()}){string.Join('.', CompareTo.Split("/").Select(x => x.Length < 3 ? x : x.Contains("_") ? MinecraftData.GetCSharpName(x) : MinecraftData.Uppercase(x)))}!))";

            public ProtoSwitch(string compareTo, Dictionary<string, ProtoType> switchMap, ProtoType? defaultVal) : base("Switch", true) {
                SwitchMap = switchMap;
                CompareTo = compareTo;
                DefaultType = defaultVal;
                this.StructureName = "Switch";
            }

            public static new ProtoSwitch Parse(JToken token) {
                var args = (JObject)token;
                var compareToField = MinecraftData.Uppercase((string)args.GetValue("compareTo")!);

                Dictionary<string, ProtoType> switchMap = new Dictionary<string, ProtoType>();

                foreach (var prop in ((JObject)args.GetValue("fields")!).Properties()) {
                    switchMap.Add(prop.Name, ProtoType.Parse(prop.Value));
                }

                ProtoType? defaultType = null;
                if (args.GetValue("default") != null) {
                    defaultType = ProtoType.Parse(args.GetValue("default")!);
                }

                return new ProtoSwitch(compareToField, switchMap, defaultType);
            }
        }

        public class ProtoMapper : ProtoType {

            public ProtoType Type;
            public Dictionary<string, ProtoType> Map = new Dictionary<string, ProtoType>();

            public ProtoMapper(ProtoType type, Dictionary<string, ProtoType> map) : base("Mapper", true) {
                this.Type = type;
                this.Map = map;
                this.StructureName = "Mapper";
            }

            public static new ProtoMapper Parse(JToken opts) {
                var args = (JObject)opts;

                var type = ProtoType.Parse(args.GetValue("type")!);

                Dictionary<string, ProtoType> map = new Dictionary<string, ProtoType>();
                foreach (var prop in ((JObject)args.GetValue("mappings")!).Properties()) {
                    map.Add(prop.Name, ProtoType.Parse(prop.Value));
                }
                return new ProtoMapper(type, map);

            }
        }

        public class ProtoBuffer : ProtoType {

            public ProtoType CountType;

            public ProtoBuffer(ProtoType countType) : base("Buffer", false) {
                this.CountType = countType;
            }

            public static new ProtoBuffer Parse(JToken opts) {
                var args = (JObject)opts;

                var countType = ProtoType.Parse(args.GetValue("countType")!);
                return new ProtoBuffer(countType);
            }
        }

        public class ProtoArray : ProtoType {

            public ProtoType? CountType;
            public ProtoType Type;
            public string? Count;

            public override string Reader => $"buffer.ReadArray<{Type.GetCSharpName()}>({(CountType == null ? $"(int){MinecraftData.Uppercase(Count!)}!" : CountType.Reader)})";
            //public override string Writer => $"buffer.WriteArray<{Type.GetCSharpName()}>(%value%)";

            public ProtoArray(ProtoType type, ProtoType? countType, string? count) : base("Array", false) {
                this.CountType = countType;
                this.Type = type;
                this.Count = count;
            }

            public static new ProtoArray Parse(JToken opts) {
                JObject args = (JObject)opts;
                ProtoType type = ProtoType.Parse(args.GetValue("type")!);
                ProtoType? countType = null;
                string? count = null;

                if (args.GetValue("countType") != null) {
                    countType = ProtoType.Parse(args.GetValue("countType")!);
                }

                if (args.GetValue("count") != null) {
                    count = (string)args.GetValue("count")!;
                }


                var arr = new ProtoArray(type, countType, count);
                arr.HasStructure = type.IsStructure;
                return arr;
            }
        }

        public class ProtoOption : ProtoType {
            public ProtoType OptionType;

            public override string Reader => $"buffer.ReadOption<{OptionType.GetCSharpName()}>()";

            public ProtoOption(ProtoType optionType) : base("Option", false) {
                OptionType = optionType;
            }

            public static new ProtoOption Parse(JToken opts) {
                var optionType = ProtoType.Parse(opts);
                var opt = new ProtoOption(optionType);
                opt.HasStructure = optionType.IsStructure;
                return opt;
            }
        }

        public class ProtoBitfield : ProtoType {

            public List<(string name, int size, bool signed)> Fields = new List<(string name, int size, bool signed)>();
            public int BitCount { get; set; }

            public ProtoBitfield(List<(string name, int size, bool signed)> fields) : base("Bitfield", true) {
                Fields = fields;
                BitCount = fields.Select(x => x.size).Aggregate((a, b) => a + b);
                this.StructureName = "Bitfield";
            }

            public static new ProtoBitfield Parse(JToken token) {
                JArray fields = (JArray)token;

                List<(string, int, bool)> f = new List<(string, int, bool)>();

                foreach (JObject field in fields) {
                    string name = (string)field.GetValue("name")!;
                    int size = (int)field.GetValue("size")!;
                    bool signed = (bool)field.GetValue("signed")!;
                    f.Add((name, size, signed));
                }

                return new ProtoBitfield(f);
            }
        }

        public class ProtoParticleData : ProtoType {

            public string CompareTo;

            public ProtoParticleData(string compareTo) : base("ParticleData", false) {
                CompareTo = compareTo;
            }

            public static new ProtoParticleData Parse(JToken opts) {
                JObject args = (JObject)opts;
                string compareTo = (string)args.GetValue("compareTo")!;
                return new ProtoParticleData(compareTo);
            }
        }

        public class ProtoTopBitSetTerminatedArray : ProtoType {

            public ProtoType Type; 
            public override string Reader => $"buffer.ReadTopBitSetTerminatedArray<{Type.GetCSharpName()}>()";

            public ProtoTopBitSetTerminatedArray(ProtoType type) : base("TopBitSetTerminatedArray", false) {
                this.Type = type;
            }

            public static new ProtoTopBitSetTerminatedArray Parse(JToken token) {
                JObject args = (JObject)token;
                var type = ProtoType.Parse(args.GetValue("type")!);

                var arr = new ProtoTopBitSetTerminatedArray(type);
                arr.HasStructure = type.IsStructure;
                return arr;
            }
        }

        #endregion

        public class ProtoField {
            public string Name { get; set; }
            public ProtoType Type { get; set; }

            public ProtoField(string name, ProtoType type) {
                this.Name = name;
                this.Type = type;
            }

            public static ProtoField Parse(JObject token) {
                var typeToken = token.GetValue("type")!;
                var type = ProtoType.Parse(typeToken);

                string? name = (string?)token.GetValue("name");
                if (name != null) name = MinecraftData.Uppercase(name);
                if (name == null) {
                    name = "Anon" + type.Name;
                }

                if (type.IsStructure) {
                    type.StructureName = name + type.Name;
                }

                if (type.HasStructure) {
                    switch (type) {
                        case ProtoOption opt:
                            opt.OptionType.StructureName = name + opt.OptionType.Name;
                            break;
                        case ProtoArray arr:
                            arr.Type.StructureName = name + arr.Type.Name;
                            break;
                        case ProtoTopBitSetTerminatedArray tbsta:
                            tbsta.Type.StructureName = name + tbsta.Type.Name;
                            break;
                    }
                }

                return new ProtoField(name, type);
            }
        }

        public static string GetBitfieldType(int size, bool signed) {
            var type = size switch {
                (> 0 and <= 9) => "sbyte",
                (> 8 and <= 16) => "short",
                (> 16 and <= 32) => "int",
                (> 32 and <= 64) => "long",
                _ => throw new Exception(size.ToString())
            };

            if (!signed) {
                type = type switch {
                    "sbyte" => "byte",
                    _ => "u" + type,
                };
            }
            return type;
        }
    }
}
