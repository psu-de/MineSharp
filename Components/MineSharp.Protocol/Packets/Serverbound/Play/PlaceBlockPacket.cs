using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class PlaceBlockPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_BlockPlace;

    public int       Hand        { get; set; }
    public Position  Location    { get; set; }
    public BlockFace Direction   { get; set; }
    public float     CursorX     { get; set; }
    public float     CursorY     { get; set; }
    public float     CursorZ     { get; set; }
    public bool      InsideBlock { get; set; }
    public int?      SequenceId  { get; set; }

    /// <summary>
    /// Constructor >= 1.19
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="location"></param>
    /// <param name="direction"></param>
    /// <param name="cursorX"></param>
    /// <param name="cursorY"></param>
    /// <param name="cursorZ"></param>
    /// <param name="insideBlock"></param>
    /// <param name="sequenceId"></param>
    public PlaceBlockPacket(int hand, Position location, BlockFace direction, float cursorX, float cursorY, float cursorZ, bool insideBlock,
                            int sequenceId)
    {
        this.Hand        = hand;
        this.Location    = location;
        this.Direction   = direction;
        this.CursorX     = cursorX;
        this.CursorY     = cursorY;
        this.CursorZ     = cursorZ;
        this.InsideBlock = insideBlock;
        this.SequenceId  = sequenceId;
    }

    /// <summary>
    /// Constructor for versions before 1.19
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="location"></param>
    /// <param name="direction"></param>
    /// <param name="cursorX"></param>
    /// <param name="cursorY"></param>
    /// <param name="cursorZ"></param>
    /// <param name="insideBlock"></param>
    public PlaceBlockPacket(int hand, Position location, BlockFace direction, float cursorX, float cursorY, float cursorZ, bool insideBlock)
    {
        this.Hand        = hand;
        this.Location    = location;
        this.Direction   = direction;
        this.CursorX     = cursorX;
        this.CursorY     = cursorY;
        this.CursorZ     = cursorZ;
        this.InsideBlock = insideBlock;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.Hand);
        buffer.WriteULong(this.Location.ToULong());
        buffer.WriteVarInt((int)this.Direction);
        buffer.WriteFloat(this.CursorX);
        buffer.WriteFloat(this.CursorY);
        buffer.WriteFloat(this.CursorZ);
        buffer.WriteBool(this.InsideBlock);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            buffer.WriteVarInt(this.SequenceId!.Value);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var hand        = buffer.ReadVarInt();
        var position    = new Position(buffer.ReadULong());
        var direction   = buffer.ReadVarInt();
        var cursorX     = buffer.ReadFloat();
        var cursorY     = buffer.ReadFloat();
        var cursorZ     = buffer.ReadFloat();
        var insideBlock = buffer.ReadBool();

        if (version.Version.Protocol < ProtocolVersion.V_1_19)
            return new PlaceBlockPacket(hand, position, (BlockFace)direction, cursorX, cursorY, cursorZ, insideBlock);

        var sequenceId = buffer.ReadVarInt();
        return new PlaceBlockPacket(
            hand,
            position,
            (BlockFace)direction,
            cursorX,
            cursorY,
            cursorZ,
            insideBlock,
            sequenceId);
    }
}
#pragma warning restore CS1591
