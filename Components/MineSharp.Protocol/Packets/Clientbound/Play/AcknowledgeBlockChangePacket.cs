using MineSharp.Core;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     Acknowledge block change packet
/// </summary>
public sealed record AcknowledgeBlockChangePacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_AcknowledgePlayerDigging;

    // Here is no non-argument constructor allowed
    // Do not use
#pragma warning disable CS8618
    private AcknowledgeBlockChangePacket()
#pragma warning restore CS8618
    {
    }

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
    ///     Different Minecraft versions use different packet bodies.
    /// </summary>
    public IPacketBody Body { get; set; }

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
    /// <param name="Location">The Position of the block</param>
    /// <param name="Block">Block state</param>
    /// <param name="Status">Status of the block change</param>
    /// <param name="Successful">Whether the block change was successful</param>
    public sealed record PacketBody118(Position Location, int Block, int Status, bool Successful) : IPacketBody
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WritePosition(Location);
            buffer.WriteVarInt(Block);
            buffer.WriteVarInt(Status);
            buffer.WriteBool(Successful);
        }

        /// <inheritdoc />
        public static IPacketBody Read(PacketBuffer buffer)
        {
            return new PacketBody118(
                buffer.ReadPosition(),
                buffer.ReadVarInt(),
                buffer.ReadVarInt(),
                buffer.ReadBool());
        }
    }

    /// <summary>
    ///     Acknowledge block change packet body for versions &gt;= 1.19
    /// </summary>
    /// <param name="SequenceId">Sequence id used for synchronization</param>
    public sealed record PacketBody119(int SequenceId) : IPacketBody
    {
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
