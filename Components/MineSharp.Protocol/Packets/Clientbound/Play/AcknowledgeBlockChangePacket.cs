using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using Newtonsoft.Json;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class AcknowledgeBlockChangePacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_AcknowledgePlayerDigging;
    
    public IPacketBody Body { get; set; }

    /// <summary>
    /// Constructor for version >= 1.19
    /// </summary>
    /// <param name="sequenceId"></param>
    public AcknowledgeBlockChangePacket(int sequenceId)
    {
        this.Body = new PacketBody_1_19(sequenceId);
    }

    /// <summary>
    /// Constructor for versions < 1.19
    /// </summary>
    /// <param name="location"></param>
    /// <param name="block"></param>
    /// <param name="status"></param>
    /// <param name="successful"></param>
    public AcknowledgeBlockChangePacket(Position location, int block, int status, bool successful)
    {
        this.Body = new PacketBody_1_18(location, block, status, successful);
    }

    internal AcknowledgeBlockChangePacket(IPacketBody body)
    {
        this.Body = body;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        this.Body.Write(buffer);
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol < ProtocolVersion.V_1_19)
            return new AcknowledgeBlockChangePacket(PacketBody_1_18.Read(buffer));

        return new AcknowledgeBlockChangePacket(PacketBody_1_19.Read(buffer));
    }


    public interface IPacketBody
    {
        void Write(PacketBuffer buffer);

        abstract static IPacketBody Read(PacketBuffer buffer);
    }

    public class PacketBody_1_18 : IPacketBody
    {
        public Position Location { get; set; }
        public int Block { get; set; }
        public int Status { get; set; }
        public bool Successful { get; set; }

        public PacketBody_1_18(Position location, int block, int status, bool successful)
        {
            this.Location = location;
            this.Block = block;
            this.Status = status;
            this.Successful = successful;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteULong(this.Location.ToULong());
            buffer.WriteVarInt(this.Block);
            buffer.WriteVarInt(this.Status);
            buffer.WriteBool(this.Successful);
        }

        public static IPacketBody Read(PacketBuffer buffer)
        {
            return new PacketBody_1_18(
                new Position(buffer.ReadULong()),
                buffer.ReadVarInt(),
                buffer.ReadVarInt(),
                buffer.ReadBool());
        }
    }

    public class PacketBody_1_19 : IPacketBody
    {
        public int SequenceId { get; set; }
        
        public PacketBody_1_19(int sequenceId)
        {
            this.SequenceId = sequenceId;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt(this.SequenceId);
        }


        public static IPacketBody Read(PacketBuffer buffer)
        {
            return new PacketBody_1_19(buffer.ReadVarInt());
        }
    }
}
