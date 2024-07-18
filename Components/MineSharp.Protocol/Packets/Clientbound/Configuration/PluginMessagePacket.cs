using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;
#pragma warning disable CS1591
/// <summary>
///     Plugin message packet
/// </summary>
public class PluginMessagePacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="channelName"></param>
    /// <param name="data"></param>
    public PluginMessagePacket(string channelName, PacketBuffer data)
    {
        ChannelName = channelName;
        Data = data;
    }

    /// <summary>
    ///     The name of the channel the data was sent
    /// </summary>
    public string ChannelName { get; set; }

    /// <summary>
    ///     The message data
    /// </summary>
    public PacketBuffer Data { get; set; }

    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_CustomPayload;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(ChannelName);
        buffer.WriteBytes(Data.GetBuffer());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var channelName = buffer.ReadString();
        var clone = new PacketBuffer(buffer.ReadBytes((int)buffer.ReadableBytes), version.Version.Protocol);
        return new PluginMessagePacket(channelName, clone);
    }
}
#pragma warning restore CS1591
