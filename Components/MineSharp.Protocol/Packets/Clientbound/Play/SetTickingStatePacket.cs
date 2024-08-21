using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet used to adjust the ticking rate of the client, and whether it's frozen.
/// </summary>
/// <param name="TickRate">The tick rate of the client</param>
/// <param name="IsFrozen">Whether the client is frozen</param>
public sealed partial record SetTickingStatePacket(float TickRate, bool IsFrozen) : IPacketStatic<SetTickingStatePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SetTickingState;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(TickRate);
        buffer.WriteBool(IsFrozen);
    }

    /// <inheritdoc />
    public static SetTickingStatePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var tickRate = buffer.ReadFloat();
        var isFrozen = buffer.ReadBool();

        return new SetTickingStatePacket(tickRate, isFrozen);
    }
}
