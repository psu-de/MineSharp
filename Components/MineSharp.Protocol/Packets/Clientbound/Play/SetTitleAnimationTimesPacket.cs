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
public sealed record SetTitleAnimationTimesPacket(int FadeIn, int Stay, int FadeOut) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SetTitleTime;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt(FadeIn);
        buffer.WriteInt(Stay);
        buffer.WriteInt(FadeOut);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var fadeIn = buffer.ReadInt();
        var stay = buffer.ReadInt();
        var fadeOut = buffer.ReadInt();

        return new SetTitleAnimationTimesPacket(fadeIn, stay, fadeOut);
    }
}
