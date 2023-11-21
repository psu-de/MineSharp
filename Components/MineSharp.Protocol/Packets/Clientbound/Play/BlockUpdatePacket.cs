using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class BlockUpdatePacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_BlockChange;

    public Position Location { get; set; }
    public int StateId { get; set; }

    public BlockUpdatePacket(Position location, int stateId)
    {
        this.Location = location;
        this.StateId = stateId;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteULong(this.Location.ToULong());
        buffer.WriteVarInt(this.StateId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = new Position(buffer.ReadULong());
        var stateId = buffer.ReadVarInt();
        return new BlockUpdatePacket(location, stateId);
    }
}
