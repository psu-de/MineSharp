using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SetHealthPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_UpdateHealth;

    public float Health { get; set; }
    public int Food { get; set; }
    public float Saturation { get; set; }

    public SetHealthPacket(float health, int food, float saturation)
    {
        this.Health = health;
        this.Food = food;
        this.Saturation = saturation;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteFloat(this.Health);
        buffer.WriteVarInt(this.Food);
        buffer.WriteFloat(this.Saturation);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var health = buffer.ReadFloat();
        var food = buffer.ReadVarInt();
        var saturation = buffer.ReadFloat();
        return new SetHealthPacket(health, food, saturation);
    }
}
#pragma warning restore CS1591