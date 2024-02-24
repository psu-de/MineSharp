using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Login;
#pragma warning disable CS1591
/// <summary>
/// Set Compression packet
/// </summary>
public class SetCompressionPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_Compress;

    /// <summary>
    /// Threshold for when to use compression
    /// </summary>
    public int Threshold { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="threshold"></param>
    public SetCompressionPacket(int threshold)
    {
        this.Threshold = threshold;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.Threshold);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var threshold = buffer.ReadVarInt();
        return new SetCompressionPacket(threshold);
    }
}
#pragma warning restore CS1591
