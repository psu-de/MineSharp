using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

public class ClientCommandPacket : IPacket
{
    public static int Id => 0x07;

    public int ActionId { get; set; }

    public ClientCommandPacket(int actionId)
    {
        this.ActionId = actionId;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarInt(this.ActionId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var actionId = buffer.ReadVarInt();
        return new ClientCommandPacket(actionId);
    }
}
