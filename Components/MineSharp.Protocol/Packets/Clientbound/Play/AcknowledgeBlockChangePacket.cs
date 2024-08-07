using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     Acknowledge block change packet
/// </summary>
public class AcknowledgeBlockChangePacket : IPacket
{
    /// <summary>
    ///     Constructor for version >= 1.19
    /// </summary>
    /// <param name="sequenceId"></param>
    public AcknowledgeBlockChangePacket(int sequenceId)
    {
        Body = new PacketBody119(sequenceId);
    }

    /// <summary>
    ///     Constructor for versions &lt; 1.19
    /// </summary>
    /// <param name="location"></param>
    /// <param name="block"></param>
    /// <param name="status"></param>
    /// <param name="successful"></param>
    public AcknowledgeBlockChangePacket(Position location, int block, int status, bool successful)
    {
        Body = new PacketBody118(location, block, status, successful);
    }

    internal AcknowledgeBlockChangePacket(IPacketBody body)
    {
        Body = body;
    }

    /// <summary>
    ///     The body of this packet.
    ///     Different minecraft versions use different packet bodies.
    /// </summary>
    public IPacketBody Body { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_AcknowledgePlayerDigging;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        Body.Write(buffer);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol < ProtocolVersion.V_1_19)
        {
            return new AcknowledgeBlockChangePacket(PacketBody118.Read(buffer));
        }

        return new AcknowledgeBlockChangePacket(PacketBody119.Read(buffer));
    }


    /// <summary>
    ///     Packet body of <see cref="AcknowledgeBlockChangePacket" />
    /// </summary>
    public interface IPacketBody
    {
        /// <summary>
        ///     Serialize the body to the buffer
        /// </summary>
        /// <param name="buffer"></param>
        void Write(PacketBuffer buffer);

        /// <summary>
        ///     Deserialize the body from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static abstract IPacketBody Read(PacketBuffer buffer);
    }

    /// <summary>
    ///     Acknowledge block change packet for &lt; 1.19
    /// </summary>
    public class PacketBody118 : IPacketBody
    {
        /// <summary>
        ///     Create a new instance
        /// </summary>
        /// <param name="location"></param>
        /// <param name="block"></param>
        /// <param name="status"></param>
        /// <param name="successful"></param>
        public PacketBody118(Position location, int block, int status, bool successful)
        {
            Location = location;
            Block = block;
            Status = status;
            Successful = successful;
        }

        /// <summary>
        ///     The Position of the block
        /// </summary>
        public Position Location { get; set; }

        /// <summary>
        ///     Block state
        /// </summary>
        public int Block { get; set; }

        /// <summary>
        ///     Status of the block change
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        ///     Whether the block change was successful
        /// </summary>
        public bool Successful { get; set; }

        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteULong(Location.ToULong());
            buffer.WriteVarInt(Block);
            buffer.WriteVarInt(Status);
            buffer.WriteBool(Successful);
        }

        /// <inheritdoc />
        public static IPacketBody Read(PacketBuffer buffer)
        {
            return new PacketBody118(
                new(buffer.ReadULong()),
                buffer.ReadVarInt(),
                buffer.ReadVarInt(),
                buffer.ReadBool());
        }
    }

    /// <summary>
    ///     Acknowledge block change packet body for versions &gt;= 1.19
    /// </summary>
    public class PacketBody119 : IPacketBody
    {
        /// <summary>
        ///     Create a new instance
        /// </summary>
        /// <param name="sequenceId"></param>
        public PacketBody119(int sequenceId)
        {
            SequenceId = sequenceId;
        }

        /// <summary>
        ///     Sequence id used for synchronization
        /// </summary>
        public int SequenceId { get; set; }

        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt(SequenceId);
        }

        /// <inheritdoc />
        public static IPacketBody Read(PacketBuffer buffer)
        {
            return new PacketBody119(buffer.ReadVarInt());
        }
    }
}
#pragma warning restore CS1591
