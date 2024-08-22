using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Update Time packet
///     
/// Time is based on ticks, where 20 ticks happen every second. There are 24000 ticks in a day, making Minecraft days exactly 20 minutes long.
/// The time of day is based on the timestamp modulo 24000. 0 is sunrise, 6000 is noon, 12000 is sunset, and 18000 is midnight.
/// The default SMP server increments the time by 20 every second.
/// </summary>
/// <param name="WorldAge">The world age in ticks</param>
/// <param name="TimeOfDay">The time of day in ticks</param>
public sealed partial record UpdateTimePacket(long WorldAge, long TimeOfDay) : IPacketStatic<UpdateTimePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_UpdateTime;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteLong(WorldAge);
        buffer.WriteLong(TimeOfDay);
    }

    /// <inheritdoc />
    public static UpdateTimePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var worldAge = buffer.ReadLong();
        var timeOfDay = buffer.ReadLong();

        return new UpdateTimePacket(worldAge, timeOfDay);
    }
}
