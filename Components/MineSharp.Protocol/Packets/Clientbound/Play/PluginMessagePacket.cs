using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Clientbound Plugin Message packet
/// </summary>
/// <param name="Channel">Name of the plugin channel used to send the data</param>
/// <param name="Data">Any data, depending on the channel</param>
public sealed record PluginMessagePacket(Identifier Channel, byte[] Data) : IPacketStatic<PluginMessagePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_CustomPayload;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteIdentifier(Channel);
        buffer.WriteBytes(Data);
    }

    /// <inheritdoc />
    public static PluginMessagePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var channel = buffer.ReadObject<Identifier>();
        var pluginData = buffer.RestBuffer();

        return new PluginMessagePacket(channel, pluginData);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
