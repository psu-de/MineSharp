using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Set Compression packet
/// </summary>
public class SetCompressionPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Login_Compress;
    
    /// <summary>
    ///     Threshold for when to use compression
    /// </summary>
    public required int Threshold { get; init; }
    
    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(Threshold);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var threshold = buffer.ReadVarInt();
        return new SetCompressionPacket() { Threshold = threshold };
    }
}
