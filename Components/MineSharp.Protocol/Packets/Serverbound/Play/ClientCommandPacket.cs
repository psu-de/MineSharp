using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class ClientCommandPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_ClientCommand;

    public int ActionId { get; set; }

    public ClientCommandPacket(int actionId)
    {
        this.ActionId = actionId;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.ActionId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var actionId = buffer.ReadVarInt();
        return new ClientCommandPacket(actionId);
    }
}
#pragma warning restore CS1591
