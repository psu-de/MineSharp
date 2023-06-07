using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class BlockUpdatePacket : IPacket
{
    public static int Id => 0x0A;

    public Position Location { get; set; }
    public int StateId { get; set; }

    public BlockUpdatePacket(Position location, int stateId)
    {
        this.Location = location;
        this.StateId = stateId;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteULong(this.Location.ToULong());
        buffer.WriteVarInt(this.StateId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var location = new Position(buffer.ReadULong());
        var stateId = buffer.ReadVarInt();
        return new BlockUpdatePacket(location, stateId);
    }
}
