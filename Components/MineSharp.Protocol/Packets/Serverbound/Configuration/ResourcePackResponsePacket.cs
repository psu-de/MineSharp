using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
#pragma warning disable CS1591
public class ResourcePackResponsePacket : IPacket
{
    public PacketType Type => PacketType.SB_Configuration_ResourcePackReceive;

    public int Result { get; set; }

    public ResourcePackResponsePacket(int result)
    {
        this.Result = result;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.Result);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ResourcePackResponsePacket(
            buffer.ReadVarInt());
    }
}
#pragma warning restore CS1591