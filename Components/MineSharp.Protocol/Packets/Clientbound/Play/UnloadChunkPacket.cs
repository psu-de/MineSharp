using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class UnloadChunkPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_UnloadChunk;

    public int X { get; set; }
    public int Z { get; set; }

    public UnloadChunkPacket(int x, int z)
    {
        this.X = x;
        this.Z = z;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt(this.X);
        buffer.WriteInt(this.Z);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x = buffer.ReadInt();
        var z = buffer.ReadInt();
        return new UnloadChunkPacket(x, z);
    }
}
#pragma warning restore CS1591