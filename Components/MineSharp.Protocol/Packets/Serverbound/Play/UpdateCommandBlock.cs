using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record UpdateCommandBlock(Position Location, string Command, int Mode, byte Flags) : IPacketStatic<UpdateCommandBlock>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_UpdateCommandBlock;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WritePosition(Location);
        buffer.WriteString(Command);
        buffer.WriteVarInt(Mode);
        buffer.WriteByte(Flags);
    }

    /// <inheritdoc />
    public static UpdateCommandBlock Read(PacketBuffer buffer, MinecraftData data)
    {
        return new UpdateCommandBlock(
            buffer.ReadPosition(),
            buffer.ReadString(),
            buffer.ReadVarInt(),
            buffer.ReadByte());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591
