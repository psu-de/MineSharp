using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Set Compression packet
/// </summary>
/// <param name="Threshold">Threshold for when to use compression</param>
public sealed partial record SetCompressionPacket(int Threshold) : IPacketStatic<SetCompressionPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Login_Compress;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(Threshold);
    }

    /// <inheritdoc />
    public static SetCompressionPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var threshold = buffer.ReadVarInt();
        return new SetCompressionPacket(threshold);
    }
}
