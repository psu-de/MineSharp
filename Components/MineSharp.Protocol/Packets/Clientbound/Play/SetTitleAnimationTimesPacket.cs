using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the title animation times.
/// </summary>
/// <param name="FadeIn">Ticks to spend fading in.</param>
/// <param name="Stay">Ticks to keep the title displayed.</param>
/// <param name="FadeOut">Ticks to spend fading out, not when to start fading out.</param>
public sealed record SetTitleAnimationTimesPacket(int FadeIn, int Stay, int FadeOut) : IPacketStatic<SetTitleAnimationTimesPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SetTitleTime;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteInt(FadeIn);
        buffer.WriteInt(Stay);
        buffer.WriteInt(FadeOut);
    }

    /// <inheritdoc />
    public static SetTitleAnimationTimesPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var fadeIn = buffer.ReadInt();
        var stay = buffer.ReadInt();
        var fadeOut = buffer.ReadInt();

        return new SetTitleAnimationTimesPacket(fadeIn, stay, fadeOut);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
