using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
/// Acknowledge block change packet
/// </summary>
public class AcknowledgeBlockChangePacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Play_AcknowledgePlayerDigging;

    /// <summary>
    /// The body of this packet.
    /// Different minecraft versions use different packet bodies.
    /// </summary>
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
    /// Constructor for versions &lt; 1.19
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

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        this.Body.Write(buffer);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol < ProtocolVersion.V_1_19)
            return new AcknowledgeBlockChangePacket(PacketBody_1_18.Read(buffer));

        return new AcknowledgeBlockChangePacket(PacketBody_1_19.Read(buffer));
    }


    /// <summary>
    /// Packet body of <see cref="AcknowledgeBlockChangePacket"/>
    /// </summary>
    public interface IPacketBody
    {
        /// <summary>
        /// Serialize the body to the buffer
        /// </summary>
        /// <param name="buffer"></param>
        void Write(PacketBuffer buffer);

        /// <summary>
        /// Deserialize the body from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        abstract static IPacketBody Read(PacketBuffer buffer);
    }

    /// <summary>
    /// Acknowledge block change packet for &lt; 1.19
    /// </summary>
    public class PacketBody_1_18 : IPacketBody
    {
        /// <summary>
        /// The Position of the block
        /// </summary>
        public Position Location { get; set; }

        /// <summary>
        /// Block state
        /// </summary>
        public int Block { get; set; }

        /// <summary>
        /// Status of the block change
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Whether the block change was successful
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="location"></param>
        /// <param name="block"></param>
        /// <param name="status"></param>
        /// <param name="successful"></param>
        public PacketBody_1_18(Position location, int block, int status, bool successful)
        {
            this.Location   = location;
            this.Block      = block;
            this.Status     = status;
            this.Successful = successful;
        }

        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteULong(this.Location.ToULong());
            buffer.WriteVarInt(this.Block);
            buffer.WriteVarInt(this.Status);
            buffer.WriteBool(this.Successful);
        }

        /// <inheritdoc />
        public static IPacketBody Read(PacketBuffer buffer)
        {
            return new PacketBody_1_18(
                new Position(buffer.ReadULong()),
                buffer.ReadVarInt(),
                buffer.ReadVarInt(),
                buffer.ReadBool());
        }
    }

    /// <summary>
    /// Acknowledge block change packet body for versions &gt;= 1.19
    /// </summary>
    public class PacketBody_1_19 : IPacketBody
    {
        /// <summary>
        /// Sequence id used for synchronization
        /// </summary>
        public int SequenceId { get; set; }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="sequenceId"></param>
        public PacketBody_1_19(int sequenceId)
        {
            this.SequenceId = sequenceId;
        }

        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt(this.SequenceId);
        }

        /// <inheritdoc />
        public static IPacketBody Read(PacketBuffer buffer)
        {
            return new PacketBody_1_19(buffer.ReadVarInt());
        }
    }
}
#pragma warning restore CS1591
