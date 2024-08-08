using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Clientbound Plugin Message packet
/// </summary>
/// <param name="Channel">Name of the plugin channel used to send the data</param>
/// <param name="Data">Any data. The length of this array must be inferred from the packet length</param>
public sealed record PluginMessagePacket(Identifier Channel, byte[] Data) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_CustomPayload;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteIdentifier(Channel);
        buffer.WriteBytes(Data);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var channel = buffer.ReadObject<Identifier>();
        var data = buffer.RestBuffer();

        return new PluginMessagePacket(channel, data);
    }
}
