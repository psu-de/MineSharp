using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class UnloadChunkPacket : IPacket
{
    public static int Id => 0x1E;

    public int X { get; set; }
    public int Z { get; set; }

    public UnloadChunkPacket(int x, int z)
    {
        this.X = x;
        this.Z = z;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteInt(this.X);
        buffer.WriteInt(this.Z);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var x = buffer.ReadInt();
        var z = buffer.ReadInt();
        return new UnloadChunkPacket(x, z);
    }
}
