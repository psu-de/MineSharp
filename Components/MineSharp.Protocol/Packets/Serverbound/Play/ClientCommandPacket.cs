using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class ClientCommandPacket : IPacket
{
    public ClientCommandPacket(int actionId)
    {
        ActionId = actionId;
    }

    public int ActionId { get; set; }
    public PacketType Type => PacketType.SB_Play_ClientCommand;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(ActionId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var actionId = buffer.ReadVarInt();
        return new ClientCommandPacket(actionId);
    }
}
#pragma warning restore CS1591
