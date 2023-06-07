using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class SetHealthPacket : IPacket
{
    public static int Id => 0x57;

    public float Health { get; set; }
    public int Food { get; set; }
    public float Saturation { get; set; }

    public SetHealthPacket(float health, int food, float saturation)
    {
        this.Health = health;
        this.Food = food;
        this.Saturation = saturation;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteFloat(this.Health);
        buffer.WriteVarInt(this.Food);
        buffer.WriteFloat(this.Saturation);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var health = buffer.ReadFloat();
        var food = buffer.ReadVarInt();
        var saturation = buffer.ReadFloat();
        return new SetHealthPacket(health, food, saturation);
    }
}
