using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed partial record PlaceBlockPacket : IPacketStatic<PlaceBlockPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_BlockPlace;

    // Here is no non-argument constructor allowed
    // Do not use
#pragma warning disable CS8618
    private PlaceBlockPacket()
#pragma warning restore CS8618
    {
    }

    /// <summary>
    ///     Constructor >= 1.19
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="location"></param>
    /// <param name="direction"></param>
    /// <param name="cursorX"></param>
    /// <param name="cursorY"></param>
    /// <param name="cursorZ"></param>
    /// <param name="insideBlock"></param>
    /// <param name="sequenceId"></param>
    public PlaceBlockPacket(PlayerHand hand, Position location, BlockFace direction, float cursorX, float cursorY,
                            float cursorZ, bool insideBlock,
                            int sequenceId)
    {
        Hand = hand;
        Location = location;
        Direction = direction;
        CursorX = cursorX;
        CursorY = cursorY;
        CursorZ = cursorZ;
        InsideBlock = insideBlock;
        SequenceId = sequenceId;
    }

    /// <summary>
    ///     Constructor for versions before 1.19
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="location"></param>
    /// <param name="direction"></param>
    /// <param name="cursorX"></param>
    /// <param name="cursorY"></param>
    /// <param name="cursorZ"></param>
    /// <param name="insideBlock"></param>
    public PlaceBlockPacket(PlayerHand hand, Position location, BlockFace direction, float cursorX, float cursorY,
                            float cursorZ, bool insideBlock)
    {
        Hand = hand;
        Location = location;
        Direction = direction;
        CursorX = cursorX;
        CursorY = cursorY;
        CursorZ = cursorZ;
        InsideBlock = insideBlock;
    }

    public PlayerHand Hand { get; init; }
    public Position Location { get; init; }
    public BlockFace Direction { get; init; }
    public float CursorX { get; init; }
    public float CursorY { get; init; }
    public float CursorZ { get; init; }
    public bool InsideBlock { get; init; }
    public int? SequenceId { get; init; }

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt((int)Hand);
        buffer.WritePosition(Location);
        buffer.WriteVarInt((int)Direction);
        buffer.WriteFloat(CursorX);
        buffer.WriteFloat(CursorY);
        buffer.WriteFloat(CursorZ);
        buffer.WriteBool(InsideBlock);

        if (data.Version.Protocol >= ProtocolVersion.V_1_19_0)
        {
            buffer.WriteVarInt(SequenceId!.Value);
        }
    }

    public static PlaceBlockPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var hand = (PlayerHand)buffer.ReadVarInt();
        var position = buffer.ReadPosition();
        var direction = buffer.ReadVarInt();
        var cursorX = buffer.ReadFloat();
        var cursorY = buffer.ReadFloat();
        var cursorZ = buffer.ReadFloat();
        var insideBlock = buffer.ReadBool();

        if (data.Version.Protocol < ProtocolVersion.V_1_19_0)
        {
            return new PlaceBlockPacket(hand, position, (BlockFace)direction, cursorX, cursorY, cursorZ, insideBlock);
        }

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
