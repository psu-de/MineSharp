using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class UpdateCommandBlock : IPacket
{
    public UpdateCommandBlock(Position location, string command, int mode, byte flags)
    {
        Location = location;
        Command = command;
        Mode = mode;
        Flags = flags;
    }

    public Position Location { get; set; }
    public string Command { get; set; }
    public int Mode { get; set; }
    public byte Flags { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Play_UpdateCommandBlock;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteULong(Location.ToULong());
        buffer.WriteString(Command);
        buffer.WriteVarInt(Mode);
        buffer.WriteByte(Flags);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new UpdateCommandBlock(
            new(buffer.ReadULong()),
            buffer.ReadString(),
            buffer.ReadVarInt(),
            buffer.ReadByte());
    }
}

#pragma warning restore CS1591
