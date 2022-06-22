using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class DeclareCommandsPacket : Packet {

        public CommandNode[] Nodes { get; private set; }
        public int RootIndex { get; private set; }

        public DeclareCommandsPacket() { }

        public DeclareCommandsPacket(CommandNode[] nodes, int rootIndex) {
            Nodes = nodes;
            this.RootIndex = rootIndex;
        }

        public override void Read(PacketBuffer buffer) {
            int length = buffer.ReadVarInt();
            Nodes = new CommandNode[length];
            for (int i = 0; i < length; i++) Nodes[i] = ReadNode(buffer);
            RootIndex = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }

        private CommandNode ReadNode(PacketBuffer buffer) {
            byte flags = buffer.ReadByte();

            int childCount = buffer.ReadVarInt();
            int[] children = new int[childCount];
            int? redirectNode = null;
            string? name = null;
            Identifier? parser = null;
            dynamic? properties = null;
            Identifier? suggestionType = null;

            for (int i = 0; i < childCount; i++) children[i] = buffer.ReadVarInt();

            if ((flags & 0x08) == 0x08) redirectNode = buffer.ReadVarInt();

            switch ((flags & 0x03)) {
                case 0: break;
                case 1: name = buffer.ReadString(); break;
                case 2:
                    name = buffer.ReadString();
                    parser = buffer.ReadIdentifier();

                    byte propFlags = 0;
                    object? min = null;
                    object? max = null;
                    switch (parser.ToString()) {
                        case "brigadier:double":
                            propFlags = buffer.ReadByte();
                            min = -double.MaxValue;
                            max = double.MaxValue;
                            if ((propFlags & 0x01) == 0x01) min = buffer.ReadDouble();
                            if ((propFlags & 0x02) == 0x02) max = buffer.ReadDouble();
                            goto case "brigadier:number";

                        case "brigadier:float":
                            propFlags = buffer.ReadByte();
                            min = -float.MaxValue;
                            max = float.MaxValue;
                            if ((propFlags & 0x01) == 0x01) min = buffer.ReadFloat();
                            if ((propFlags & 0x02) == 0x02) max = buffer.ReadFloat();
                            goto case "brigadier:number";

                        case "brigadier:integer":
                            propFlags = buffer.ReadByte();
                            min = -int.MaxValue;
                            max = int.MaxValue;
                            if ((propFlags & 0x01) == 0x01) min = buffer.ReadInt();
                            if ((propFlags & 0x02) == 0x02) max = buffer.ReadInt();
                            goto case "brigadier:number";

                        case "brigadier:long":
                            propFlags = buffer.ReadByte();
                            min = -long.MaxValue;
                            max = long.MaxValue;
                            if ((propFlags & 0x01) == 0x01) min = buffer.ReadLong();
                            if ((propFlags & 0x02) == 0x02) max = buffer.ReadLong();
                            goto case "brigadier:number";

                        case "brigadier:number":
                            properties = new { Flags = propFlags, Min = min, Max = max };
                            break;

                        case "brigadier:string":
                            BrigadierStringMode stringMode = (BrigadierStringMode)buffer.ReadVarInt();
                            properties = new { StringMode = stringMode };
                            break;

                        case "minecraft:entity":
                        case "minecraft:score_holder":
                            propFlags = buffer.ReadByte();
                            properties = new { Flags = propFlags };
                            break;
                        case "minecraft:range":
                            throw new NotImplementedException();
                    }
                    break;
            }

            if ((flags & 0x10) == 0x10) suggestionType = buffer.ReadIdentifier();

            return new CommandNode(flags, children, redirectNode, name, parser, properties, suggestionType);
        }
    }
}
