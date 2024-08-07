using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class UnloadChunkPacket : IPacket
{
    public UnloadChunkPacket(int x, int z)
    {
        X = x;
        Z = z;
    }

    public int X { get; set; }
    public int Z { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_UnloadChunk;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt(X);
        buffer.WriteInt(Z);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x = buffer.ReadInt();
        var z = buffer.ReadInt();
        return new UnloadChunkPacket(x, z);
    }
}
#pragma warning restore CS1591
