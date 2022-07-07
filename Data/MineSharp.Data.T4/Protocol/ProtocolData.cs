

////////////////////////////////////////////////////////////
//  Generated Protocol Data for Minecraft Version 1.18.1  //
////////////////////////////////////////////////////////////

using MineSharp.Core.Types;
using MineSharp.Core.Types.Protocol;
using fNbt;

namespace MineSharp.Data.Protocol {

    public abstract partial class PacketBuffer {

        public abstract void Write<T>(T value);
        public abstract T Read<T>();
        public abstract T Read<T>(T payload, params object?[]? args) where T : IPacketPayload;
        public abstract object? ReadVoid();
        public abstract void WriteVoid(object? value);
        public abstract T[] ReadArray<T>(int length, params object?[]? args);
        public abstract void WriteArray<T>(T[] value);
        public abstract MinecraftSmeltingFormat ReadMinecraftSmeltingFormat();
        public abstract void WriteMinecraftSmeltingFormat(MinecraftSmeltingFormat value);
        public abstract Tags ReadTags();
        public abstract void WriteTags(Tags value);
        public abstract Ingredient ReadIngredient();
        public abstract void WriteIngredient(Ingredient value);


        public abstract string ReadString();
        public abstract long ReadI64();
        public abstract byte[] ReadBuffer();
        public abstract UUID ReadUUID();
        public abstract int ReadVarint();
        public abstract byte[] ReadRestbuffer();
        public abstract double ReadF64();
        public abstract sbyte ReadI8();
        public abstract int ReadI32();
        public abstract short ReadI16();
        public abstract Position ReadPosition();
        public abstract byte ReadU8();
        public abstract bool ReadBool();
        public abstract NbtCompound ReadOptionalnbt();
        public abstract float ReadF32();
        public abstract Slot ReadSlot();
        public abstract NbtCompound ReadNbt();
        public abstract ParticleData ReadParticleData();
        public abstract T ReadOption<T>();
        public abstract Entitymetadata ReadEntitymetadata();
        public abstract TopBitSetTerminatedArray<T> ReadTopBitSetTerminatedArray<T>();
        public abstract ushort ReadU16();

        public abstract void WriteString(string value);
        public abstract void WriteI64(long value);
        public abstract void WriteBuffer(byte[] value);
        public abstract void WriteUUID(UUID value);
        public abstract void WriteVarint(int value);
        public abstract void WriteRestbuffer(byte[] value);
        public abstract void WriteF64(double value);
        public abstract void WriteI8(sbyte value);
        public abstract void WriteI32(int value);
        public abstract void WriteI16(short value);
        public abstract void WritePosition(Position value);
        public abstract void WriteU8(byte value);
        public abstract void WriteBool(bool value);
        public abstract void WriteOptionalnbt(NbtCompound value);
        public abstract void WriteF32(float value);
        public abstract void WriteSlot(Slot value);
        public abstract void WriteNbt(NbtCompound value);
        public abstract void WriteParticleData(ParticleData value);
        public abstract void WriteOption<T>(T value);
        public abstract void WriteEntitymetadata(Entitymetadata value);
        public abstract void WriteTopBitSetTerminatedArray<T>(TopBitSetTerminatedArray<T> value);
        public abstract void WriteU16(ushort value);
    }


}


namespace MineSharp.Data.Protocol.Clientbound.Handshaking {


}
namespace MineSharp.Data.Protocol.Clientbound.Status {



    public class ServerInfoPacket : Packet {
        public const int ProtocolId = 0x00;

        public string? Response { get; set; }

        public ServerInfoPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Response = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Response!);
        }

    }



    public class PingPacket : Packet {
        public const int ProtocolId = 0x01;

        public long? Time { get; set; }

        public PingPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Time = buffer.ReadI64();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI64((long)Time!);
        }

    }


}
namespace MineSharp.Data.Protocol.Clientbound.Login {



    public class DisconnectPacket : Packet {
        public const int ProtocolId = 0x00;

        public string? Reason { get; set; }

        public DisconnectPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Reason = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Reason!);
        }

    }



    public class EncryptionBeginPacket : Packet {
        public const int ProtocolId = 0x01;

        public string? ServerId { get; set; }
        public byte[]? PublicKey { get; set; }
        public byte[]? VerifyToken { get; set; }

        public EncryptionBeginPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ServerId = buffer.ReadString();
            this.PublicKey = buffer.ReadBuffer();
            this.VerifyToken = buffer.ReadBuffer();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)ServerId!);
            buffer.WriteBuffer((byte[])PublicKey!);
            buffer.WriteBuffer((byte[])VerifyToken!);
        }

    }



    public class SuccessPacket : Packet {
        public const int ProtocolId = 0x02;

        public UUID? Uuid { get; set; }
        public string? Username { get; set; }

        public SuccessPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Uuid = buffer.ReadUUID();
            this.Username = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteUUID((UUID)Uuid!);
            buffer.WriteString((string)Username!);
        }

    }



    public class CompressPacket : Packet {
        public const int ProtocolId = 0x03;

        public int? Threshold { get; set; }

        public CompressPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Threshold = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Threshold!);
        }

    }



    public class LoginPluginRequestPacket : Packet {
        public const int ProtocolId = 0x04;

        public int? MessageId { get; set; }
        public string? Channel { get; set; }
        public byte[]? Data { get; set; }

        public LoginPluginRequestPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.MessageId = buffer.ReadVarint();
            this.Channel = buffer.ReadString();
            this.Data = buffer.ReadRestbuffer();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)MessageId!);
            buffer.WriteString((string)Channel!);
            buffer.WriteRestbuffer((byte[])Data!);
        }

    }


}
namespace MineSharp.Data.Protocol.Clientbound.Play {



    public class SpawnEntityPacket : Packet {
        public const int ProtocolId = 0x00;

        public int? EntityId { get; set; }
        public UUID? ObjectUUID { get; set; }
        public int? Type { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public sbyte? Pitch { get; set; }
        public sbyte? Yaw { get; set; }
        public int? ObjectData { get; set; }
        public short? VelocityX { get; set; }
        public short? VelocityY { get; set; }
        public short? VelocityZ { get; set; }

        public SpawnEntityPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.ObjectUUID = buffer.ReadUUID();
            this.Type = buffer.ReadVarint();
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.Pitch = buffer.ReadI8();
            this.Yaw = buffer.ReadI8();
            this.ObjectData = buffer.ReadI32();
            this.VelocityX = buffer.ReadI16();
            this.VelocityY = buffer.ReadI16();
            this.VelocityZ = buffer.ReadI16();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteUUID((UUID)ObjectUUID!);
            buffer.WriteVarint((int)Type!);
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteI8((sbyte)Pitch!);
            buffer.WriteI8((sbyte)Yaw!);
            buffer.WriteI32((int)ObjectData!);
            buffer.WriteI16((short)VelocityX!);
            buffer.WriteI16((short)VelocityY!);
            buffer.WriteI16((short)VelocityZ!);
        }

    }



    public class SpawnEntityExperienceOrbPacket : Packet {
        public const int ProtocolId = 0x01;

        public int? EntityId { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public short? Count { get; set; }

        public SpawnEntityExperienceOrbPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.Count = buffer.ReadI16();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteI16((short)Count!);
        }

    }



    public class SpawnEntityLivingPacket : Packet {
        public const int ProtocolId = 0x02;

        public int? EntityId { get; set; }
        public UUID? EntityUUID { get; set; }
        public int? Type { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public sbyte? Yaw { get; set; }
        public sbyte? Pitch { get; set; }
        public sbyte? HeadPitch { get; set; }
        public short? VelocityX { get; set; }
        public short? VelocityY { get; set; }
        public short? VelocityZ { get; set; }

        public SpawnEntityLivingPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.EntityUUID = buffer.ReadUUID();
            this.Type = buffer.ReadVarint();
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.Yaw = buffer.ReadI8();
            this.Pitch = buffer.ReadI8();
            this.HeadPitch = buffer.ReadI8();
            this.VelocityX = buffer.ReadI16();
            this.VelocityY = buffer.ReadI16();
            this.VelocityZ = buffer.ReadI16();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteUUID((UUID)EntityUUID!);
            buffer.WriteVarint((int)Type!);
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteI8((sbyte)Yaw!);
            buffer.WriteI8((sbyte)Pitch!);
            buffer.WriteI8((sbyte)HeadPitch!);
            buffer.WriteI16((short)VelocityX!);
            buffer.WriteI16((short)VelocityY!);
            buffer.WriteI16((short)VelocityZ!);
        }

    }



    public class SpawnEntityPaintingPacket : Packet {
        public const int ProtocolId = 0x03;

        public int? EntityId { get; set; }
        public UUID? EntityUUID { get; set; }
        public int? Title { get; set; }
        public Position? Location { get; set; }
        public byte? Direction { get; set; }

        public SpawnEntityPaintingPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.EntityUUID = buffer.ReadUUID();
            this.Title = buffer.ReadVarint();
            this.Location = buffer.ReadPosition();
            this.Direction = buffer.ReadU8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteUUID((UUID)EntityUUID!);
            buffer.WriteVarint((int)Title!);
            buffer.WritePosition((Position)Location!);
            buffer.WriteU8((byte)Direction!);
        }

    }



    public class NamedEntitySpawnPacket : Packet {
        public const int ProtocolId = 0x04;

        public int? EntityId { get; set; }
        public UUID? PlayerUUID { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public sbyte? Yaw { get; set; }
        public sbyte? Pitch { get; set; }

        public NamedEntitySpawnPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.PlayerUUID = buffer.ReadUUID();
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.Yaw = buffer.ReadI8();
            this.Pitch = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteUUID((UUID)PlayerUUID!);
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteI8((sbyte)Yaw!);
            buffer.WriteI8((sbyte)Pitch!);
        }

    }



    public class AnimationPacket : Packet {
        public const int ProtocolId = 0x06;

        public int? EntityId { get; set; }
        public byte? Animation { get; set; }

        public AnimationPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.Animation = buffer.ReadU8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteU8((byte)Animation!);
        }

    }



    public class StatisticsPacket : Packet {
        public const int ProtocolId = 0x07;

        public EntriesContainer[]? Entries { get; set; }

        public StatisticsPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Entries = buffer.ReadArray<EntriesContainer>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteArray((EntriesContainer[])Entries!);
        }

        public class EntriesContainer : IPacketPayload {
            public int? CategoryId { get; set; }
            public int? StatisticId { get; set; }
            public int? Value { get; set; }

            public EntriesContainer() { }

            public void Read(PacketBuffer buffer) {
                this.CategoryId = buffer.ReadVarint();
                this.StatisticId = buffer.ReadVarint();
                this.Value = buffer.ReadVarint();
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteVarint((int)CategoryId!);
                buffer.WriteVarint((int)StatisticId!);
                buffer.WriteVarint((int)Value!);
            }


        }

    }



    public class AdvancementsPacket : Packet {
        public const int ProtocolId = 0x63;

        public bool? Reset { get; set; }
        public AdvancementMappingContainer[]? AdvancementMapping { get; set; }
        public string[]? Identifiers { get; set; }
        public ProgressMappingContainer[]? ProgressMapping { get; set; }

        public AdvancementsPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Reset = buffer.ReadBool();
            this.AdvancementMapping = buffer.ReadArray<AdvancementMappingContainer>(buffer.ReadVarint());
            this.Identifiers = buffer.ReadArray<string>(buffer.ReadVarint());
            this.ProgressMapping = buffer.ReadArray<ProgressMappingContainer>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBool((bool)Reset!);
            buffer.WriteArray((AdvancementMappingContainer[])AdvancementMapping!);
            buffer.WriteArray((string[])Identifiers!);
            buffer.WriteArray((ProgressMappingContainer[])ProgressMapping!);
        }

        public class AdvancementMappingContainer : IPacketPayload {
            public string? Key { get; set; }
            public ValueContainer? Value { get; set; }

            public AdvancementMappingContainer() { }

            public void Read(PacketBuffer buffer) {
                this.Key = buffer.ReadString();
                this.Value = buffer.Read(new ValueContainer());
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteString((string)Key!);
                buffer.Write((ValueContainer)Value!);
            }

            public class ValueContainer : IPacketPayload {
                public string? ParentId { get; set; }
                public DisplayDataContainer? DisplayData { get; set; }
                public CriteriaContainer[]? Criteria { get; set; }
                public string[][]? Requirements { get; set; }

                public ValueContainer() { }

                public void Read(PacketBuffer buffer) {
                    this.ParentId = buffer.ReadOption<string>();
                    this.DisplayData = buffer.ReadOption<DisplayDataContainer>();
                    this.Criteria = buffer.ReadArray<CriteriaContainer>(buffer.ReadVarint());
                    this.Requirements = buffer.ReadArray<string[]>(buffer.ReadVarint());
                }

                public void Write(PacketBuffer buffer) {
                    buffer.WriteOption((string)ParentId!);
                    buffer.WriteOption((DisplayDataContainer)DisplayData!);
                    buffer.WriteArray((CriteriaContainer[])Criteria!);
                    buffer.WriteArray((string[][])Requirements!);
                }

                public class DisplayDataContainer : IPacketPayload {
                    public string? Title { get; set; }
                    public string? Description { get; set; }
                    public Slot? Icon { get; set; }
                    public int? FrameType { get; set; }
                    public FlagsBitfield? Flags { get; set; }
                    public BackgroundTextureSwitch? BackgroundTexture { get; set; }
                    public float? XCord { get; set; }
                    public float? YCord { get; set; }

                    public DisplayDataContainer() { }

                    public void Read(PacketBuffer buffer) {
                        this.Title = buffer.ReadString();
                        this.Description = buffer.ReadString();
                        this.Icon = buffer.ReadSlot();
                        this.FrameType = buffer.ReadVarint();
                        this.Flags = buffer.Read<FlagsBitfield>(new FlagsBitfield());
                        this.BackgroundTexture = buffer.Read<BackgroundTextureSwitch>(new BackgroundTextureSwitch((int)Flags.HasBackgroundTexture!));
                        this.XCord = buffer.ReadF32();
                        this.YCord = buffer.ReadF32();
                    }

                    public void Write(PacketBuffer buffer) {
                        buffer.WriteString((string)Title!);
                        buffer.WriteString((string)Description!);
                        buffer.WriteSlot((Slot)Icon!);
                        buffer.WriteVarint((int)FrameType!);
                        buffer.Write((FlagsBitfield)Flags!);
                        buffer.Write((BackgroundTextureSwitch)BackgroundTexture!);
                        buffer.WriteF32((float)XCord!);
                        buffer.WriteF32((float)YCord!);
                    }

                    public class FlagsBitfield : IPacketPayload {

                        public uint? Value { get; set; }

                        public uint Unused {
                            get { return (uint)(((uint)Value! >> 3) & (536870911)); }
                            set { var val = value << 3; var inv = ~val; var x = (uint)this.Value! & (uint)inv; this.Value = (uint)(x | (uint)val); }
                        }
                        public byte Hidden {
                            get { return (byte)(((uint)Value! >> 2) & (1)); }
                            set { var val = value << 2; var inv = ~val; var x = (uint)this.Value! & (uint)inv; this.Value = (uint)(x | (byte)val); }
                        }
                        public byte ShowToast {
                            get { return (byte)(((uint)Value! >> 1) & (1)); }
                            set { var val = value << 1; var inv = ~val; var x = (uint)this.Value! & (uint)inv; this.Value = (uint)(x | (byte)val); }
                        }
                        public byte HasBackgroundTexture {
                            get { return (byte)(((uint)Value! >> 0) & (1)); }
                            set { var val = value << 0; var inv = ~val; var x = (uint)this.Value! & (uint)inv; this.Value = (uint)(x | (byte)val); }
                        }
                        public void Write(PacketBuffer buffer) {
                            buffer.Write(this.Value!);
                        }

                        public void Read(PacketBuffer buffer) {
                            this.Value = buffer.Read<uint>();
                        }

                    }

                    public class BackgroundTextureSwitch : IPacketPayload {

                        public object? Value { get; set; }
                        public int SwitchState { get; set; }

                        public BackgroundTextureSwitch(object? value, int switchState) {
                            this.Value = value;
                            this.SwitchState = switchState;
                        }

                        public BackgroundTextureSwitch(int switchState) {
                            this.SwitchState = switchState;
                        }

                        public void Read(PacketBuffer buffer) {

                            this.Value = SwitchState switch {
                                1 => buffer.ReadString(),
                                _ => buffer.ReadVoid()
                            };

                        }

                        public void Write(PacketBuffer buffer) {
                            switch (SwitchState) {
                                case 1: buffer.WriteString((string)this.Value!); break;

                            }

                        }


                    }


                }

                public class CriteriaContainer : IPacketPayload {
                    public string? Key { get; set; }
                    public object? Value { get; set; }

                    public CriteriaContainer() { }

                    public void Read(PacketBuffer buffer) {
                        this.Key = buffer.ReadString();
                        this.Value = buffer.ReadVoid();
                    }

                    public void Write(PacketBuffer buffer) {
                        buffer.WriteString((string)Key!);
                        buffer.WriteVoid((object)Value!);
                    }


                }


            }


        }

        public class ProgressMappingContainer : IPacketPayload {
            public string? Key { get; set; }
            public ValueContainer[]? Value { get; set; }

            public ProgressMappingContainer() { }

            public void Read(PacketBuffer buffer) {
                this.Key = buffer.ReadString();
                this.Value = buffer.ReadArray<ValueContainer>(buffer.ReadVarint());
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteString((string)Key!);
                buffer.WriteArray((ValueContainer[])Value!);
            }

            public class ValueContainer : IPacketPayload {
                public string? CriterionIdentifier { get; set; }
                public long? CriterionProgress { get; set; }

                public ValueContainer() { }

                public void Read(PacketBuffer buffer) {
                    this.CriterionIdentifier = buffer.ReadString();
                    this.CriterionProgress = buffer.ReadOption<long>();
                }

                public void Write(PacketBuffer buffer) {
                    buffer.WriteString((string)CriterionIdentifier!);
                    buffer.WriteOption((long)CriterionProgress!);
                }


            }


        }

    }



    public class BlockBreakAnimationPacket : Packet {
        public const int ProtocolId = 0x09;

        public int? EntityId { get; set; }
        public Position? Location { get; set; }
        public sbyte? DestroyStage { get; set; }

        public BlockBreakAnimationPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.Location = buffer.ReadPosition();
            this.DestroyStage = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WritePosition((Position)Location!);
            buffer.WriteI8((sbyte)DestroyStage!);
        }

    }



    public class TileEntityDataPacket : Packet {
        public const int ProtocolId = 0x0a;

        public Position? Location { get; set; }
        public int? Action { get; set; }
        public NbtCompound? NbtData { get; set; }

        public TileEntityDataPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Action = buffer.ReadVarint();
            this.NbtData = buffer.ReadOptionalnbt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteVarint((int)Action!);
            buffer.WriteOptionalnbt((NbtCompound)NbtData!);
        }

    }



    public class BlockActionPacket : Packet {
        public const int ProtocolId = 0x0b;

        public Position? Location { get; set; }
        public byte? Byte1 { get; set; }
        public byte? Byte2 { get; set; }
        public int? BlockId { get; set; }

        public BlockActionPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Byte1 = buffer.ReadU8();
            this.Byte2 = buffer.ReadU8();
            this.BlockId = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteU8((byte)Byte1!);
            buffer.WriteU8((byte)Byte2!);
            buffer.WriteVarint((int)BlockId!);
        }

    }



    public class BlockChangePacket : Packet {
        public const int ProtocolId = 0x0c;

        public Position? Location { get; set; }
        public int? Type { get; set; }

        public BlockChangePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Type = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteVarint((int)Type!);
        }

    }



    public class BossBarPacket : Packet {
        public const int ProtocolId = 0x0d;

        public UUID? EntityUUID { get; set; }
        public int? Action { get; set; }
        public TitleSwitch? Title { get; set; }
        public HealthSwitch? Health { get; set; }
        public ColorSwitch? Color { get; set; }
        public DividersSwitch? Dividers { get; set; }
        public FlagsSwitch? Flags { get; set; }

        public BossBarPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityUUID = buffer.ReadUUID();
            this.Action = buffer.ReadVarint();
            this.Title = buffer.Read<TitleSwitch>(new TitleSwitch((int)Action!));
            this.Health = buffer.Read<HealthSwitch>(new HealthSwitch((int)Action!));
            this.Color = buffer.Read<ColorSwitch>(new ColorSwitch((int)Action!));
            this.Dividers = buffer.Read<DividersSwitch>(new DividersSwitch((int)Action!));
            this.Flags = buffer.Read<FlagsSwitch>(new FlagsSwitch((int)Action!));
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteUUID((UUID)EntityUUID!);
            buffer.WriteVarint((int)Action!);
            buffer.Write((TitleSwitch)Title!);
            buffer.Write((HealthSwitch)Health!);
            buffer.Write((ColorSwitch)Color!);
            buffer.Write((DividersSwitch)Dividers!);
            buffer.Write((FlagsSwitch)Flags!);
        }

        public class TitleSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public TitleSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public TitleSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadString(),
                    3 => buffer.ReadString(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteString((string)this.Value!); break;
                    case 3: buffer.WriteString((string)this.Value!); break;

                }

            }


        }

        public class HealthSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public HealthSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public HealthSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadF32(),
                    2 => buffer.ReadF32(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteF32((float)this.Value!); break;
                    case 2: buffer.WriteF32((float)this.Value!); break;

                }

            }


        }

        public class ColorSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public ColorSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public ColorSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadVarint(),
                    4 => buffer.ReadVarint(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteVarint((int)this.Value!); break;
                    case 4: buffer.WriteVarint((int)this.Value!); break;

                }

            }


        }

        public class DividersSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public DividersSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public DividersSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadVarint(),
                    4 => buffer.ReadVarint(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteVarint((int)this.Value!); break;
                    case 4: buffer.WriteVarint((int)this.Value!); break;

                }

            }


        }

        public class FlagsSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public FlagsSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public FlagsSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadU8(),
                    5 => buffer.ReadU8(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteU8((byte)this.Value!); break;
                    case 5: buffer.WriteU8((byte)this.Value!); break;

                }

            }


        }

    }



    public class DifficultyPacket : Packet {
        public const int ProtocolId = 0x0e;

        public byte? Difficulty { get; set; }
        public bool? DifficultyLocked { get; set; }

        public DifficultyPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Difficulty = buffer.ReadU8();
            this.DifficultyLocked = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)Difficulty!);
            buffer.WriteBool((bool)DifficultyLocked!);
        }

    }



    public class TabCompletePacket : Packet {
        public const int ProtocolId = 0x11;

        public int? TransactionId { get; set; }
        public int? Start { get; set; }
        public int? Length { get; set; }
        public MatchesContainer[]? Matches { get; set; }

        public TabCompletePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.TransactionId = buffer.ReadVarint();
            this.Start = buffer.ReadVarint();
            this.Length = buffer.ReadVarint();
            this.Matches = buffer.ReadArray<MatchesContainer>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)TransactionId!);
            buffer.WriteVarint((int)Start!);
            buffer.WriteVarint((int)Length!);
            buffer.WriteArray((MatchesContainer[])Matches!);
        }

        public class MatchesContainer : IPacketPayload {
            public string? Match { get; set; }
            public string? Tooltip { get; set; }

            public MatchesContainer() { }

            public void Read(PacketBuffer buffer) {
                this.Match = buffer.ReadString();
                this.Tooltip = buffer.ReadOption<string>();
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteString((string)Match!);
                buffer.WriteOption((string)Tooltip!);
            }


        }

    }



    public class DeclareCommandsPacket : Packet {
        public const int ProtocolId = 0x12;

        public NodesContainer[]? Nodes { get; set; }
        public int? RootIndex { get; set; }

        public DeclareCommandsPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Nodes = buffer.ReadArray<NodesContainer>(buffer.ReadVarint());
            this.RootIndex = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteArray((NodesContainer[])Nodes!);
            buffer.WriteVarint((int)RootIndex!);
        }

        public class NodesContainer : IPacketPayload {
            public FlagsBitfield? Flags { get; set; }
            public int[]? Children { get; set; }
            public RedirectNodeSwitch? RedirectNode { get; set; }
            public ExtraNodeDataSwitch? ExtraNodeData { get; set; }

            public NodesContainer() { }

            public void Read(PacketBuffer buffer) {
                this.Flags = buffer.Read<FlagsBitfield>(new FlagsBitfield());
                this.Children = buffer.ReadArray<int>(buffer.ReadVarint());
                this.RedirectNode = buffer.Read<RedirectNodeSwitch>(new RedirectNodeSwitch((int)Flags.HasRedirectNode!));
                this.ExtraNodeData = buffer.Read<ExtraNodeDataSwitch>(new ExtraNodeDataSwitch((int)Flags.CommandNodeType!, Flags));
            }

            public void Write(PacketBuffer buffer) {
                buffer.Write((FlagsBitfield)Flags!);
                buffer.WriteArray((int[])Children!);
                buffer.Write((RedirectNodeSwitch)RedirectNode!);
                buffer.Write((ExtraNodeDataSwitch)ExtraNodeData!);
            }

            public class FlagsBitfield : IPacketPayload {

                public byte? Value { get; set; }

                public byte Unused {
                    get { return (byte)(((byte)Value! >> 5) & (7)); }
                    set { var val = value << 5; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                }
                public byte HasCustomSuggestions {
                    get { return (byte)(((byte)Value! >> 4) & (1)); }
                    set { var val = value << 4; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                }
                public byte HasRedirectNode {
                    get { return (byte)(((byte)Value! >> 3) & (1)); }
                    set { var val = value << 3; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                }
                public byte HasCommand {
                    get { return (byte)(((byte)Value! >> 2) & (1)); }
                    set { var val = value << 2; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                }
                public byte CommandNodeType {
                    get { return (byte)(((byte)Value! >> 0) & (3)); }
                    set { var val = value << 0; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                }
                public void Write(PacketBuffer buffer) {
                    buffer.Write(this.Value!);
                }

                public void Read(PacketBuffer buffer) {
                    this.Value = buffer.Read<byte>();
                }

            }

            public class RedirectNodeSwitch : IPacketPayload {

                public object? Value { get; set; }
                public int SwitchState { get; set; }

                public RedirectNodeSwitch(object? value, int switchState) {
                    this.Value = value;
                    this.SwitchState = switchState;
                }

                public RedirectNodeSwitch(int switchState) {
                    this.SwitchState = switchState;
                }

                public void Read(PacketBuffer buffer) {

                    this.Value = SwitchState switch {
                        1 => buffer.ReadVarint(),
                        _ => buffer.ReadVoid()
                    };

                }

                public void Write(PacketBuffer buffer) {
                    switch (SwitchState) {
                        case 1: buffer.WriteVarint((int)this.Value!); break;

                    }

                }


            }

            public class ExtraNodeDataSwitch : IPacketPayload {

                public object? Value { get; set; }
                public int SwitchState { get; set; }

                private FlagsBitfield Flags { get; set; }

                public ExtraNodeDataSwitch(object? value, int switchState, FlagsBitfield flags) {
                    this.Value = value;
                    this.SwitchState = switchState;
                    this.Flags = flags;
                }

                public ExtraNodeDataSwitch(int switchState, FlagsBitfield flags) {
                    this.SwitchState = switchState;
                    this.Flags = flags;
                }

                public void Read(PacketBuffer buffer) {

                    this.Value = SwitchState switch {
                        0 => buffer.ReadVoid(),
                        1 => buffer.ReadString(),
                        2 => buffer.Read(new Container(Flags)),
                        _ => throw new Exception()
                    };

                }

                public void Write(PacketBuffer buffer) {
                    switch (SwitchState) {
                        case 0: buffer.WriteVoid((object)this.Value!); break;
                        case 1: buffer.WriteString((string)this.Value!); break;
                        case 2: buffer.Write((Container)this.Value!); break;

                    }

                }

                public class Container : IPacketPayload {
                    public string? Name { get; set; }
                    public string? Parser { get; set; }
                    public PropertiesSwitch? Properties { get; set; }
                    public SuggestsSwitch? Suggests { get; set; }

                    private FlagsBitfield Flags { get; set; }
                    public Container(FlagsBitfield flags) {
                        this.Flags = flags;
                    }

                    public void Read(PacketBuffer buffer) {
                        this.Name = buffer.ReadString();
                        this.Parser = buffer.ReadString();
                        this.Properties = buffer.Read<PropertiesSwitch>(new PropertiesSwitch((string)Parser!));
                        this.Suggests = buffer.Read<SuggestsSwitch>(new SuggestsSwitch((int)Flags.HasCustomSuggestions!));
                    }

                    public void Write(PacketBuffer buffer) {
                        buffer.WriteString((string)Name!);
                        buffer.WriteString((string)Parser!);
                        buffer.Write((PropertiesSwitch)Properties!);
                        buffer.Write((SuggestsSwitch)Suggests!);
                    }

                    public class PropertiesSwitch : IPacketPayload {

                        public object? Value { get; set; }
                        public string SwitchState { get; set; }

                        public PropertiesSwitch(object? value, string switchState) {
                            this.Value = value;
                            this.SwitchState = switchState;
                        }

                        public PropertiesSwitch(string switchState) {
                            this.SwitchState = switchState;
                        }

                        public void Read(PacketBuffer buffer) {

                            this.Value = SwitchState switch {
                                "brigadier:double" => buffer.Read(new Container()),
                                "brigadier:float" => buffer.Read(new Container()),
                                "brigadier:integer" => buffer.Read(new Container()),
                                "brigadier:long" => buffer.Read(new Container()),
                                "brigadier:string" => buffer.ReadVarint(),
                                "minecraft:entity" => buffer.ReadI8(),
                                "minecraft:score_holder" => buffer.ReadI8(),
                                "minecraft:range" => buffer.ReadBool(),
                                _ => buffer.ReadVoid()
                            };

                        }

                        public void Write(PacketBuffer buffer) {
                            switch (SwitchState) {
                                case "brigadier:double": buffer.Write((Container)this.Value!); break;
                                case "brigadier:float": buffer.Write((Container)this.Value!); break;
                                case "brigadier:integer": buffer.Write((Container)this.Value!); break;
                                case "brigadier:long": buffer.Write((Container)this.Value!); break;
                                case "brigadier:string": buffer.WriteVarint((int)this.Value!); break;
                                case "minecraft:entity": buffer.WriteI8((sbyte)this.Value!); break;
                                case "minecraft:score_holder": buffer.WriteI8((sbyte)this.Value!); break;
                                case "minecraft:range": buffer.WriteBool((bool)this.Value!); break;

                            }

                        }

                        public class Container : IPacketPayload {
                            public FlagsBitfield? Flags { get; set; }
                            public MinSwitch? Min { get; set; }
                            public MaxSwitch? Max { get; set; }

                            public Container() { }

                            public void Read(PacketBuffer buffer) {
                                this.Flags = buffer.Read<FlagsBitfield>(new FlagsBitfield());
                                this.Min = buffer.Read<MinSwitch>(new MinSwitch((int)Flags.MinPresent!));
                                this.Max = buffer.Read<MaxSwitch>(new MaxSwitch((int)Flags.MaxPresent!));
                            }

                            public void Write(PacketBuffer buffer) {
                                buffer.Write((FlagsBitfield)Flags!);
                                buffer.Write((MinSwitch)Min!);
                                buffer.Write((MaxSwitch)Max!);
                            }

                            public class FlagsBitfield : IPacketPayload {

                                public byte? Value { get; set; }

                                public byte Unused {
                                    get { return (byte)(((byte)Value! >> 2) & (63)); }
                                    set { var val = value << 2; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public byte MaxPresent {
                                    get { return (byte)(((byte)Value! >> 1) & (1)); }
                                    set { var val = value << 1; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public byte MinPresent {
                                    get { return (byte)(((byte)Value! >> 0) & (1)); }
                                    set { var val = value << 0; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public void Write(PacketBuffer buffer) {
                                    buffer.Write(this.Value!);
                                }

                                public void Read(PacketBuffer buffer) {
                                    this.Value = buffer.Read<byte>();
                                }

                            }

                            public class MinSwitch : IPacketPayload {

                                public object? Value { get; set; }
                                public int SwitchState { get; set; }

                                public MinSwitch(object? value, int switchState) {
                                    this.Value = value;
                                    this.SwitchState = switchState;
                                }

                                public MinSwitch(int switchState) {
                                    this.SwitchState = switchState;
                                }

                                public void Read(PacketBuffer buffer) {

                                    this.Value = SwitchState switch {
                                        1 => buffer.ReadF64(),
                                        _ => buffer.ReadVoid()
                                    };

                                }

                                public void Write(PacketBuffer buffer) {
                                    switch (SwitchState) {
                                        case 1: buffer.WriteF64((double)this.Value!); break;

                                    }

                                }


                            }

                            public class MaxSwitch : IPacketPayload {

                                public object? Value { get; set; }
                                public int SwitchState { get; set; }

                                public MaxSwitch(object? value, int switchState) {
                                    this.Value = value;
                                    this.SwitchState = switchState;
                                }

                                public MaxSwitch(int switchState) {
                                    this.SwitchState = switchState;
                                }

                                public void Read(PacketBuffer buffer) {

                                    this.Value = SwitchState switch {
                                        1 => buffer.ReadF64(),
                                        _ => buffer.ReadVoid()
                                    };

                                }

                                public void Write(PacketBuffer buffer) {
                                    switch (SwitchState) {
                                        case 1: buffer.WriteF64((double)this.Value!); break;

                                    }

                                }


                            }


                        }

                        public class Container1 : IPacketPayload {
                            public FlagsBitfield? Flags { get; set; }
                            public MinSwitch? Min { get; set; }
                            public MaxSwitch? Max { get; set; }

                            public Container1() { }

                            public void Read(PacketBuffer buffer) {
                                this.Flags = buffer.Read<FlagsBitfield>(new FlagsBitfield());
                                this.Min = buffer.Read<MinSwitch>(new MinSwitch((int)Flags.MinPresent!));
                                this.Max = buffer.Read<MaxSwitch>(new MaxSwitch((int)Flags.MaxPresent!));
                            }

                            public void Write(PacketBuffer buffer) {
                                buffer.Write((FlagsBitfield)Flags!);
                                buffer.Write((MinSwitch)Min!);
                                buffer.Write((MaxSwitch)Max!);
                            }

                            public class FlagsBitfield : IPacketPayload {

                                public byte? Value { get; set; }

                                public byte Unused {
                                    get { return (byte)(((byte)Value! >> 2) & (63)); }
                                    set { var val = value << 2; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public byte MaxPresent {
                                    get { return (byte)(((byte)Value! >> 1) & (1)); }
                                    set { var val = value << 1; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public byte MinPresent {
                                    get { return (byte)(((byte)Value! >> 0) & (1)); }
                                    set { var val = value << 0; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public void Write(PacketBuffer buffer) {
                                    buffer.Write(this.Value!);
                                }

                                public void Read(PacketBuffer buffer) {
                                    this.Value = buffer.Read<byte>();
                                }

                            }

                            public class MinSwitch : IPacketPayload {

                                public object? Value { get; set; }
                                public int SwitchState { get; set; }

                                public MinSwitch(object? value, int switchState) {
                                    this.Value = value;
                                    this.SwitchState = switchState;
                                }

                                public MinSwitch(int switchState) {
                                    this.SwitchState = switchState;
                                }

                                public void Read(PacketBuffer buffer) {

                                    this.Value = SwitchState switch {
                                        1 => buffer.ReadF32(),
                                        _ => buffer.ReadVoid()
                                    };

                                }

                                public void Write(PacketBuffer buffer) {
                                    switch (SwitchState) {
                                        case 1: buffer.WriteF32((float)this.Value!); break;

                                    }

                                }


                            }

                            public class MaxSwitch : IPacketPayload {

                                public object? Value { get; set; }
                                public int SwitchState { get; set; }

                                public MaxSwitch(object? value, int switchState) {
                                    this.Value = value;
                                    this.SwitchState = switchState;
                                }

                                public MaxSwitch(int switchState) {
                                    this.SwitchState = switchState;
                                }

                                public void Read(PacketBuffer buffer) {

                                    this.Value = SwitchState switch {
                                        1 => buffer.ReadF32(),
                                        _ => buffer.ReadVoid()
                                    };

                                }

                                public void Write(PacketBuffer buffer) {
                                    switch (SwitchState) {
                                        case 1: buffer.WriteF32((float)this.Value!); break;

                                    }

                                }


                            }


                        }

                        public class Container2 : IPacketPayload {
                            public FlagsBitfield? Flags { get; set; }
                            public MinSwitch? Min { get; set; }
                            public MaxSwitch? Max { get; set; }

                            public Container2() { }

                            public void Read(PacketBuffer buffer) {
                                this.Flags = buffer.Read<FlagsBitfield>(new FlagsBitfield());
                                this.Min = buffer.Read<MinSwitch>(new MinSwitch((int)Flags.MinPresent!));
                                this.Max = buffer.Read<MaxSwitch>(new MaxSwitch((int)Flags.MaxPresent!));
                            }

                            public void Write(PacketBuffer buffer) {
                                buffer.Write((FlagsBitfield)Flags!);
                                buffer.Write((MinSwitch)Min!);
                                buffer.Write((MaxSwitch)Max!);
                            }

                            public class FlagsBitfield : IPacketPayload {

                                public byte? Value { get; set; }

                                public byte Unused {
                                    get { return (byte)(((byte)Value! >> 2) & (63)); }
                                    set { var val = value << 2; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public byte MaxPresent {
                                    get { return (byte)(((byte)Value! >> 1) & (1)); }
                                    set { var val = value << 1; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public byte MinPresent {
                                    get { return (byte)(((byte)Value! >> 0) & (1)); }
                                    set { var val = value << 0; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public void Write(PacketBuffer buffer) {
                                    buffer.Write(this.Value!);
                                }

                                public void Read(PacketBuffer buffer) {
                                    this.Value = buffer.Read<byte>();
                                }

                            }

                            public class MinSwitch : IPacketPayload {

                                public object? Value { get; set; }
                                public int SwitchState { get; set; }

                                public MinSwitch(object? value, int switchState) {
                                    this.Value = value;
                                    this.SwitchState = switchState;
                                }

                                public MinSwitch(int switchState) {
                                    this.SwitchState = switchState;
                                }

                                public void Read(PacketBuffer buffer) {

                                    this.Value = SwitchState switch {
                                        1 => buffer.ReadI32(),
                                        _ => buffer.ReadVoid()
                                    };

                                }

                                public void Write(PacketBuffer buffer) {
                                    switch (SwitchState) {
                                        case 1: buffer.WriteI32((int)this.Value!); break;

                                    }

                                }


                            }

                            public class MaxSwitch : IPacketPayload {

                                public object? Value { get; set; }
                                public int SwitchState { get; set; }

                                public MaxSwitch(object? value, int switchState) {
                                    this.Value = value;
                                    this.SwitchState = switchState;
                                }

                                public MaxSwitch(int switchState) {
                                    this.SwitchState = switchState;
                                }

                                public void Read(PacketBuffer buffer) {

                                    this.Value = SwitchState switch {
                                        1 => buffer.ReadI32(),
                                        _ => buffer.ReadVoid()
                                    };

                                }

                                public void Write(PacketBuffer buffer) {
                                    switch (SwitchState) {
                                        case 1: buffer.WriteI32((int)this.Value!); break;

                                    }

                                }


                            }


                        }

                        public class Container3 : IPacketPayload {
                            public FlagsBitfield? Flags { get; set; }
                            public MinSwitch? Min { get; set; }
                            public MaxSwitch? Max { get; set; }

                            public Container3() { }

                            public void Read(PacketBuffer buffer) {
                                this.Flags = buffer.Read<FlagsBitfield>(new FlagsBitfield());
                                this.Min = buffer.Read<MinSwitch>(new MinSwitch((int)Flags.MinPresent!));
                                this.Max = buffer.Read<MaxSwitch>(new MaxSwitch((int)Flags.MaxPresent!));
                            }

                            public void Write(PacketBuffer buffer) {
                                buffer.Write((FlagsBitfield)Flags!);
                                buffer.Write((MinSwitch)Min!);
                                buffer.Write((MaxSwitch)Max!);
                            }

                            public class FlagsBitfield : IPacketPayload {

                                public byte? Value { get; set; }

                                public byte Unused {
                                    get { return (byte)(((byte)Value! >> 2) & (63)); }
                                    set { var val = value << 2; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public byte MaxPresent {
                                    get { return (byte)(((byte)Value! >> 1) & (1)); }
                                    set { var val = value << 1; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public byte MinPresent {
                                    get { return (byte)(((byte)Value! >> 0) & (1)); }
                                    set { var val = value << 0; var inv = ~val; var x = (byte)this.Value! & (byte)inv; this.Value = (byte)(x | (byte)val); }
                                }
                                public void Write(PacketBuffer buffer) {
                                    buffer.Write(this.Value!);
                                }

                                public void Read(PacketBuffer buffer) {
                                    this.Value = buffer.Read<byte>();
                                }

                            }

                            public class MinSwitch : IPacketPayload {

                                public object? Value { get; set; }
                                public int SwitchState { get; set; }

                                public MinSwitch(object? value, int switchState) {
                                    this.Value = value;
                                    this.SwitchState = switchState;
                                }

                                public MinSwitch(int switchState) {
                                    this.SwitchState = switchState;
                                }

                                public void Read(PacketBuffer buffer) {

                                    this.Value = SwitchState switch {
                                        1 => buffer.ReadI64(),
                                        _ => buffer.ReadVoid()
                                    };

                                }

                                public void Write(PacketBuffer buffer) {
                                    switch (SwitchState) {
                                        case 1: buffer.WriteI64((long)this.Value!); break;

                                    }

                                }


                            }

                            public class MaxSwitch : IPacketPayload {

                                public object? Value { get; set; }
                                public int SwitchState { get; set; }

                                public MaxSwitch(object? value, int switchState) {
                                    this.Value = value;
                                    this.SwitchState = switchState;
                                }

                                public MaxSwitch(int switchState) {
                                    this.SwitchState = switchState;
                                }

                                public void Read(PacketBuffer buffer) {

                                    this.Value = SwitchState switch {
                                        1 => buffer.ReadI64(),
                                        _ => buffer.ReadVoid()
                                    };

                                }

                                public void Write(PacketBuffer buffer) {
                                    switch (SwitchState) {
                                        case 1: buffer.WriteI64((long)this.Value!); break;

                                    }

                                }


                            }


                        }


                    }

                    public class SuggestsSwitch : IPacketPayload {

                        public object? Value { get; set; }
                        public int SwitchState { get; set; }

                        public SuggestsSwitch(object? value, int switchState) {
                            this.Value = value;
                            this.SwitchState = switchState;
                        }

                        public SuggestsSwitch(int switchState) {
                            this.SwitchState = switchState;
                        }

                        public void Read(PacketBuffer buffer) {

                            this.Value = SwitchState switch {
                                1 => buffer.ReadString(),
                                _ => buffer.ReadVoid()
                            };

                        }

                        public void Write(PacketBuffer buffer) {
                            switch (SwitchState) {
                                case 1: buffer.WriteString((string)this.Value!); break;

                            }

                        }


                    }


                }


            }


        }

    }



    public class FacePlayerPacket : Packet {
        public const int ProtocolId = 0x37;

        public int? Feet_eyes { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public bool? IsEntity { get; set; }
        public EntityIdSwitch? EntityId { get; set; }
        public Entity_feet_eyesSwitch? Entity_feet_eyes { get; set; }

        public FacePlayerPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Feet_eyes = buffer.ReadVarint();
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.IsEntity = buffer.ReadBool();
            this.EntityId = buffer.Read<EntityIdSwitch>(new EntityIdSwitch((bool)IsEntity!));
            this.Entity_feet_eyes = buffer.Read<Entity_feet_eyesSwitch>(new Entity_feet_eyesSwitch((bool)IsEntity!));
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Feet_eyes!);
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteBool((bool)IsEntity!);
            buffer.Write((EntityIdSwitch)EntityId!);
            buffer.Write((Entity_feet_eyesSwitch)Entity_feet_eyes!);
        }

        public class EntityIdSwitch : IPacketPayload {

            public object? Value { get; set; }
            public bool SwitchState { get; set; }

            public EntityIdSwitch(object? value, bool switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public EntityIdSwitch(bool switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    true => buffer.ReadVarint(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case true: buffer.WriteVarint((int)this.Value!); break;

                }

            }


        }

        public class Entity_feet_eyesSwitch : IPacketPayload {

            public object? Value { get; set; }
            public bool SwitchState { get; set; }

            public Entity_feet_eyesSwitch(object? value, bool switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public Entity_feet_eyesSwitch(bool switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    true => buffer.ReadString(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case true: buffer.WriteString((string)this.Value!); break;

                }

            }


        }

    }



    public class NbtQueryResponsePacket : Packet {
        public const int ProtocolId = 0x60;

        public int? TransactionId { get; set; }
        public NbtCompound? Nbt { get; set; }

        public NbtQueryResponsePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.TransactionId = buffer.ReadVarint();
            this.Nbt = buffer.ReadOptionalnbt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)TransactionId!);
            buffer.WriteOptionalnbt((NbtCompound)Nbt!);
        }

    }



    public class ChatPacket : Packet {
        public const int ProtocolId = 0x0f;

        public string? Message { get; set; }
        public sbyte? Position { get; set; }
        public UUID? Sender { get; set; }

        public ChatPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Message = buffer.ReadString();
            this.Position = buffer.ReadI8();
            this.Sender = buffer.ReadUUID();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Message!);
            buffer.WriteI8((sbyte)Position!);
            buffer.WriteUUID((UUID)Sender!);
        }

    }



    public class MultiBlockChangePacket : Packet {
        public const int ProtocolId = 0x3f;

        public ChunkCoordinatesBitfield? ChunkCoordinates { get; set; }
        public bool? NotTrustEdges { get; set; }
        public int[]? Records { get; set; }

        public MultiBlockChangePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ChunkCoordinates = buffer.Read<ChunkCoordinatesBitfield>(new ChunkCoordinatesBitfield());
            this.NotTrustEdges = buffer.ReadBool();
            this.Records = buffer.ReadArray<int>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.Write((ChunkCoordinatesBitfield)ChunkCoordinates!);
            buffer.WriteBool((bool)NotTrustEdges!);
            buffer.WriteArray((int[])Records!);
        }

        public class ChunkCoordinatesBitfield : IPacketPayload {

            public ulong? Value { get; set; }

            public int X {
                get { return (int)(((ulong)Value! >> 42) & (4194303)); }
                set { var val = value << 42; var inv = ~val; var x = (ulong)this.Value! & (ulong)inv; this.Value = (ulong)(x | (uint)val); }
            }
            public int Z {
                get { return (int)(((ulong)Value! >> 20) & (4194303)); }
                set { var val = value << 20; var inv = ~val; var x = (ulong)this.Value! & (ulong)inv; this.Value = (ulong)(x | (uint)val); }
            }
            public int Y {
                get { return (int)(((ulong)Value! >> 0) & (1048575)); }
                set { var val = value << 0; var inv = ~val; var x = (ulong)this.Value! & (ulong)inv; this.Value = (ulong)(x | (uint)val); }
            }
            public void Write(PacketBuffer buffer) {
                buffer.Write(this.Value!);
            }

            public void Read(PacketBuffer buffer) {
                this.Value = buffer.Read<ulong>();
            }

        }

    }



    public class CloseWindowPacket : Packet {
        public const int ProtocolId = 0x13;

        public byte? WindowId { get; set; }

        public CloseWindowPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadU8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)WindowId!);
        }

    }



    public class OpenWindowPacket : Packet {
        public const int ProtocolId = 0x2e;

        public int? WindowId { get; set; }
        public int? InventoryType { get; set; }
        public string? WindowTitle { get; set; }

        public OpenWindowPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadVarint();
            this.InventoryType = buffer.ReadVarint();
            this.WindowTitle = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)WindowId!);
            buffer.WriteVarint((int)InventoryType!);
            buffer.WriteString((string)WindowTitle!);
        }

    }



    public class WindowItemsPacket : Packet {
        public const int ProtocolId = 0x14;

        public byte? WindowId { get; set; }
        public int? StateId { get; set; }
        public Slot[]? Items { get; set; }
        public Slot? CarriedItem { get; set; }

        public WindowItemsPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadU8();
            this.StateId = buffer.ReadVarint();
            this.Items = buffer.ReadArray<Slot>(buffer.ReadVarint());
            this.CarriedItem = buffer.ReadSlot();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)WindowId!);
            buffer.WriteVarint((int)StateId!);
            buffer.WriteArray((Slot[])Items!);
            buffer.WriteSlot((Slot)CarriedItem!);
        }

    }



    public class CraftProgressBarPacket : Packet {
        public const int ProtocolId = 0x15;

        public byte? WindowId { get; set; }
        public short? Property { get; set; }
        public short? Value { get; set; }

        public CraftProgressBarPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadU8();
            this.Property = buffer.ReadI16();
            this.Value = buffer.ReadI16();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)WindowId!);
            buffer.WriteI16((short)Property!);
            buffer.WriteI16((short)Value!);
        }

    }



    public class SetSlotPacket : Packet {
        public const int ProtocolId = 0x16;

        public sbyte? WindowId { get; set; }
        public int? StateId { get; set; }
        public short? Slot { get; set; }
        public Slot? Item { get; set; }

        public SetSlotPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadI8();
            this.StateId = buffer.ReadVarint();
            this.Slot = buffer.ReadI16();
            this.Item = buffer.ReadSlot();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)WindowId!);
            buffer.WriteVarint((int)StateId!);
            buffer.WriteI16((short)Slot!);
            buffer.WriteSlot((Slot)Item!);
        }

    }



    public class SetCooldownPacket : Packet {
        public const int ProtocolId = 0x17;

        public int? ItemID { get; set; }
        public int? CooldownTicks { get; set; }

        public SetCooldownPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ItemID = buffer.ReadVarint();
            this.CooldownTicks = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)ItemID!);
            buffer.WriteVarint((int)CooldownTicks!);
        }

    }



    public class CustomPayloadPacket : Packet {
        public const int ProtocolId = 0x18;

        public string? Channel { get; set; }
        public byte[]? Data { get; set; }

        public CustomPayloadPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Channel = buffer.ReadString();
            this.Data = buffer.ReadRestbuffer();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Channel!);
            buffer.WriteRestbuffer((byte[])Data!);
        }

    }



    public class NamedSoundEffectPacket : Packet {
        public const int ProtocolId = 0x19;

        public string? SoundName { get; set; }
        public int? SoundCategory { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Z { get; set; }
        public float? Volume { get; set; }
        public float? Pitch { get; set; }

        public NamedSoundEffectPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.SoundName = buffer.ReadString();
            this.SoundCategory = buffer.ReadVarint();
            this.X = buffer.ReadI32();
            this.Y = buffer.ReadI32();
            this.Z = buffer.ReadI32();
            this.Volume = buffer.ReadF32();
            this.Pitch = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)SoundName!);
            buffer.WriteVarint((int)SoundCategory!);
            buffer.WriteI32((int)X!);
            buffer.WriteI32((int)Y!);
            buffer.WriteI32((int)Z!);
            buffer.WriteF32((float)Volume!);
            buffer.WriteF32((float)Pitch!);
        }

    }



    public class KickDisconnectPacket : Packet {
        public const int ProtocolId = 0x1a;

        public string? Reason { get; set; }

        public KickDisconnectPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Reason = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Reason!);
        }

    }



    public class EntityStatusPacket : Packet {
        public const int ProtocolId = 0x1b;

        public int? EntityId { get; set; }
        public sbyte? EntityStatus { get; set; }

        public EntityStatusPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadI32();
            this.EntityStatus = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)EntityId!);
            buffer.WriteI8((sbyte)EntityStatus!);
        }

    }



    public class ExplosionPacket : Packet {
        public const int ProtocolId = 0x1c;

        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Z { get; set; }
        public float? Radius { get; set; }
        public AffectedBlockOffsetsContainer[]? AffectedBlockOffsets { get; set; }
        public float? PlayerMotionX { get; set; }
        public float? PlayerMotionY { get; set; }
        public float? PlayerMotionZ { get; set; }

        public ExplosionPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadF32();
            this.Y = buffer.ReadF32();
            this.Z = buffer.ReadF32();
            this.Radius = buffer.ReadF32();
            this.AffectedBlockOffsets = buffer.ReadArray<AffectedBlockOffsetsContainer>(buffer.ReadVarint());
            this.PlayerMotionX = buffer.ReadF32();
            this.PlayerMotionY = buffer.ReadF32();
            this.PlayerMotionZ = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF32((float)X!);
            buffer.WriteF32((float)Y!);
            buffer.WriteF32((float)Z!);
            buffer.WriteF32((float)Radius!);
            buffer.WriteArray((AffectedBlockOffsetsContainer[])AffectedBlockOffsets!);
            buffer.WriteF32((float)PlayerMotionX!);
            buffer.WriteF32((float)PlayerMotionY!);
            buffer.WriteF32((float)PlayerMotionZ!);
        }

        public class AffectedBlockOffsetsContainer : IPacketPayload {
            public sbyte? X { get; set; }
            public sbyte? Y { get; set; }
            public sbyte? Z { get; set; }

            public AffectedBlockOffsetsContainer() { }

            public void Read(PacketBuffer buffer) {
                this.X = buffer.ReadI8();
                this.Y = buffer.ReadI8();
                this.Z = buffer.ReadI8();
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteI8((sbyte)X!);
                buffer.WriteI8((sbyte)Y!);
                buffer.WriteI8((sbyte)Z!);
            }


        }

    }



    public class UnloadChunkPacket : Packet {
        public const int ProtocolId = 0x1d;

        public int? ChunkX { get; set; }
        public int? ChunkZ { get; set; }

        public UnloadChunkPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ChunkX = buffer.ReadI32();
            this.ChunkZ = buffer.ReadI32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)ChunkX!);
            buffer.WriteI32((int)ChunkZ!);
        }

    }



    public class GameStateChangePacket : Packet {
        public const int ProtocolId = 0x1e;

        public byte? Reason { get; set; }
        public float? GameMode { get; set; }

        public GameStateChangePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Reason = buffer.ReadU8();
            this.GameMode = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)Reason!);
            buffer.WriteF32((float)GameMode!);
        }

    }



    public class OpenHorseWindowPacket : Packet {
        public const int ProtocolId = 0x1f;

        public byte? WindowId { get; set; }
        public int? NbSlots { get; set; }
        public int? EntityId { get; set; }

        public OpenHorseWindowPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadU8();
            this.NbSlots = buffer.ReadVarint();
            this.EntityId = buffer.ReadI32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)WindowId!);
            buffer.WriteVarint((int)NbSlots!);
            buffer.WriteI32((int)EntityId!);
        }

    }



    public class KeepAlivePacket : Packet {
        public const int ProtocolId = 0x21;

        public long? KeepAliveId { get; set; }

        public KeepAlivePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.KeepAliveId = buffer.ReadI64();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI64((long)KeepAliveId!);
        }

    }



    public class MapChunkPacket : Packet {
        public const int ProtocolId = 0x22;

        public int? X { get; set; }
        public int? Z { get; set; }
        public NbtCompound? Heightmaps { get; set; }
        public byte[]? ChunkData { get; set; }
        public Chunkblockentity[]? BlockEntities { get; set; }
        public bool? TrustEdges { get; set; }
        public long[]? SkyLightMask { get; set; }
        public long[]? BlockLightMask { get; set; }
        public long[]? EmptySkyLightMask { get; set; }
        public long[]? EmptyBlockLightMask { get; set; }
        public byte[][]? SkyLight { get; set; }
        public byte[][]? BlockLight { get; set; }

        public MapChunkPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadI32();
            this.Z = buffer.ReadI32();
            this.Heightmaps = buffer.ReadNbt();
            this.ChunkData = buffer.ReadBuffer();
            this.BlockEntities = buffer.ReadArray<Chunkblockentity>(buffer.ReadVarint());
            this.TrustEdges = buffer.ReadBool();
            this.SkyLightMask = buffer.ReadArray<long>(buffer.ReadVarint());
            this.BlockLightMask = buffer.ReadArray<long>(buffer.ReadVarint());
            this.EmptySkyLightMask = buffer.ReadArray<long>(buffer.ReadVarint());
            this.EmptyBlockLightMask = buffer.ReadArray<long>(buffer.ReadVarint());
            this.SkyLight = buffer.ReadArray<byte[]>(buffer.ReadVarint());
            this.BlockLight = buffer.ReadArray<byte[]>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)X!);
            buffer.WriteI32((int)Z!);
            buffer.WriteNbt((NbtCompound)Heightmaps!);
            buffer.WriteBuffer((byte[])ChunkData!);
            buffer.WriteArray((Chunkblockentity[])BlockEntities!);
            buffer.WriteBool((bool)TrustEdges!);
            buffer.WriteArray((long[])SkyLightMask!);
            buffer.WriteArray((long[])BlockLightMask!);
            buffer.WriteArray((long[])EmptySkyLightMask!);
            buffer.WriteArray((long[])EmptyBlockLightMask!);
            buffer.WriteArray((byte[][])SkyLight!);
            buffer.WriteArray((byte[][])BlockLight!);
        }

    }



    public class WorldEventPacket : Packet {
        public const int ProtocolId = 0x23;

        public int? EffectId { get; set; }
        public Position? Location { get; set; }
        public int? Data { get; set; }
        public bool? Global { get; set; }

        public WorldEventPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EffectId = buffer.ReadI32();
            this.Location = buffer.ReadPosition();
            this.Data = buffer.ReadI32();
            this.Global = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)EffectId!);
            buffer.WritePosition((Position)Location!);
            buffer.WriteI32((int)Data!);
            buffer.WriteBool((bool)Global!);
        }

    }



    public class WorldParticlesPacket : Packet {
        public const int ProtocolId = 0x24;

        public int? ParticleId { get; set; }
        public bool? LongDistance { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public float? OffsetX { get; set; }
        public float? OffsetY { get; set; }
        public float? OffsetZ { get; set; }
        public float? ParticleData { get; set; }
        public int? Particles { get; set; }
        public ParticleData? Data { get; set; }

        public WorldParticlesPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ParticleId = buffer.ReadI32();
            this.LongDistance = buffer.ReadBool();
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.OffsetX = buffer.ReadF32();
            this.OffsetY = buffer.ReadF32();
            this.OffsetZ = buffer.ReadF32();
            this.ParticleData = buffer.ReadF32();
            this.Particles = buffer.ReadI32();
            this.Data = buffer.ReadParticleData();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)ParticleId!);
            buffer.WriteBool((bool)LongDistance!);
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteF32((float)OffsetX!);
            buffer.WriteF32((float)OffsetY!);
            buffer.WriteF32((float)OffsetZ!);
            buffer.WriteF32((float)ParticleData!);
            buffer.WriteI32((int)Particles!);
            buffer.WriteParticleData((ParticleData)Data!);
        }

    }



    public class UpdateLightPacket : Packet {
        public const int ProtocolId = 0x25;

        public int? ChunkX { get; set; }
        public int? ChunkZ { get; set; }
        public bool? TrustEdges { get; set; }
        public long[]? SkyLightMask { get; set; }
        public long[]? BlockLightMask { get; set; }
        public long[]? EmptySkyLightMask { get; set; }
        public long[]? EmptyBlockLightMask { get; set; }
        public byte[][]? SkyLight { get; set; }
        public byte[][]? BlockLight { get; set; }

        public UpdateLightPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ChunkX = buffer.ReadVarint();
            this.ChunkZ = buffer.ReadVarint();
            this.TrustEdges = buffer.ReadBool();
            this.SkyLightMask = buffer.ReadArray<long>(buffer.ReadVarint());
            this.BlockLightMask = buffer.ReadArray<long>(buffer.ReadVarint());
            this.EmptySkyLightMask = buffer.ReadArray<long>(buffer.ReadVarint());
            this.EmptyBlockLightMask = buffer.ReadArray<long>(buffer.ReadVarint());
            this.SkyLight = buffer.ReadArray<byte[]>(buffer.ReadVarint());
            this.BlockLight = buffer.ReadArray<byte[]>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)ChunkX!);
            buffer.WriteVarint((int)ChunkZ!);
            buffer.WriteBool((bool)TrustEdges!);
            buffer.WriteArray((long[])SkyLightMask!);
            buffer.WriteArray((long[])BlockLightMask!);
            buffer.WriteArray((long[])EmptySkyLightMask!);
            buffer.WriteArray((long[])EmptyBlockLightMask!);
            buffer.WriteArray((byte[][])SkyLight!);
            buffer.WriteArray((byte[][])BlockLight!);
        }

    }



    public class LoginPacket : Packet {
        public const int ProtocolId = 0x26;

        public int? EntityId { get; set; }
        public bool? IsHardcore { get; set; }
        public byte? GameMode { get; set; }
        public sbyte? PreviousGameMode { get; set; }
        public string[]? WorldNames { get; set; }
        public NbtCompound? DimensionCodec { get; set; }
        public NbtCompound? Dimension { get; set; }
        public string? WorldName { get; set; }
        public long? HashedSeed { get; set; }
        public int? MaxPlayers { get; set; }
        public int? ViewDistance { get; set; }
        public int? SimulationDistance { get; set; }
        public bool? ReducedDebugInfo { get; set; }
        public bool? EnableRespawnScreen { get; set; }
        public bool? IsDebug { get; set; }
        public bool? IsFlat { get; set; }

        public LoginPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadI32();
            this.IsHardcore = buffer.ReadBool();
            this.GameMode = buffer.ReadU8();
            this.PreviousGameMode = buffer.ReadI8();
            this.WorldNames = buffer.ReadArray<string>(buffer.ReadVarint());
            this.DimensionCodec = buffer.ReadNbt();
            this.Dimension = buffer.ReadNbt();
            this.WorldName = buffer.ReadString();
            this.HashedSeed = buffer.ReadI64();
            this.MaxPlayers = buffer.ReadVarint();
            this.ViewDistance = buffer.ReadVarint();
            this.SimulationDistance = buffer.ReadVarint();
            this.ReducedDebugInfo = buffer.ReadBool();
            this.EnableRespawnScreen = buffer.ReadBool();
            this.IsDebug = buffer.ReadBool();
            this.IsFlat = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)EntityId!);
            buffer.WriteBool((bool)IsHardcore!);
            buffer.WriteU8((byte)GameMode!);
            buffer.WriteI8((sbyte)PreviousGameMode!);
            buffer.WriteArray((string[])WorldNames!);
            buffer.WriteNbt((NbtCompound)DimensionCodec!);
            buffer.WriteNbt((NbtCompound)Dimension!);
            buffer.WriteString((string)WorldName!);
            buffer.WriteI64((long)HashedSeed!);
            buffer.WriteVarint((int)MaxPlayers!);
            buffer.WriteVarint((int)ViewDistance!);
            buffer.WriteVarint((int)SimulationDistance!);
            buffer.WriteBool((bool)ReducedDebugInfo!);
            buffer.WriteBool((bool)EnableRespawnScreen!);
            buffer.WriteBool((bool)IsDebug!);
            buffer.WriteBool((bool)IsFlat!);
        }

    }



    public class MapPacket : Packet {
        public const int ProtocolId = 0x27;

        public int? ItemDamage { get; set; }
        public sbyte? Scale { get; set; }
        public bool? Locked { get; set; }
        public Container[]? Icons { get; set; }
        public byte? Columns { get; set; }
        public RowsSwitch? Rows { get; set; }
        public XSwitch? X { get; set; }
        public YSwitch? Y { get; set; }
        public DataSwitch? Data { get; set; }

        public MapPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ItemDamage = buffer.ReadVarint();
            this.Scale = buffer.ReadI8();
            this.Locked = buffer.ReadBool();
            this.Icons = buffer.ReadOption<Container[]>();
            this.Columns = buffer.ReadU8();
            this.Rows = buffer.Read<RowsSwitch>(new RowsSwitch((byte)Columns!));
            this.X = buffer.Read<XSwitch>(new XSwitch((byte)Columns!));
            this.Y = buffer.Read<YSwitch>(new YSwitch((byte)Columns!));
            this.Data = buffer.Read<DataSwitch>(new DataSwitch((byte)Columns!));
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)ItemDamage!);
            buffer.WriteI8((sbyte)Scale!);
            buffer.WriteBool((bool)Locked!);
            buffer.WriteOption((Container[])Icons!);
            buffer.WriteU8((byte)Columns!);
            buffer.Write((RowsSwitch)Rows!);
            buffer.Write((XSwitch)X!);
            buffer.Write((YSwitch)Y!);
            buffer.Write((DataSwitch)Data!);
        }

        public class Container : IPacketPayload {
            public int? Type { get; set; }
            public sbyte? X { get; set; }
            public sbyte? Z { get; set; }
            public byte? Direction { get; set; }
            public string? DisplayName { get; set; }

            public Container() { }

            public void Read(PacketBuffer buffer) {
                this.Type = buffer.ReadVarint();
                this.X = buffer.ReadI8();
                this.Z = buffer.ReadI8();
                this.Direction = buffer.ReadU8();
                this.DisplayName = buffer.ReadOption<string>();
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteVarint((int)Type!);
                buffer.WriteI8((sbyte)X!);
                buffer.WriteI8((sbyte)Z!);
                buffer.WriteU8((byte)Direction!);
                buffer.WriteOption((string)DisplayName!);
            }


        }

        public class RowsSwitch : IPacketPayload {

            public object? Value { get; set; }
            public byte SwitchState { get; set; }

            public RowsSwitch(object? value, byte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public RowsSwitch(byte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadVoid(),
                    _ => buffer.ReadU8()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteVoid((object)this.Value!); break;

                }

            }


        }

        public class XSwitch : IPacketPayload {

            public object? Value { get; set; }
            public byte SwitchState { get; set; }

            public XSwitch(object? value, byte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public XSwitch(byte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadVoid(),
                    _ => buffer.ReadU8()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteVoid((object)this.Value!); break;

                }

            }


        }

        public class YSwitch : IPacketPayload {

            public object? Value { get; set; }
            public byte SwitchState { get; set; }

            public YSwitch(object? value, byte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public YSwitch(byte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadVoid(),
                    _ => buffer.ReadU8()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteVoid((object)this.Value!); break;

                }

            }


        }

        public class DataSwitch : IPacketPayload {

            public object? Value { get; set; }
            public byte SwitchState { get; set; }

            public DataSwitch(object? value, byte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public DataSwitch(byte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadVoid(),
                    _ => buffer.ReadBuffer()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteVoid((object)this.Value!); break;

                }

            }


        }

    }



    public class TradeListPacket : Packet {
        public const int ProtocolId = 0x28;

        public int? WindowId { get; set; }
        public TradesContainer[]? Trades { get; set; }
        public int? VillagerLevel { get; set; }
        public int? Experience { get; set; }
        public bool? IsRegularVillager { get; set; }
        public bool? CanRestock { get; set; }

        public TradeListPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadVarint();
            this.Trades = buffer.ReadArray<TradesContainer>(buffer.ReadU8());
            this.VillagerLevel = buffer.ReadVarint();
            this.Experience = buffer.ReadVarint();
            this.IsRegularVillager = buffer.ReadBool();
            this.CanRestock = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)WindowId!);
            buffer.WriteArray((TradesContainer[])Trades!);
            buffer.WriteVarint((int)VillagerLevel!);
            buffer.WriteVarint((int)Experience!);
            buffer.WriteBool((bool)IsRegularVillager!);
            buffer.WriteBool((bool)CanRestock!);
        }

        public class TradesContainer : IPacketPayload {
            public Slot? InputItem1 { get; set; }
            public Slot? OutputItem { get; set; }
            public Slot? InputItem2 { get; set; }
            public bool? TradeDisabled { get; set; }
            public int? NbTradeUses { get; set; }
            public int? MaximumNbTradeUses { get; set; }
            public int? Xp { get; set; }
            public int? SpecialPrice { get; set; }
            public float? PriceMultiplier { get; set; }
            public int? Demand { get; set; }

            public TradesContainer() { }

            public void Read(PacketBuffer buffer) {
                this.InputItem1 = buffer.ReadSlot();
                this.OutputItem = buffer.ReadSlot();
                this.InputItem2 = buffer.ReadOption<Slot>();
                this.TradeDisabled = buffer.ReadBool();
                this.NbTradeUses = buffer.ReadI32();
                this.MaximumNbTradeUses = buffer.ReadI32();
                this.Xp = buffer.ReadI32();
                this.SpecialPrice = buffer.ReadI32();
                this.PriceMultiplier = buffer.ReadF32();
                this.Demand = buffer.ReadI32();
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteSlot((Slot)InputItem1!);
                buffer.WriteSlot((Slot)OutputItem!);
                buffer.WriteOption((Slot)InputItem2!);
                buffer.WriteBool((bool)TradeDisabled!);
                buffer.WriteI32((int)NbTradeUses!);
                buffer.WriteI32((int)MaximumNbTradeUses!);
                buffer.WriteI32((int)Xp!);
                buffer.WriteI32((int)SpecialPrice!);
                buffer.WriteF32((float)PriceMultiplier!);
                buffer.WriteI32((int)Demand!);
            }


        }

    }



    public class RelEntityMovePacket : Packet {
        public const int ProtocolId = 0x29;

        public int? EntityId { get; set; }
        public short? DX { get; set; }
        public short? DY { get; set; }
        public short? DZ { get; set; }
        public bool? OnGround { get; set; }

        public RelEntityMovePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.DX = buffer.ReadI16();
            this.DY = buffer.ReadI16();
            this.DZ = buffer.ReadI16();
            this.OnGround = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteI16((short)DX!);
            buffer.WriteI16((short)DY!);
            buffer.WriteI16((short)DZ!);
            buffer.WriteBool((bool)OnGround!);
        }

    }



    public class EntityMoveLookPacket : Packet {
        public const int ProtocolId = 0x2a;

        public int? EntityId { get; set; }
        public short? DX { get; set; }
        public short? DY { get; set; }
        public short? DZ { get; set; }
        public sbyte? Yaw { get; set; }
        public sbyte? Pitch { get; set; }
        public bool? OnGround { get; set; }

        public EntityMoveLookPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.DX = buffer.ReadI16();
            this.DY = buffer.ReadI16();
            this.DZ = buffer.ReadI16();
            this.Yaw = buffer.ReadI8();
            this.Pitch = buffer.ReadI8();
            this.OnGround = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteI16((short)DX!);
            buffer.WriteI16((short)DY!);
            buffer.WriteI16((short)DZ!);
            buffer.WriteI8((sbyte)Yaw!);
            buffer.WriteI8((sbyte)Pitch!);
            buffer.WriteBool((bool)OnGround!);
        }

    }



    public class EntityLookPacket : Packet {
        public const int ProtocolId = 0x2b;

        public int? EntityId { get; set; }
        public sbyte? Yaw { get; set; }
        public sbyte? Pitch { get; set; }
        public bool? OnGround { get; set; }

        public EntityLookPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.Yaw = buffer.ReadI8();
            this.Pitch = buffer.ReadI8();
            this.OnGround = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteI8((sbyte)Yaw!);
            buffer.WriteI8((sbyte)Pitch!);
            buffer.WriteBool((bool)OnGround!);
        }

    }



    public class VehicleMovePacket : Packet {
        public const int ProtocolId = 0x2c;

        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public float? Yaw { get; set; }
        public float? Pitch { get; set; }

        public VehicleMovePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.Yaw = buffer.ReadF32();
            this.Pitch = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteF32((float)Yaw!);
            buffer.WriteF32((float)Pitch!);
        }

    }



    public class OpenBookPacket : Packet {
        public const int ProtocolId = 0x2d;

        public int? Hand { get; set; }

        public OpenBookPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Hand = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Hand!);
        }

    }



    public class OpenSignEntityPacket : Packet {
        public const int ProtocolId = 0x2f;

        public Position? Location { get; set; }

        public OpenSignEntityPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
        }

    }



    public class CraftRecipeResponsePacket : Packet {
        public const int ProtocolId = 0x31;

        public sbyte? WindowId { get; set; }
        public string? Recipe { get; set; }

        public CraftRecipeResponsePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadI8();
            this.Recipe = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)WindowId!);
            buffer.WriteString((string)Recipe!);
        }

    }



    public class AbilitiesPacket : Packet {
        public const int ProtocolId = 0x32;

        public sbyte? Flags { get; set; }
        public float? FlyingSpeed { get; set; }
        public float? WalkingSpeed { get; set; }

        public AbilitiesPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Flags = buffer.ReadI8();
            this.FlyingSpeed = buffer.ReadF32();
            this.WalkingSpeed = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)Flags!);
            buffer.WriteF32((float)FlyingSpeed!);
            buffer.WriteF32((float)WalkingSpeed!);
        }

    }



    public class EndCombatEventPacket : Packet {
        public const int ProtocolId = 0x33;

        public int? Duration { get; set; }
        public int? EntityId { get; set; }

        public EndCombatEventPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Duration = buffer.ReadVarint();
            this.EntityId = buffer.ReadI32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Duration!);
            buffer.WriteI32((int)EntityId!);
        }

    }



    public class EnterCombatEventPacket : Packet {
        public const int ProtocolId = 0x34;


        public EnterCombatEventPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
        }

        public override void Write(PacketBuffer buffer) {
        }

    }



    public class DeathCombatEventPacket : Packet {
        public const int ProtocolId = 0x35;

        public int? PlayerId { get; set; }
        public int? EntityId { get; set; }
        public string? Message { get; set; }

        public DeathCombatEventPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.PlayerId = buffer.ReadVarint();
            this.EntityId = buffer.ReadI32();
            this.Message = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)PlayerId!);
            buffer.WriteI32((int)EntityId!);
            buffer.WriteString((string)Message!);
        }

    }



    public class PlayerInfoPacket : Packet {
        public const int ProtocolId = 0x36;

        public int? Action { get; set; }
        public DataContainer[]? Data { get; set; }

        public PlayerInfoPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Action = buffer.ReadVarint();
            this.Data = buffer.ReadArray<DataContainer>(buffer.ReadVarint(), Action);
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Action!);
            buffer.WriteArray((DataContainer[])Data!);
        }

        public class DataContainer : IPacketPayload {
            public UUID? UUID { get; set; }
            public NameSwitch? Name { get; set; }
            public PropertiesSwitch? Properties { get; set; }
            public GamemodeSwitch? Gamemode { get; set; }
            public PingSwitch? Ping { get; set; }
            public DisplayNameSwitch? DisplayName { get; set; }

            public int Action { get; set; }
            public DataContainer(int action) {
                this.Action = action;
            }

            public void Read(PacketBuffer buffer) {
                this.UUID = buffer.ReadUUID();
                this.Name = buffer.Read<NameSwitch>(new NameSwitch((int)Action!));
                this.Properties = buffer.Read<PropertiesSwitch>(new PropertiesSwitch((int)Action!));
                this.Gamemode = buffer.Read<GamemodeSwitch>(new GamemodeSwitch((int)Action!));
                this.Ping = buffer.Read<PingSwitch>(new PingSwitch((int)Action!));
                this.DisplayName = buffer.Read<DisplayNameSwitch>(new DisplayNameSwitch((int)Action!));
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteUUID((UUID)UUID!);
                buffer.Write((NameSwitch)Name!);
                buffer.Write((PropertiesSwitch)Properties!);
                buffer.Write((GamemodeSwitch)Gamemode!);
                buffer.Write((PingSwitch)Ping!);
                buffer.Write((DisplayNameSwitch)DisplayName!);
            }

            public class NameSwitch : IPacketPayload {

                public object? Value { get; set; }
                public int SwitchState { get; set; }

                public NameSwitch(object? value, int switchState) {
                    this.Value = value;
                    this.SwitchState = switchState;
                }

                public NameSwitch(int switchState) {
                    this.SwitchState = switchState;
                }

                public void Read(PacketBuffer buffer) {

                    this.Value = SwitchState switch {
                        0 => buffer.ReadString(),
                        _ => buffer.ReadVoid()
                    };

                }

                public void Write(PacketBuffer buffer) {
                    switch (SwitchState) {
                        case 0: buffer.WriteString((string)this.Value!); break;

                    }

                }


            }

            public class PropertiesSwitch : IPacketPayload {

                public object? Value { get; set; }
                public int SwitchState { get; set; }

                public PropertiesSwitch(object? value, int switchState) {
                    this.Value = value;
                    this.SwitchState = switchState;
                }

                public PropertiesSwitch(int switchState) {
                    this.SwitchState = switchState;
                }

                public void Read(PacketBuffer buffer) {

                    this.Value = SwitchState switch {
                        0 => buffer.ReadArray<Container>(buffer.ReadVarint()),
                        _ => buffer.ReadVoid()
                    };

                }

                public void Write(PacketBuffer buffer) {
                    switch (SwitchState) {
                        case 0: buffer.WriteArray((Container[])this.Value!); break;

                    }

                }

                public class Container : IPacketPayload {
                    public string? Name { get; set; }
                    public string? Value { get; set; }
                    public string? Signature { get; set; }

                    public Container() { }

                    public void Read(PacketBuffer buffer) {
                        this.Name = buffer.ReadString();
                        this.Value = buffer.ReadString();
                        this.Signature = buffer.ReadOption<string>();
                    }

                    public void Write(PacketBuffer buffer) {
                        buffer.WriteString((string)Name!);
                        buffer.WriteString((string)Value!);
                        buffer.WriteOption((string)Signature!);
                    }


                }


            }

            public class GamemodeSwitch : IPacketPayload {

                public object? Value { get; set; }
                public int SwitchState { get; set; }

                public GamemodeSwitch(object? value, int switchState) {
                    this.Value = value;
                    this.SwitchState = switchState;
                }

                public GamemodeSwitch(int switchState) {
                    this.SwitchState = switchState;
                }

                public void Read(PacketBuffer buffer) {

                    this.Value = SwitchState switch {
                        0 => buffer.ReadVarint(),
                        1 => buffer.ReadVarint(),
                        _ => buffer.ReadVoid()
                    };

                }

                public void Write(PacketBuffer buffer) {
                    switch (SwitchState) {
                        case 0: buffer.WriteVarint((int)this.Value!); break;
                        case 1: buffer.WriteVarint((int)this.Value!); break;

                    }

                }


            }

            public class PingSwitch : IPacketPayload {

                public object? Value { get; set; }
                public int SwitchState { get; set; }

                public PingSwitch(object? value, int switchState) {
                    this.Value = value;
                    this.SwitchState = switchState;
                }

                public PingSwitch(int switchState) {
                    this.SwitchState = switchState;
                }

                public void Read(PacketBuffer buffer) {

                    this.Value = SwitchState switch {
                        0 => buffer.ReadVarint(),
                        2 => buffer.ReadVarint(),
                        _ => buffer.ReadVoid()
                    };

                }

                public void Write(PacketBuffer buffer) {
                    switch (SwitchState) {
                        case 0: buffer.WriteVarint((int)this.Value!); break;
                        case 2: buffer.WriteVarint((int)this.Value!); break;

                    }

                }


            }

            public class DisplayNameSwitch : IPacketPayload {

                public object? Value { get; set; }
                public int SwitchState { get; set; }

                public DisplayNameSwitch(object? value, int switchState) {
                    this.Value = value;
                    this.SwitchState = switchState;
                }

                public DisplayNameSwitch(int switchState) {
                    this.SwitchState = switchState;
                }

                public void Read(PacketBuffer buffer) {

                    this.Value = SwitchState switch {
                        0 => buffer.ReadOption<string>(),
                        3 => buffer.ReadOption<string>(),
                        _ => buffer.ReadVoid()
                    };

                }

                public void Write(PacketBuffer buffer) {
                    switch (SwitchState) {
                        case 0: buffer.WriteOption((string)this.Value!); break;
                        case 3: buffer.WriteOption((string)this.Value!); break;

                    }

                }


            }


        }

    }



    public class PositionPacket : Packet {
        public const int ProtocolId = 0x38;

        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public float? Yaw { get; set; }
        public float? Pitch { get; set; }
        public sbyte? Flags { get; set; }
        public int? TeleportId { get; set; }
        public bool? DismountVehicle { get; set; }

        public PositionPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.Yaw = buffer.ReadF32();
            this.Pitch = buffer.ReadF32();
            this.Flags = buffer.ReadI8();
            this.TeleportId = buffer.ReadVarint();
            this.DismountVehicle = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteF32((float)Yaw!);
            buffer.WriteF32((float)Pitch!);
            buffer.WriteI8((sbyte)Flags!);
            buffer.WriteVarint((int)TeleportId!);
            buffer.WriteBool((bool)DismountVehicle!);
        }

    }



    public class UnlockRecipesPacket : Packet {
        public const int ProtocolId = 0x39;

        public int? Action { get; set; }
        public bool? CraftingBookOpen { get; set; }
        public bool? FilteringCraftable { get; set; }
        public bool? SmeltingBookOpen { get; set; }
        public bool? FilteringSmeltable { get; set; }
        public bool? BlastFurnaceOpen { get; set; }
        public bool? FilteringBlastFurnace { get; set; }
        public bool? SmokerBookOpen { get; set; }
        public bool? FilteringSmoker { get; set; }
        public string[]? Recipes1 { get; set; }
        public Recipes2Switch? Recipes2 { get; set; }

        public UnlockRecipesPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Action = buffer.ReadVarint();
            this.CraftingBookOpen = buffer.ReadBool();
            this.FilteringCraftable = buffer.ReadBool();
            this.SmeltingBookOpen = buffer.ReadBool();
            this.FilteringSmeltable = buffer.ReadBool();
            this.BlastFurnaceOpen = buffer.ReadBool();
            this.FilteringBlastFurnace = buffer.ReadBool();
            this.SmokerBookOpen = buffer.ReadBool();
            this.FilteringSmoker = buffer.ReadBool();
            this.Recipes1 = buffer.ReadArray<string>(buffer.ReadVarint());
            this.Recipes2 = buffer.Read<Recipes2Switch>(new Recipes2Switch((int)Action!));
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Action!);
            buffer.WriteBool((bool)CraftingBookOpen!);
            buffer.WriteBool((bool)FilteringCraftable!);
            buffer.WriteBool((bool)SmeltingBookOpen!);
            buffer.WriteBool((bool)FilteringSmeltable!);
            buffer.WriteBool((bool)BlastFurnaceOpen!);
            buffer.WriteBool((bool)FilteringBlastFurnace!);
            buffer.WriteBool((bool)SmokerBookOpen!);
            buffer.WriteBool((bool)FilteringSmoker!);
            buffer.WriteArray((string[])Recipes1!);
            buffer.Write((Recipes2Switch)Recipes2!);
        }

        public class Recipes2Switch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public Recipes2Switch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public Recipes2Switch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadArray<string>(buffer.ReadVarint()),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteArray((string[])this.Value!); break;

                }

            }


        }

    }



    public class EntityDestroyPacket : Packet {
        public const int ProtocolId = 0x3a;

        public int[]? EntityIds { get; set; }

        public EntityDestroyPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityIds = buffer.ReadArray<int>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteArray((int[])EntityIds!);
        }

    }



    public class RemoveEntityEffectPacket : Packet {
        public const int ProtocolId = 0x3b;

        public int? EntityId { get; set; }
        public sbyte? EffectId { get; set; }

        public RemoveEntityEffectPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.EffectId = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteI8((sbyte)EffectId!);
        }

    }



    public class ResourcePackSendPacket : Packet {
        public const int ProtocolId = 0x3c;

        public string? Url { get; set; }
        public string? Hash { get; set; }
        public bool? Forced { get; set; }
        public string? PromptMessage { get; set; }

        public ResourcePackSendPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Url = buffer.ReadString();
            this.Hash = buffer.ReadString();
            this.Forced = buffer.ReadBool();
            this.PromptMessage = buffer.ReadOption<string>();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Url!);
            buffer.WriteString((string)Hash!);
            buffer.WriteBool((bool)Forced!);
            buffer.WriteOption((string)PromptMessage!);
        }

    }



    public class RespawnPacket : Packet {
        public const int ProtocolId = 0x3d;

        public NbtCompound? Dimension { get; set; }
        public string? WorldName { get; set; }
        public long? HashedSeed { get; set; }
        public byte? Gamemode { get; set; }
        public byte? PreviousGamemode { get; set; }
        public bool? IsDebug { get; set; }
        public bool? IsFlat { get; set; }
        public bool? CopyMetadata { get; set; }

        public RespawnPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Dimension = buffer.ReadNbt();
            this.WorldName = buffer.ReadString();
            this.HashedSeed = buffer.ReadI64();
            this.Gamemode = buffer.ReadU8();
            this.PreviousGamemode = buffer.ReadU8();
            this.IsDebug = buffer.ReadBool();
            this.IsFlat = buffer.ReadBool();
            this.CopyMetadata = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteNbt((NbtCompound)Dimension!);
            buffer.WriteString((string)WorldName!);
            buffer.WriteI64((long)HashedSeed!);
            buffer.WriteU8((byte)Gamemode!);
            buffer.WriteU8((byte)PreviousGamemode!);
            buffer.WriteBool((bool)IsDebug!);
            buffer.WriteBool((bool)IsFlat!);
            buffer.WriteBool((bool)CopyMetadata!);
        }

    }



    public class EntityHeadRotationPacket : Packet {
        public const int ProtocolId = 0x3e;

        public int? EntityId { get; set; }
        public sbyte? HeadYaw { get; set; }

        public EntityHeadRotationPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.HeadYaw = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteI8((sbyte)HeadYaw!);
        }

    }



    public class CameraPacket : Packet {
        public const int ProtocolId = 0x47;

        public int? CameraId { get; set; }

        public CameraPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.CameraId = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)CameraId!);
        }

    }



    public class HeldItemSlotPacket : Packet {
        public const int ProtocolId = 0x48;

        public sbyte? Slot { get; set; }

        public HeldItemSlotPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Slot = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)Slot!);
        }

    }



    public class UpdateViewPositionPacket : Packet {
        public const int ProtocolId = 0x49;

        public int? ChunkX { get; set; }
        public int? ChunkZ { get; set; }

        public UpdateViewPositionPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ChunkX = buffer.ReadVarint();
            this.ChunkZ = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)ChunkX!);
            buffer.WriteVarint((int)ChunkZ!);
        }

    }



    public class UpdateViewDistancePacket : Packet {
        public const int ProtocolId = 0x4a;

        public int? ViewDistance { get; set; }

        public UpdateViewDistancePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ViewDistance = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)ViewDistance!);
        }

    }



    public class ScoreboardDisplayObjectivePacket : Packet {
        public const int ProtocolId = 0x4c;

        public sbyte? Position { get; set; }
        public string? Name { get; set; }

        public ScoreboardDisplayObjectivePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Position = buffer.ReadI8();
            this.Name = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)Position!);
            buffer.WriteString((string)Name!);
        }

    }



    public class EntityMetadataPacket : Packet {
        public const int ProtocolId = 0x4d;

        public int? EntityId { get; set; }
        public Entitymetadata? Metadata { get; set; }

        public EntityMetadataPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.Metadata = buffer.ReadEntitymetadata();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteEntitymetadata((Entitymetadata)Metadata!);
        }

    }



    public class AttachEntityPacket : Packet {
        public const int ProtocolId = 0x4e;

        public int? EntityId { get; set; }
        public int? VehicleId { get; set; }

        public AttachEntityPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadI32();
            this.VehicleId = buffer.ReadI32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)EntityId!);
            buffer.WriteI32((int)VehicleId!);
        }

    }



    public class EntityVelocityPacket : Packet {
        public const int ProtocolId = 0x4f;

        public int? EntityId { get; set; }
        public short? VelocityX { get; set; }
        public short? VelocityY { get; set; }
        public short? VelocityZ { get; set; }

        public EntityVelocityPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.VelocityX = buffer.ReadI16();
            this.VelocityY = buffer.ReadI16();
            this.VelocityZ = buffer.ReadI16();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteI16((short)VelocityX!);
            buffer.WriteI16((short)VelocityY!);
            buffer.WriteI16((short)VelocityZ!);
        }

    }



    public class EntityEquipmentPacket : Packet {
        public const int ProtocolId = 0x50;

        public int? EntityId { get; set; }
        public TopBitSetTerminatedArray<EquipmentsContainer>? Equipments { get; set; }

        public EntityEquipmentPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.Equipments = buffer.ReadTopBitSetTerminatedArray<EquipmentsContainer>();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteTopBitSetTerminatedArray((TopBitSetTerminatedArray<EquipmentsContainer>)Equipments!);
        }

        public class EquipmentsContainer : IPacketPayload {
            public sbyte? Slot { get; set; }
            public Slot? Item { get; set; }

            public EquipmentsContainer() { }

            public void Read(PacketBuffer buffer) {
                this.Slot = buffer.ReadI8();
                this.Item = buffer.ReadSlot();
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteI8((sbyte)Slot!);
                buffer.WriteSlot((Slot)Item!);
            }


        }

    }



    public class ExperiencePacket : Packet {
        public const int ProtocolId = 0x51;

        public float? ExperienceBar { get; set; }
        public int? Level { get; set; }
        public int? TotalExperience { get; set; }

        public ExperiencePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ExperienceBar = buffer.ReadF32();
            this.Level = buffer.ReadVarint();
            this.TotalExperience = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF32((float)ExperienceBar!);
            buffer.WriteVarint((int)Level!);
            buffer.WriteVarint((int)TotalExperience!);
        }

    }



    public class UpdateHealthPacket : Packet {
        public const int ProtocolId = 0x52;

        public float? Health { get; set; }
        public int? Food { get; set; }
        public float? FoodSaturation { get; set; }

        public UpdateHealthPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Health = buffer.ReadF32();
            this.Food = buffer.ReadVarint();
            this.FoodSaturation = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF32((float)Health!);
            buffer.WriteVarint((int)Food!);
            buffer.WriteF32((float)FoodSaturation!);
        }

    }



    public class ScoreboardObjectivePacket : Packet {
        public const int ProtocolId = 0x53;

        public string? Name { get; set; }
        public sbyte? Action { get; set; }
        public DisplayTextSwitch? DisplayText { get; set; }
        public TypeSwitch? Type { get; set; }

        public ScoreboardObjectivePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Name = buffer.ReadString();
            this.Action = buffer.ReadI8();
            this.DisplayText = buffer.Read<DisplayTextSwitch>(new DisplayTextSwitch((sbyte)Action!));
            this.Type = buffer.Read<TypeSwitch>(new TypeSwitch((sbyte)Action!));
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Name!);
            buffer.WriteI8((sbyte)Action!);
            buffer.Write((DisplayTextSwitch)DisplayText!);
            buffer.Write((TypeSwitch)Type!);
        }

        public class DisplayTextSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public DisplayTextSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public DisplayTextSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadString(),
                    2 => buffer.ReadString(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteString((string)this.Value!); break;
                    case 2: buffer.WriteString((string)this.Value!); break;

                }

            }


        }

        public class TypeSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public TypeSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public TypeSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadVarint(),
                    2 => buffer.ReadVarint(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteVarint((int)this.Value!); break;
                    case 2: buffer.WriteVarint((int)this.Value!); break;

                }

            }


        }

    }



    public class SetPassengersPacket : Packet {
        public const int ProtocolId = 0x54;

        public int? EntityId { get; set; }
        public int[]? Passengers { get; set; }

        public SetPassengersPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.Passengers = buffer.ReadArray<int>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteArray((int[])Passengers!);
        }

    }



    public class TeamsPacket : Packet {
        public const int ProtocolId = 0x55;

        public string? Team { get; set; }
        public sbyte? Mode { get; set; }
        public NameSwitch? Name { get; set; }
        public FriendlyFireSwitch? FriendlyFire { get; set; }
        public NameTagVisibilitySwitch? NameTagVisibility { get; set; }
        public CollisionRuleSwitch? CollisionRule { get; set; }
        public FormattingSwitch? Formatting { get; set; }
        public PrefixSwitch? Prefix { get; set; }
        public SuffixSwitch? Suffix { get; set; }
        public PlayersSwitch? Players { get; set; }

        public TeamsPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Team = buffer.ReadString();
            this.Mode = buffer.ReadI8();
            this.Name = buffer.Read<NameSwitch>(new NameSwitch((sbyte)Mode!));
            this.FriendlyFire = buffer.Read<FriendlyFireSwitch>(new FriendlyFireSwitch((sbyte)Mode!));
            this.NameTagVisibility = buffer.Read<NameTagVisibilitySwitch>(new NameTagVisibilitySwitch((sbyte)Mode!));
            this.CollisionRule = buffer.Read<CollisionRuleSwitch>(new CollisionRuleSwitch((sbyte)Mode!));
            this.Formatting = buffer.Read<FormattingSwitch>(new FormattingSwitch((sbyte)Mode!));
            this.Prefix = buffer.Read<PrefixSwitch>(new PrefixSwitch((sbyte)Mode!));
            this.Suffix = buffer.Read<SuffixSwitch>(new SuffixSwitch((sbyte)Mode!));
            this.Players = buffer.Read<PlayersSwitch>(new PlayersSwitch((sbyte)Mode!));
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Team!);
            buffer.WriteI8((sbyte)Mode!);
            buffer.Write((NameSwitch)Name!);
            buffer.Write((FriendlyFireSwitch)FriendlyFire!);
            buffer.Write((NameTagVisibilitySwitch)NameTagVisibility!);
            buffer.Write((CollisionRuleSwitch)CollisionRule!);
            buffer.Write((FormattingSwitch)Formatting!);
            buffer.Write((PrefixSwitch)Prefix!);
            buffer.Write((SuffixSwitch)Suffix!);
            buffer.Write((PlayersSwitch)Players!);
        }

        public class NameSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public NameSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public NameSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadString(),
                    2 => buffer.ReadString(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteString((string)this.Value!); break;
                    case 2: buffer.WriteString((string)this.Value!); break;

                }

            }


        }

        public class FriendlyFireSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public FriendlyFireSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public FriendlyFireSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadI8(),
                    2 => buffer.ReadI8(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteI8((sbyte)this.Value!); break;
                    case 2: buffer.WriteI8((sbyte)this.Value!); break;

                }

            }


        }

        public class NameTagVisibilitySwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public NameTagVisibilitySwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public NameTagVisibilitySwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadString(),
                    2 => buffer.ReadString(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteString((string)this.Value!); break;
                    case 2: buffer.WriteString((string)this.Value!); break;

                }

            }


        }

        public class CollisionRuleSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public CollisionRuleSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public CollisionRuleSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadString(),
                    2 => buffer.ReadString(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteString((string)this.Value!); break;
                    case 2: buffer.WriteString((string)this.Value!); break;

                }

            }


        }

        public class FormattingSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public FormattingSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public FormattingSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadVarint(),
                    2 => buffer.ReadVarint(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteVarint((int)this.Value!); break;
                    case 2: buffer.WriteVarint((int)this.Value!); break;

                }

            }


        }

        public class PrefixSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public PrefixSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public PrefixSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadString(),
                    2 => buffer.ReadString(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteString((string)this.Value!); break;
                    case 2: buffer.WriteString((string)this.Value!); break;

                }

            }


        }

        public class SuffixSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public SuffixSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public SuffixSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadString(),
                    2 => buffer.ReadString(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteString((string)this.Value!); break;
                    case 2: buffer.WriteString((string)this.Value!); break;

                }

            }


        }

        public class PlayersSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public PlayersSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public PlayersSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadArray<string>(buffer.ReadVarint()),
                    3 => buffer.ReadArray<string>(buffer.ReadVarint()),
                    4 => buffer.ReadArray<string>(buffer.ReadVarint()),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteArray((string[])this.Value!); break;
                    case 3: buffer.WriteArray((string[])this.Value!); break;
                    case 4: buffer.WriteArray((string[])this.Value!); break;

                }

            }


        }

    }



    public class ScoreboardScorePacket : Packet {
        public const int ProtocolId = 0x56;

        public string? ItemName { get; set; }
        public sbyte? Action { get; set; }
        public string? ScoreName { get; set; }
        public ValueSwitch? Value { get; set; }

        public ScoreboardScorePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ItemName = buffer.ReadString();
            this.Action = buffer.ReadI8();
            this.ScoreName = buffer.ReadString();
            this.Value = buffer.Read<ValueSwitch>(new ValueSwitch((sbyte)Action!));
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)ItemName!);
            buffer.WriteI8((sbyte)Action!);
            buffer.WriteString((string)ScoreName!);
            buffer.Write((ValueSwitch)Value!);
        }

        public class ValueSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public ValueSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public ValueSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    1 => buffer.ReadVoid(),
                    _ => buffer.ReadVarint()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 1: buffer.WriteVoid((object)this.Value!); break;

                }

            }


        }

    }



    public class SpawnPositionPacket : Packet {
        public const int ProtocolId = 0x4b;

        public Position? Location { get; set; }
        public float? Angle { get; set; }

        public SpawnPositionPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Angle = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteF32((float)Angle!);
        }

    }



    public class UpdateTimePacket : Packet {
        public const int ProtocolId = 0x59;

        public long? Age { get; set; }
        public long? Time { get; set; }

        public UpdateTimePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Age = buffer.ReadI64();
            this.Time = buffer.ReadI64();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI64((long)Age!);
            buffer.WriteI64((long)Time!);
        }

    }



    public class EntitySoundEffectPacket : Packet {
        public const int ProtocolId = 0x5c;

        public int? SoundId { get; set; }
        public int? SoundCategory { get; set; }
        public int? EntityId { get; set; }
        public float? Volume { get; set; }
        public float? Pitch { get; set; }

        public EntitySoundEffectPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.SoundId = buffer.ReadVarint();
            this.SoundCategory = buffer.ReadVarint();
            this.EntityId = buffer.ReadVarint();
            this.Volume = buffer.ReadF32();
            this.Pitch = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)SoundId!);
            buffer.WriteVarint((int)SoundCategory!);
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteF32((float)Volume!);
            buffer.WriteF32((float)Pitch!);
        }

    }



    public class StopSoundPacket : Packet {
        public const int ProtocolId = 0x5e;

        public sbyte? Flags { get; set; }
        public SourceSwitch? Source { get; set; }
        public SoundSwitch? Sound { get; set; }

        public StopSoundPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Flags = buffer.ReadI8();
            this.Source = buffer.Read<SourceSwitch>(new SourceSwitch((sbyte)Flags!));
            this.Sound = buffer.Read<SoundSwitch>(new SoundSwitch((sbyte)Flags!));
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)Flags!);
            buffer.Write((SourceSwitch)Source!);
            buffer.Write((SoundSwitch)Sound!);
        }

        public class SourceSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public SourceSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public SourceSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    3 => buffer.ReadVarint(),
                    1 => buffer.ReadVarint(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 3: buffer.WriteVarint((int)this.Value!); break;
                    case 1: buffer.WriteVarint((int)this.Value!); break;

                }

            }


        }

        public class SoundSwitch : IPacketPayload {

            public object? Value { get; set; }
            public sbyte SwitchState { get; set; }

            public SoundSwitch(object? value, sbyte switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public SoundSwitch(sbyte switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    3 => buffer.ReadString(),
                    2 => buffer.ReadString(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 3: buffer.WriteString((string)this.Value!); break;
                    case 2: buffer.WriteString((string)this.Value!); break;

                }

            }


        }

    }



    public class SoundEffectPacket : Packet {
        public const int ProtocolId = 0x5d;

        public int? SoundId { get; set; }
        public int? SoundCategory { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Z { get; set; }
        public float? Volume { get; set; }
        public float? Pitch { get; set; }

        public SoundEffectPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.SoundId = buffer.ReadVarint();
            this.SoundCategory = buffer.ReadVarint();
            this.X = buffer.ReadI32();
            this.Y = buffer.ReadI32();
            this.Z = buffer.ReadI32();
            this.Volume = buffer.ReadF32();
            this.Pitch = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)SoundId!);
            buffer.WriteVarint((int)SoundCategory!);
            buffer.WriteI32((int)X!);
            buffer.WriteI32((int)Y!);
            buffer.WriteI32((int)Z!);
            buffer.WriteF32((float)Volume!);
            buffer.WriteF32((float)Pitch!);
        }

    }



    public class PlayerlistHeaderPacket : Packet {
        public const int ProtocolId = 0x5f;

        public string? Header { get; set; }
        public string? Footer { get; set; }

        public PlayerlistHeaderPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Header = buffer.ReadString();
            this.Footer = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Header!);
            buffer.WriteString((string)Footer!);
        }

    }



    public class CollectPacket : Packet {
        public const int ProtocolId = 0x61;

        public int? CollectedEntityId { get; set; }
        public int? CollectorEntityId { get; set; }
        public int? PickupItemCount { get; set; }

        public CollectPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.CollectedEntityId = buffer.ReadVarint();
            this.CollectorEntityId = buffer.ReadVarint();
            this.PickupItemCount = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)CollectedEntityId!);
            buffer.WriteVarint((int)CollectorEntityId!);
            buffer.WriteVarint((int)PickupItemCount!);
        }

    }



    public class EntityTeleportPacket : Packet {
        public const int ProtocolId = 0x62;

        public int? EntityId { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public sbyte? Yaw { get; set; }
        public sbyte? Pitch { get; set; }
        public bool? OnGround { get; set; }

        public EntityTeleportPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.Yaw = buffer.ReadI8();
            this.Pitch = buffer.ReadI8();
            this.OnGround = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteI8((sbyte)Yaw!);
            buffer.WriteI8((sbyte)Pitch!);
            buffer.WriteBool((bool)OnGround!);
        }

    }



    public class EntityUpdateAttributesPacket : Packet {
        public const int ProtocolId = 0x64;

        public int? EntityId { get; set; }
        public PropertiesContainer[]? Properties { get; set; }

        public EntityUpdateAttributesPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.Properties = buffer.ReadArray<PropertiesContainer>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteArray((PropertiesContainer[])Properties!);
        }

        public class PropertiesContainer : IPacketPayload {
            public string? Key { get; set; }
            public double? Value { get; set; }
            public ModifiersContainer[]? Modifiers { get; set; }

            public PropertiesContainer() { }

            public void Read(PacketBuffer buffer) {
                this.Key = buffer.ReadString();
                this.Value = buffer.ReadF64();
                this.Modifiers = buffer.ReadArray<ModifiersContainer>(buffer.ReadVarint());
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteString((string)Key!);
                buffer.WriteF64((double)Value!);
                buffer.WriteArray((ModifiersContainer[])Modifiers!);
            }

            public class ModifiersContainer : IPacketPayload {
                public UUID? Uuid { get; set; }
                public double? Amount { get; set; }
                public sbyte? Operation { get; set; }

                public ModifiersContainer() { }

                public void Read(PacketBuffer buffer) {
                    this.Uuid = buffer.ReadUUID();
                    this.Amount = buffer.ReadF64();
                    this.Operation = buffer.ReadI8();
                }

                public void Write(PacketBuffer buffer) {
                    buffer.WriteUUID((UUID)Uuid!);
                    buffer.WriteF64((double)Amount!);
                    buffer.WriteI8((sbyte)Operation!);
                }


            }


        }

    }



    public class EntityEffectPacket : Packet {
        public const int ProtocolId = 0x65;

        public int? EntityId { get; set; }
        public sbyte? EffectId { get; set; }
        public sbyte? Amplifier { get; set; }
        public int? Duration { get; set; }
        public sbyte? HideParticles { get; set; }

        public EntityEffectPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.EffectId = buffer.ReadI8();
            this.Amplifier = buffer.ReadI8();
            this.Duration = buffer.ReadVarint();
            this.HideParticles = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteI8((sbyte)EffectId!);
            buffer.WriteI8((sbyte)Amplifier!);
            buffer.WriteVarint((int)Duration!);
            buffer.WriteI8((sbyte)HideParticles!);
        }

    }



    public class SelectAdvancementTabPacket : Packet {
        public const int ProtocolId = 0x40;

        public string? Id { get; set; }

        public SelectAdvancementTabPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Id = buffer.ReadOption<string>();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteOption((string)Id!);
        }

    }



    public class DeclareRecipesPacket : Packet {
        public const int ProtocolId = 0x66;

        public RecipesContainer[]? Recipes { get; set; }

        public DeclareRecipesPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Recipes = buffer.ReadArray<RecipesContainer>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteArray((RecipesContainer[])Recipes!);
        }

        public class RecipesContainer : IPacketPayload {
            public string? Type { get; set; }
            public string? RecipeId { get; set; }
            public DataSwitch? Data { get; set; }

            public RecipesContainer() { }

            public void Read(PacketBuffer buffer) {
                this.Type = buffer.ReadString();
                this.RecipeId = buffer.ReadString();
                this.Data = buffer.Read<DataSwitch>(new DataSwitch((string)Type!));
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteString((string)Type!);
                buffer.WriteString((string)RecipeId!);
                buffer.Write((DataSwitch)Data!);
            }

            public class DataSwitch : IPacketPayload {

                public object? Value { get; set; }
                public string SwitchState { get; set; }

                public DataSwitch(object? value, string switchState) {
                    this.Value = value;
                    this.SwitchState = switchState;
                }

                public DataSwitch(string switchState) {
                    this.SwitchState = switchState;
                }

                public void Read(PacketBuffer buffer) {

                    this.Value = SwitchState switch {
                        "minecraft:crafting_shapeless" => buffer.Read(new Container()),
                        "minecraft:crafting_shaped" => buffer.Read(new Container()),
                        "minecraft:crafting_special_armordye" => buffer.ReadVoid(),
                        "minecraft:crafting_special_bookcloning" => buffer.ReadVoid(),
                        "minecraft:crafting_special_mapcloning" => buffer.ReadVoid(),
                        "minecraft:crafting_special_mapextending" => buffer.ReadVoid(),
                        "minecraft:crafting_special_firework_rocket" => buffer.ReadVoid(),
                        "minecraft:crafting_special_firework_star" => buffer.ReadVoid(),
                        "minecraft:crafting_special_firework_star_fade" => buffer.ReadVoid(),
                        "minecraft:crafting_special_repairitem" => buffer.ReadVoid(),
                        "minecraft:crafting_special_tippedarrow" => buffer.ReadVoid(),
                        "minecraft:crafting_special_bannerduplicate" => buffer.ReadVoid(),
                        "minecraft:crafting_special_banneraddpattern" => buffer.ReadVoid(),
                        "minecraft:crafting_special_shielddecoration" => buffer.ReadVoid(),
                        "minecraft:crafting_special_shulkerboxcoloring" => buffer.ReadVoid(),
                        "minecraft:crafting_special_suspiciousstew" => buffer.ReadVoid(),
                        "minecraft:smelting" => buffer.ReadMinecraftSmeltingFormat(),
                        "minecraft:blasting" => buffer.ReadMinecraftSmeltingFormat(),
                        "minecraft:smoking" => buffer.ReadMinecraftSmeltingFormat(),
                        "minecraft:campfire_cooking" => buffer.ReadMinecraftSmeltingFormat(),
                        "minecraft:stonecutting" => buffer.Read(new Container()),
                        "minecraft:smithing" => buffer.Read(new Container()),
                        _ => throw new Exception()
                    };

                }

                public void Write(PacketBuffer buffer) {
                    switch (SwitchState) {
                        case "minecraft:crafting_shapeless": buffer.Write((Container)this.Value!); break;
                        case "minecraft:crafting_shaped": buffer.Write((Container)this.Value!); break;
                        case "minecraft:crafting_special_armordye": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_bookcloning": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_mapcloning": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_mapextending": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_firework_rocket": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_firework_star": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_firework_star_fade": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_repairitem": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_tippedarrow": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_bannerduplicate": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_banneraddpattern": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_shielddecoration": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_shulkerboxcoloring": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:crafting_special_suspiciousstew": buffer.WriteVoid((object)this.Value!); break;
                        case "minecraft:smelting": buffer.WriteMinecraftSmeltingFormat((MinecraftSmeltingFormat)this.Value!); break;
                        case "minecraft:blasting": buffer.WriteMinecraftSmeltingFormat((MinecraftSmeltingFormat)this.Value!); break;
                        case "minecraft:smoking": buffer.WriteMinecraftSmeltingFormat((MinecraftSmeltingFormat)this.Value!); break;
                        case "minecraft:campfire_cooking": buffer.WriteMinecraftSmeltingFormat((MinecraftSmeltingFormat)this.Value!); break;
                        case "minecraft:stonecutting": buffer.Write((Container)this.Value!); break;
                        case "minecraft:smithing": buffer.Write((Container)this.Value!); break;

                    }

                }

                public class Container : IPacketPayload {
                    public string? Group { get; set; }
                    public Ingredient[]? Ingredients { get; set; }
                    public Slot? Result { get; set; }

                    public Container() { }

                    public void Read(PacketBuffer buffer) {
                        this.Group = buffer.ReadString();
                        this.Ingredients = buffer.ReadArray<Ingredient>(buffer.ReadVarint());
                        this.Result = buffer.ReadSlot();
                    }

                    public void Write(PacketBuffer buffer) {
                        buffer.WriteString((string)Group!);
                        buffer.WriteArray((Ingredient[])Ingredients!);
                        buffer.WriteSlot((Slot)Result!);
                    }


                }

                public class Container1 : IPacketPayload {
                    public int? Width { get; set; }
                    public int? Height { get; set; }
                    public string? Group { get; set; }
                    public Ingredient[][]? Ingredients { get; set; }
                    public Slot? Result { get; set; }

                    public Container1() { }

                    public void Read(PacketBuffer buffer) {
                        this.Width = buffer.ReadVarint();
                        this.Height = buffer.ReadVarint();
                        this.Group = buffer.ReadString();
                        this.Ingredients = buffer.ReadArray<Ingredient[]>((int)Width!);
                        this.Result = buffer.ReadSlot();
                    }

                    public void Write(PacketBuffer buffer) {
                        buffer.WriteVarint((int)Width!);
                        buffer.WriteVarint((int)Height!);
                        buffer.WriteString((string)Group!);
                        buffer.WriteArray((Ingredient[][])Ingredients!);
                        buffer.WriteSlot((Slot)Result!);
                    }


                }

                public class Container2 : IPacketPayload {
                    public string? Group { get; set; }
                    public Ingredient? Ingredient { get; set; }
                    public Slot? Result { get; set; }

                    public Container2() { }

                    public void Read(PacketBuffer buffer) {
                        this.Group = buffer.ReadString();
                        this.Ingredient = buffer.ReadIngredient();
                        this.Result = buffer.ReadSlot();
                    }

                    public void Write(PacketBuffer buffer) {
                        buffer.WriteString((string)Group!);
                        buffer.WriteIngredient((Ingredient)Ingredient!);
                        buffer.WriteSlot((Slot)Result!);
                    }


                }

                public class Container3 : IPacketPayload {
                    public Ingredient? Base { get; set; }
                    public Ingredient? Addition { get; set; }
                    public Slot? Result { get; set; }

                    public Container3() { }

                    public void Read(PacketBuffer buffer) {
                        this.Base = buffer.ReadIngredient();
                        this.Addition = buffer.ReadIngredient();
                        this.Result = buffer.ReadSlot();
                    }

                    public void Write(PacketBuffer buffer) {
                        buffer.WriteIngredient((Ingredient)Base!);
                        buffer.WriteIngredient((Ingredient)Addition!);
                        buffer.WriteSlot((Slot)Result!);
                    }


                }


            }


        }

    }



    public class TagsPacket : Packet {
        public const int ProtocolId = 0x67;

        public TagsContainer[]? Tags { get; set; }

        public TagsPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Tags = buffer.ReadArray<TagsContainer>(buffer.ReadVarint());
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteArray((TagsContainer[])Tags!);
        }

        public class TagsContainer : IPacketPayload {
            public string? TagType { get; set; }
            public Tags? Tags { get; set; }

            public TagsContainer() { }

            public void Read(PacketBuffer buffer) {
                this.TagType = buffer.ReadString();
                this.Tags = buffer.ReadTags();
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteString((string)TagType!);
                buffer.WriteTags((Tags)Tags!);
            }


        }

    }



    public class AcknowledgePlayerDiggingPacket : Packet {
        public const int ProtocolId = 0x08;

        public Position? Location { get; set; }
        public int? Block { get; set; }
        public int? Status { get; set; }
        public bool? Successful { get; set; }

        public AcknowledgePlayerDiggingPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Block = buffer.ReadVarint();
            this.Status = buffer.ReadVarint();
            this.Successful = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteVarint((int)Block!);
            buffer.WriteVarint((int)Status!);
            buffer.WriteBool((bool)Successful!);
        }

    }



    public class SculkVibrationSignalPacket : Packet {
        public const int ProtocolId = 0x05;

        public Position? SourcePosition { get; set; }
        public string? DestinationIdentifier { get; set; }
        public DestinationSwitch? Destination { get; set; }
        public int? ArrivalTicks { get; set; }

        public SculkVibrationSignalPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.SourcePosition = buffer.ReadPosition();
            this.DestinationIdentifier = buffer.ReadString();
            this.Destination = buffer.Read<DestinationSwitch>(new DestinationSwitch((string)DestinationIdentifier!));
            this.ArrivalTicks = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)SourcePosition!);
            buffer.WriteString((string)DestinationIdentifier!);
            buffer.Write((DestinationSwitch)Destination!);
            buffer.WriteVarint((int)ArrivalTicks!);
        }

        public class DestinationSwitch : IPacketPayload {

            public object? Value { get; set; }
            public string SwitchState { get; set; }

            public DestinationSwitch(object? value, string switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public DestinationSwitch(string switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    "block" => buffer.ReadPosition(),
                    "entityId" => buffer.ReadVarint(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case "block": buffer.WritePosition((Position)this.Value!); break;
                    case "entityId": buffer.WriteVarint((int)this.Value!); break;

                }

            }


        }

    }



    public class ClearTitlesPacket : Packet {
        public const int ProtocolId = 0x10;

        public bool? Reset { get; set; }

        public ClearTitlesPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Reset = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBool((bool)Reset!);
        }

    }



    public class InitializeWorldBorderPacket : Packet {
        public const int ProtocolId = 0x20;

        public double? X { get; set; }
        public double? Z { get; set; }
        public double? OldDiameter { get; set; }
        public double? NewDiameter { get; set; }
        public int? Speed { get; set; }
        public int? PortalTeleportBoundary { get; set; }
        public int? WarningBlocks { get; set; }
        public int? WarningTime { get; set; }

        public InitializeWorldBorderPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.OldDiameter = buffer.ReadF64();
            this.NewDiameter = buffer.ReadF64();
            this.Speed = buffer.ReadVarint();
            this.PortalTeleportBoundary = buffer.ReadVarint();
            this.WarningBlocks = buffer.ReadVarint();
            this.WarningTime = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Z!);
            buffer.WriteF64((double)OldDiameter!);
            buffer.WriteF64((double)NewDiameter!);
            buffer.WriteVarint((int)Speed!);
            buffer.WriteVarint((int)PortalTeleportBoundary!);
            buffer.WriteVarint((int)WarningBlocks!);
            buffer.WriteVarint((int)WarningTime!);
        }

    }



    public class ActionBarPacket : Packet {
        public const int ProtocolId = 0x41;

        public string? Text { get; set; }

        public ActionBarPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Text = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Text!);
        }

    }



    public class WorldBorderCenterPacket : Packet {
        public const int ProtocolId = 0x42;

        public double? X { get; set; }
        public double? Z { get; set; }

        public WorldBorderCenterPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadF64();
            this.Z = buffer.ReadF64();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Z!);
        }

    }



    public class WorldBorderLerpSizePacket : Packet {
        public const int ProtocolId = 0x43;

        public double? OldDiameter { get; set; }
        public double? NewDiameter { get; set; }
        public int? Speed { get; set; }

        public WorldBorderLerpSizePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.OldDiameter = buffer.ReadF64();
            this.NewDiameter = buffer.ReadF64();
            this.Speed = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF64((double)OldDiameter!);
            buffer.WriteF64((double)NewDiameter!);
            buffer.WriteVarint((int)Speed!);
        }

    }



    public class WorldBorderSizePacket : Packet {
        public const int ProtocolId = 0x44;

        public double? Diameter { get; set; }

        public WorldBorderSizePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Diameter = buffer.ReadF64();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF64((double)Diameter!);
        }

    }



    public class WorldBorderWarningDelayPacket : Packet {
        public const int ProtocolId = 0x45;

        public int? WarningTime { get; set; }

        public WorldBorderWarningDelayPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WarningTime = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)WarningTime!);
        }

    }



    public class WorldBorderWarningReachPacket : Packet {
        public const int ProtocolId = 0x46;

        public int? WarningBlocks { get; set; }

        public WorldBorderWarningReachPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WarningBlocks = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)WarningBlocks!);
        }

    }



    public class PingPacket : Packet {
        public const int ProtocolId = 0x30;

        public int? Id { get; set; }

        public PingPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Id = buffer.ReadI32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)Id!);
        }

    }



    public class SetTitleSubtitlePacket : Packet {
        public const int ProtocolId = 0x58;

        public string? Text { get; set; }

        public SetTitleSubtitlePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Text = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Text!);
        }

    }



    public class SetTitleTextPacket : Packet {
        public const int ProtocolId = 0x5a;

        public string? Text { get; set; }

        public SetTitleTextPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Text = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Text!);
        }

    }



    public class SetTitleTimePacket : Packet {
        public const int ProtocolId = 0x5b;

        public int? FadeIn { get; set; }
        public int? Stay { get; set; }
        public int? FadeOut { get; set; }

        public SetTitleTimePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.FadeIn = buffer.ReadI32();
            this.Stay = buffer.ReadI32();
            this.FadeOut = buffer.ReadI32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)FadeIn!);
            buffer.WriteI32((int)Stay!);
            buffer.WriteI32((int)FadeOut!);
        }

    }



    public class SimulationDistancePacket : Packet {
        public const int ProtocolId = 0x57;

        public int? Distance { get; set; }

        public SimulationDistancePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Distance = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Distance!);
        }

    }


}

namespace MineSharp.Data.Protocol.Serverbound.Handshaking {



    public class SetProtocolPacket : Packet {
        public const int ProtocolId = 0x00;

        public int? ProtocolVersion { get; set; }
        public string? ServerHost { get; set; }
        public ushort? ServerPort { get; set; }
        public int? NextState { get; set; }

        public SetProtocolPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ProtocolVersion = buffer.ReadVarint();
            this.ServerHost = buffer.ReadString();
            this.ServerPort = buffer.ReadU16();
            this.NextState = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)ProtocolVersion!);
            buffer.WriteString((string)ServerHost!);
            buffer.WriteU16((ushort)ServerPort!);
            buffer.WriteVarint((int)NextState!);
        }

    }



    public class LegacyServerListPingPacket : Packet {
        public const int ProtocolId = 0xfe;

        public byte? Payload { get; set; }

        public LegacyServerListPingPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Payload = buffer.ReadU8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)Payload!);
        }

    }


}
namespace MineSharp.Data.Protocol.Serverbound.Status {



    public class PingStartPacket : Packet {
        public const int ProtocolId = 0x00;


        public PingStartPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
        }

        public override void Write(PacketBuffer buffer) {
        }

    }



    public class PingPacket : Packet {
        public const int ProtocolId = 0x01;

        public long? Time { get; set; }

        public PingPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Time = buffer.ReadI64();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI64((long)Time!);
        }

    }


}
namespace MineSharp.Data.Protocol.Serverbound.Login {



    public class LoginStartPacket : Packet {
        public const int ProtocolId = 0x00;

        public string? Username { get; set; }

        public LoginStartPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Username = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Username!);
        }

    }



    public class EncryptionBeginPacket : Packet {
        public const int ProtocolId = 0x01;

        public byte[]? SharedSecret { get; set; }
        public byte[]? VerifyToken { get; set; }

        public EncryptionBeginPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.SharedSecret = buffer.ReadBuffer();
            this.VerifyToken = buffer.ReadBuffer();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBuffer((byte[])SharedSecret!);
            buffer.WriteBuffer((byte[])VerifyToken!);
        }

    }



    public class LoginPluginResponsePacket : Packet {
        public const int ProtocolId = 0x02;

        public int? MessageId { get; set; }
        public byte[]? Data { get; set; }

        public LoginPluginResponsePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.MessageId = buffer.ReadVarint();
            this.Data = buffer.ReadOption<byte[]>();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)MessageId!);
            buffer.WriteOption((byte[])Data!);
        }

    }


}
namespace MineSharp.Data.Protocol.Serverbound.Play {



    public class TeleportConfirmPacket : Packet {
        public const int ProtocolId = 0x00;

        public int? TeleportId { get; set; }

        public TeleportConfirmPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.TeleportId = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)TeleportId!);
        }

    }



    public class QueryBlockNbtPacket : Packet {
        public const int ProtocolId = 0x01;

        public int? TransactionId { get; set; }
        public Position? Location { get; set; }

        public QueryBlockNbtPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.TransactionId = buffer.ReadVarint();
            this.Location = buffer.ReadPosition();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)TransactionId!);
            buffer.WritePosition((Position)Location!);
        }

    }



    public class SetDifficultyPacket : Packet {
        public const int ProtocolId = 0x02;

        public byte? NewDifficulty { get; set; }

        public SetDifficultyPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.NewDifficulty = buffer.ReadU8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)NewDifficulty!);
        }

    }



    public class EditBookPacket : Packet {
        public const int ProtocolId = 0x0b;

        public int? Hand { get; set; }
        public string[]? Pages { get; set; }
        public string? Title { get; set; }

        public EditBookPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Hand = buffer.ReadVarint();
            this.Pages = buffer.ReadArray<string>(buffer.ReadVarint());
            this.Title = buffer.ReadOption<string>();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Hand!);
            buffer.WriteArray((string[])Pages!);
            buffer.WriteOption((string)Title!);
        }

    }



    public class QueryEntityNbtPacket : Packet {
        public const int ProtocolId = 0x0c;

        public int? TransactionId { get; set; }
        public int? EntityId { get; set; }

        public QueryEntityNbtPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.TransactionId = buffer.ReadVarint();
            this.EntityId = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)TransactionId!);
            buffer.WriteVarint((int)EntityId!);
        }

    }



    public class PickItemPacket : Packet {
        public const int ProtocolId = 0x17;

        public int? Slot { get; set; }

        public PickItemPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Slot = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Slot!);
        }

    }



    public class NameItemPacket : Packet {
        public const int ProtocolId = 0x20;

        public string? Name { get; set; }

        public NameItemPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Name = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Name!);
        }

    }



    public class SelectTradePacket : Packet {
        public const int ProtocolId = 0x23;

        public int? Slot { get; set; }

        public SelectTradePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Slot = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Slot!);
        }

    }



    public class SetBeaconEffectPacket : Packet {
        public const int ProtocolId = 0x24;

        public int? Primary_effect { get; set; }
        public int? Secondary_effect { get; set; }

        public SetBeaconEffectPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Primary_effect = buffer.ReadVarint();
            this.Secondary_effect = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Primary_effect!);
            buffer.WriteVarint((int)Secondary_effect!);
        }

    }



    public class UpdateCommandBlockPacket : Packet {
        public const int ProtocolId = 0x26;

        public Position? Location { get; set; }
        public string? Command { get; set; }
        public int? Mode { get; set; }
        public byte? Flags { get; set; }

        public UpdateCommandBlockPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Command = buffer.ReadString();
            this.Mode = buffer.ReadVarint();
            this.Flags = buffer.ReadU8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteString((string)Command!);
            buffer.WriteVarint((int)Mode!);
            buffer.WriteU8((byte)Flags!);
        }

    }



    public class UpdateCommandBlockMinecartPacket : Packet {
        public const int ProtocolId = 0x27;

        public int? EntityId { get; set; }
        public string? Command { get; set; }
        public bool? Track_output { get; set; }

        public UpdateCommandBlockMinecartPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.Command = buffer.ReadString();
            this.Track_output = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteString((string)Command!);
            buffer.WriteBool((bool)Track_output!);
        }

    }



    public class UpdateStructureBlockPacket : Packet {
        public const int ProtocolId = 0x2a;

        public Position? Location { get; set; }
        public int? Action { get; set; }
        public int? Mode { get; set; }
        public string? Name { get; set; }
        public byte? Offset_x { get; set; }
        public byte? Offset_y { get; set; }
        public byte? Offset_z { get; set; }
        public byte? Size_x { get; set; }
        public byte? Size_y { get; set; }
        public byte? Size_z { get; set; }
        public int? Mirror { get; set; }
        public int? Rotation { get; set; }
        public string? Metadata { get; set; }
        public float? Integrity { get; set; }
        public int? Seed { get; set; }
        public byte? Flags { get; set; }

        public UpdateStructureBlockPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Action = buffer.ReadVarint();
            this.Mode = buffer.ReadVarint();
            this.Name = buffer.ReadString();
            this.Offset_x = buffer.ReadU8();
            this.Offset_y = buffer.ReadU8();
            this.Offset_z = buffer.ReadU8();
            this.Size_x = buffer.ReadU8();
            this.Size_y = buffer.ReadU8();
            this.Size_z = buffer.ReadU8();
            this.Mirror = buffer.ReadVarint();
            this.Rotation = buffer.ReadVarint();
            this.Metadata = buffer.ReadString();
            this.Integrity = buffer.ReadF32();
            this.Seed = buffer.ReadVarint();
            this.Flags = buffer.ReadU8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteVarint((int)Action!);
            buffer.WriteVarint((int)Mode!);
            buffer.WriteString((string)Name!);
            buffer.WriteU8((byte)Offset_x!);
            buffer.WriteU8((byte)Offset_y!);
            buffer.WriteU8((byte)Offset_z!);
            buffer.WriteU8((byte)Size_x!);
            buffer.WriteU8((byte)Size_y!);
            buffer.WriteU8((byte)Size_z!);
            buffer.WriteVarint((int)Mirror!);
            buffer.WriteVarint((int)Rotation!);
            buffer.WriteString((string)Metadata!);
            buffer.WriteF32((float)Integrity!);
            buffer.WriteVarint((int)Seed!);
            buffer.WriteU8((byte)Flags!);
        }

    }



    public class TabCompletePacket : Packet {
        public const int ProtocolId = 0x06;

        public int? TransactionId { get; set; }
        public string? Text { get; set; }

        public TabCompletePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.TransactionId = buffer.ReadVarint();
            this.Text = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)TransactionId!);
            buffer.WriteString((string)Text!);
        }

    }



    public class ChatPacket : Packet {
        public const int ProtocolId = 0x03;

        public string? Message { get; set; }

        public ChatPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Message = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Message!);
        }

    }



    public class ClientCommandPacket : Packet {
        public const int ProtocolId = 0x04;

        public int? ActionId { get; set; }

        public ClientCommandPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.ActionId = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)ActionId!);
        }

    }



    public class SettingsPacket : Packet {
        public const int ProtocolId = 0x05;

        public string? Locale { get; set; }
        public sbyte? ViewDistance { get; set; }
        public int? ChatFlags { get; set; }
        public bool? ChatColors { get; set; }
        public byte? SkinParts { get; set; }
        public int? MainHand { get; set; }
        public bool? EnableTextFiltering { get; set; }
        public bool? EnableServerListing { get; set; }

        public SettingsPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Locale = buffer.ReadString();
            this.ViewDistance = buffer.ReadI8();
            this.ChatFlags = buffer.ReadVarint();
            this.ChatColors = buffer.ReadBool();
            this.SkinParts = buffer.ReadU8();
            this.MainHand = buffer.ReadVarint();
            this.EnableTextFiltering = buffer.ReadBool();
            this.EnableServerListing = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Locale!);
            buffer.WriteI8((sbyte)ViewDistance!);
            buffer.WriteVarint((int)ChatFlags!);
            buffer.WriteBool((bool)ChatColors!);
            buffer.WriteU8((byte)SkinParts!);
            buffer.WriteVarint((int)MainHand!);
            buffer.WriteBool((bool)EnableTextFiltering!);
            buffer.WriteBool((bool)EnableServerListing!);
        }

    }



    public class EnchantItemPacket : Packet {
        public const int ProtocolId = 0x07;

        public sbyte? WindowId { get; set; }
        public sbyte? Enchantment { get; set; }

        public EnchantItemPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadI8();
            this.Enchantment = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)WindowId!);
            buffer.WriteI8((sbyte)Enchantment!);
        }

    }



    public class WindowClickPacket : Packet {
        public const int ProtocolId = 0x08;

        public byte? WindowId { get; set; }
        public int? StateId { get; set; }
        public short? Slot { get; set; }
        public sbyte? MouseButton { get; set; }
        public int? Mode { get; set; }
        public ChangedSlotsContainer[]? ChangedSlots { get; set; }
        public Slot? CursorItem { get; set; }

        public WindowClickPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadU8();
            this.StateId = buffer.ReadVarint();
            this.Slot = buffer.ReadI16();
            this.MouseButton = buffer.ReadI8();
            this.Mode = buffer.ReadVarint();
            this.ChangedSlots = buffer.ReadArray<ChangedSlotsContainer>(buffer.ReadVarint());
            this.CursorItem = buffer.ReadSlot();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)WindowId!);
            buffer.WriteVarint((int)StateId!);
            buffer.WriteI16((short)Slot!);
            buffer.WriteI8((sbyte)MouseButton!);
            buffer.WriteVarint((int)Mode!);
            buffer.WriteArray((ChangedSlotsContainer[])ChangedSlots!);
            buffer.WriteSlot((Slot)CursorItem!);
        }

        public class ChangedSlotsContainer : IPacketPayload {
            public short? Location { get; set; }
            public Slot? Item { get; set; }

            public ChangedSlotsContainer() { }

            public void Read(PacketBuffer buffer) {
                this.Location = buffer.ReadI16();
                this.Item = buffer.ReadSlot();
            }

            public void Write(PacketBuffer buffer) {
                buffer.WriteI16((short)Location!);
                buffer.WriteSlot((Slot)Item!);
            }


        }

    }



    public class CloseWindowPacket : Packet {
        public const int ProtocolId = 0x09;

        public byte? WindowId { get; set; }

        public CloseWindowPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadU8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteU8((byte)WindowId!);
        }

    }



    public class CustomPayloadPacket : Packet {
        public const int ProtocolId = 0x0a;

        public string? Channel { get; set; }
        public byte[]? Data { get; set; }

        public CustomPayloadPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Channel = buffer.ReadString();
            this.Data = buffer.ReadRestbuffer();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)Channel!);
            buffer.WriteRestbuffer((byte[])Data!);
        }

    }



    public class UseEntityPacket : Packet {
        public const int ProtocolId = 0x0d;

        public int? Target { get; set; }
        public int? Mouse { get; set; }
        public XSwitch? X { get; set; }
        public YSwitch? Y { get; set; }
        public ZSwitch? Z { get; set; }
        public HandSwitch? Hand { get; set; }
        public bool? Sneaking { get; set; }

        public UseEntityPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Target = buffer.ReadVarint();
            this.Mouse = buffer.ReadVarint();
            this.X = buffer.Read<XSwitch>(new XSwitch((int)Mouse!));
            this.Y = buffer.Read<YSwitch>(new YSwitch((int)Mouse!));
            this.Z = buffer.Read<ZSwitch>(new ZSwitch((int)Mouse!));
            this.Hand = buffer.Read<HandSwitch>(new HandSwitch((int)Mouse!));
            this.Sneaking = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Target!);
            buffer.WriteVarint((int)Mouse!);
            buffer.Write((XSwitch)X!);
            buffer.Write((YSwitch)Y!);
            buffer.Write((ZSwitch)Z!);
            buffer.Write((HandSwitch)Hand!);
            buffer.WriteBool((bool)Sneaking!);
        }

        public class XSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public XSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public XSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    2 => buffer.ReadF32(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 2: buffer.WriteF32((float)this.Value!); break;

                }

            }


        }

        public class YSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public YSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public YSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    2 => buffer.ReadF32(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 2: buffer.WriteF32((float)this.Value!); break;

                }

            }


        }

        public class ZSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public ZSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public ZSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    2 => buffer.ReadF32(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 2: buffer.WriteF32((float)this.Value!); break;

                }

            }


        }

        public class HandSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public HandSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public HandSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadVarint(),
                    2 => buffer.ReadVarint(),
                    _ => buffer.ReadVoid()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteVarint((int)this.Value!); break;
                    case 2: buffer.WriteVarint((int)this.Value!); break;

                }

            }


        }

    }



    public class GenerateStructurePacket : Packet {
        public const int ProtocolId = 0x0e;

        public Position? Location { get; set; }
        public int? Levels { get; set; }
        public bool? KeepJigsaws { get; set; }

        public GenerateStructurePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Levels = buffer.ReadVarint();
            this.KeepJigsaws = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteVarint((int)Levels!);
            buffer.WriteBool((bool)KeepJigsaws!);
        }

    }



    public class KeepAlivePacket : Packet {
        public const int ProtocolId = 0x0f;

        public long? KeepAliveId { get; set; }

        public KeepAlivePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.KeepAliveId = buffer.ReadI64();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI64((long)KeepAliveId!);
        }

    }



    public class LockDifficultyPacket : Packet {
        public const int ProtocolId = 0x10;

        public bool? Locked { get; set; }

        public LockDifficultyPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Locked = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBool((bool)Locked!);
        }

    }



    public class PositionPacket : Packet {
        public const int ProtocolId = 0x11;

        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public bool? OnGround { get; set; }

        public PositionPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.OnGround = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteBool((bool)OnGround!);
        }

    }



    public class PositionLookPacket : Packet {
        public const int ProtocolId = 0x12;

        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public float? Yaw { get; set; }
        public float? Pitch { get; set; }
        public bool? OnGround { get; set; }

        public PositionLookPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.Yaw = buffer.ReadF32();
            this.Pitch = buffer.ReadF32();
            this.OnGround = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteF32((float)Yaw!);
            buffer.WriteF32((float)Pitch!);
            buffer.WriteBool((bool)OnGround!);
        }

    }



    public class LookPacket : Packet {
        public const int ProtocolId = 0x13;

        public float? Yaw { get; set; }
        public float? Pitch { get; set; }
        public bool? OnGround { get; set; }

        public LookPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Yaw = buffer.ReadF32();
            this.Pitch = buffer.ReadF32();
            this.OnGround = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF32((float)Yaw!);
            buffer.WriteF32((float)Pitch!);
            buffer.WriteBool((bool)OnGround!);
        }

    }



    public class FlyingPacket : Packet {
        public const int ProtocolId = 0x14;

        public bool? OnGround { get; set; }

        public FlyingPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.OnGround = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBool((bool)OnGround!);
        }

    }



    public class VehicleMovePacket : Packet {
        public const int ProtocolId = 0x15;

        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public float? Yaw { get; set; }
        public float? Pitch { get; set; }

        public VehicleMovePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadF64();
            this.Y = buffer.ReadF64();
            this.Z = buffer.ReadF64();
            this.Yaw = buffer.ReadF32();
            this.Pitch = buffer.ReadF32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF64((double)X!);
            buffer.WriteF64((double)Y!);
            buffer.WriteF64((double)Z!);
            buffer.WriteF32((float)Yaw!);
            buffer.WriteF32((float)Pitch!);
        }

    }



    public class SteerBoatPacket : Packet {
        public const int ProtocolId = 0x16;

        public bool? LeftPaddle { get; set; }
        public bool? RightPaddle { get; set; }

        public SteerBoatPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.LeftPaddle = buffer.ReadBool();
            this.RightPaddle = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBool((bool)LeftPaddle!);
            buffer.WriteBool((bool)RightPaddle!);
        }

    }



    public class CraftRecipeRequestPacket : Packet {
        public const int ProtocolId = 0x18;

        public sbyte? WindowId { get; set; }
        public string? Recipe { get; set; }
        public bool? MakeAll { get; set; }

        public CraftRecipeRequestPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.WindowId = buffer.ReadI8();
            this.Recipe = buffer.ReadString();
            this.MakeAll = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)WindowId!);
            buffer.WriteString((string)Recipe!);
            buffer.WriteBool((bool)MakeAll!);
        }

    }



    public class AbilitiesPacket : Packet {
        public const int ProtocolId = 0x19;

        public sbyte? Flags { get; set; }

        public AbilitiesPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Flags = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)Flags!);
        }

    }



    public class BlockDigPacket : Packet {
        public const int ProtocolId = 0x1a;

        public sbyte? Status { get; set; }
        public Position? Location { get; set; }
        public sbyte? Face { get; set; }

        public BlockDigPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Status = buffer.ReadI8();
            this.Location = buffer.ReadPosition();
            this.Face = buffer.ReadI8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI8((sbyte)Status!);
            buffer.WritePosition((Position)Location!);
            buffer.WriteI8((sbyte)Face!);
        }

    }



    public class EntityActionPacket : Packet {
        public const int ProtocolId = 0x1b;

        public int? EntityId { get; set; }
        public int? ActionId { get; set; }
        public int? JumpBoost { get; set; }

        public EntityActionPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarint();
            this.ActionId = buffer.ReadVarint();
            this.JumpBoost = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)EntityId!);
            buffer.WriteVarint((int)ActionId!);
            buffer.WriteVarint((int)JumpBoost!);
        }

    }



    public class SteerVehiclePacket : Packet {
        public const int ProtocolId = 0x1c;

        public float? Sideways { get; set; }
        public float? Forward { get; set; }
        public byte? Jump { get; set; }

        public SteerVehiclePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Sideways = buffer.ReadF32();
            this.Forward = buffer.ReadF32();
            this.Jump = buffer.ReadU8();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteF32((float)Sideways!);
            buffer.WriteF32((float)Forward!);
            buffer.WriteU8((byte)Jump!);
        }

    }



    public class DisplayedRecipePacket : Packet {
        public const int ProtocolId = 0x1f;

        public string? RecipeId { get; set; }

        public DisplayedRecipePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.RecipeId = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString((string)RecipeId!);
        }

    }



    public class RecipeBookPacket : Packet {
        public const int ProtocolId = 0x1e;

        public int? BookId { get; set; }
        public bool? BookOpen { get; set; }
        public bool? FilterActive { get; set; }

        public RecipeBookPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.BookId = buffer.ReadVarint();
            this.BookOpen = buffer.ReadBool();
            this.FilterActive = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)BookId!);
            buffer.WriteBool((bool)BookOpen!);
            buffer.WriteBool((bool)FilterActive!);
        }

    }



    public class ResourcePackReceivePacket : Packet {
        public const int ProtocolId = 0x21;

        public int? Result { get; set; }

        public ResourcePackReceivePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Result = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Result!);
        }

    }



    public class HeldItemSlotPacket : Packet {
        public const int ProtocolId = 0x25;

        public short? SlotId { get; set; }

        public HeldItemSlotPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.SlotId = buffer.ReadI16();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI16((short)SlotId!);
        }

    }



    public class SetCreativeSlotPacket : Packet {
        public const int ProtocolId = 0x28;

        public short? Slot { get; set; }
        public Slot? Item { get; set; }

        public SetCreativeSlotPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Slot = buffer.ReadI16();
            this.Item = buffer.ReadSlot();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI16((short)Slot!);
            buffer.WriteSlot((Slot)Item!);
        }

    }



    public class UpdateJigsawBlockPacket : Packet {
        public const int ProtocolId = 0x29;

        public Position? Location { get; set; }
        public string? Name { get; set; }
        public string? Target { get; set; }
        public string? Pool { get; set; }
        public string? FinalState { get; set; }
        public string? JointType { get; set; }

        public UpdateJigsawBlockPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Name = buffer.ReadString();
            this.Target = buffer.ReadString();
            this.Pool = buffer.ReadString();
            this.FinalState = buffer.ReadString();
            this.JointType = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteString((string)Name!);
            buffer.WriteString((string)Target!);
            buffer.WriteString((string)Pool!);
            buffer.WriteString((string)FinalState!);
            buffer.WriteString((string)JointType!);
        }

    }



    public class UpdateSignPacket : Packet {
        public const int ProtocolId = 0x2b;

        public Position? Location { get; set; }
        public string? Text1 { get; set; }
        public string? Text2 { get; set; }
        public string? Text3 { get; set; }
        public string? Text4 { get; set; }

        public UpdateSignPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Text1 = buffer.ReadString();
            this.Text2 = buffer.ReadString();
            this.Text3 = buffer.ReadString();
            this.Text4 = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition((Position)Location!);
            buffer.WriteString((string)Text1!);
            buffer.WriteString((string)Text2!);
            buffer.WriteString((string)Text3!);
            buffer.WriteString((string)Text4!);
        }

    }



    public class ArmAnimationPacket : Packet {
        public const int ProtocolId = 0x2c;

        public int? Hand { get; set; }

        public ArmAnimationPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Hand = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Hand!);
        }

    }



    public class SpectatePacket : Packet {
        public const int ProtocolId = 0x2d;

        public UUID? Target { get; set; }

        public SpectatePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Target = buffer.ReadUUID();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteUUID((UUID)Target!);
        }

    }



    public class BlockPlacePacket : Packet {
        public const int ProtocolId = 0x2e;

        public int? Hand { get; set; }
        public Position? Location { get; set; }
        public int? Direction { get; set; }
        public float? CursorX { get; set; }
        public float? CursorY { get; set; }
        public float? CursorZ { get; set; }
        public bool? InsideBlock { get; set; }

        public BlockPlacePacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Hand = buffer.ReadVarint();
            this.Location = buffer.ReadPosition();
            this.Direction = buffer.ReadVarint();
            this.CursorX = buffer.ReadF32();
            this.CursorY = buffer.ReadF32();
            this.CursorZ = buffer.ReadF32();
            this.InsideBlock = buffer.ReadBool();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Hand!);
            buffer.WritePosition((Position)Location!);
            buffer.WriteVarint((int)Direction!);
            buffer.WriteF32((float)CursorX!);
            buffer.WriteF32((float)CursorY!);
            buffer.WriteF32((float)CursorZ!);
            buffer.WriteBool((bool)InsideBlock!);
        }

    }



    public class UseItemPacket : Packet {
        public const int ProtocolId = 0x2f;

        public int? Hand { get; set; }

        public UseItemPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Hand = buffer.ReadVarint();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Hand!);
        }

    }



    public class AdvancementTabPacket : Packet {
        public const int ProtocolId = 0x22;

        public int? Action { get; set; }
        public TabIdSwitch? TabId { get; set; }

        public AdvancementTabPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Action = buffer.ReadVarint();
            this.TabId = buffer.Read<TabIdSwitch>(new TabIdSwitch((int)Action!));
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarint((int)Action!);
            buffer.Write((TabIdSwitch)TabId!);
        }

        public class TabIdSwitch : IPacketPayload {

            public object? Value { get; set; }
            public int SwitchState { get; set; }

            public TabIdSwitch(object? value, int switchState) {
                this.Value = value;
                this.SwitchState = switchState;
            }

            public TabIdSwitch(int switchState) {
                this.SwitchState = switchState;
            }

            public void Read(PacketBuffer buffer) {

                this.Value = SwitchState switch {
                    0 => buffer.ReadString(),
                    1 => buffer.ReadVoid(),
                    _ => throw new Exception()
                };

            }

            public void Write(PacketBuffer buffer) {
                switch (SwitchState) {
                    case 0: buffer.WriteString((string)this.Value!); break;
                    case 1: buffer.WriteVoid((object)this.Value!); break;

                }

            }


        }

    }



    public class PongPacket : Packet {
        public const int ProtocolId = 0x1d;

        public int? Id { get; set; }

        public PongPacket() : base(ProtocolId) { }

        public override void Read(PacketBuffer buffer) {
            this.Id = buffer.ReadI32();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteI32((int)Id!);
        }

    }


}

