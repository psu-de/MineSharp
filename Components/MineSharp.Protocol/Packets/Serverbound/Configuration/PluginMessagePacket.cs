using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;

/// <summary>
///     Plugin message packet
/// </summary>
/// <param name="Channel">The name of the channel</param>
/// <param name="Data">The data of the plugin message</param>
public sealed partial record PluginMessagePacket(Identifier Channel, byte[] Data) : IPacketStatic<PluginMessagePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Configuration_CustomPayload;

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
        var pluginData = buffer.RestBuffer();

        return new PluginMessagePacket(channel, pluginData);
    }
}
