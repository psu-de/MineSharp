using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class UpdateCommandBlock : IPacket
{
    public PacketType Type => PacketType.SB_Play_UpdateCommandBlock;

    public Position Location { get; set; }
    public string Command { get; set; }
    public int Mode { get; set; }
    public byte Flags { get; set; }

    public UpdateCommandBlock(Position location, string command, int mode, byte flags)
    {
        this.Location = location;
        this.Command = command;
        this.Mode = mode;
        this.Flags = flags;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteULong(this.Location.ToULong());
        buffer.WriteString(this.Command);
        buffer.WriteVarInt(this.Mode);
        buffer.WriteByte(this.Flags);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new UpdateCommandBlock(
            new Position(buffer.ReadULong()),
            buffer.ReadString(),
            buffer.ReadVarInt(),
            buffer.ReadByte());
    }
}