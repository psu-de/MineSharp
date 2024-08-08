using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Set Compression packet
/// </summary>
/// <param name="Threshold">Threshold for when to use compression</param>
public sealed record SetCompressionPacket(int Threshold) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Login_Compress;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(Threshold);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var threshold = buffer.ReadVarInt();
        return new SetCompressionPacket(threshold);
    }
}
