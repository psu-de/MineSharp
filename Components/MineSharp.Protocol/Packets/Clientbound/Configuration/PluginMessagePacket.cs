using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Plugin message packet
/// </summary>
/// <param name="ChannelName">The name of the channel the data was sent</param>
/// <param name="Data">The message data</param>
public sealed record PluginMessagePacket(Identifier ChannelName, byte[] Data) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_CustomPayload;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteIdentifier(ChannelName);
        buffer.WriteBytes(Data);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var channelName = buffer.ReadIdentifier();
        var data = buffer.RestBuffer();
        return new PluginMessagePacket(channelName, data);
    }
}

