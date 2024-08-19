using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Serverbound Plugin Message packet
/// </summary>
/// <param name="Channel">Name of the plugin channel used to send the data</param>
/// <param name="Data">Any data, depending on the channel</param>
public sealed record PluginMessagePacket(Identifier Channel, byte[] Data) : IPacketStatic<PluginMessagePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_CustomPayload;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteIdentifier(Channel);
        buffer.WriteBytes(Data);
    }

    /// <inheritdoc />
    public static PluginMessagePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var channel = buffer.ReadIdentifier();
        var pluginData = buffer.ReadBytes((int)buffer.ReadableBytes);

        return new(channel, pluginData);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
