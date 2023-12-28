using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;
#pragma warning disable CS1591
/// <summary>
/// Plugin message packet
/// </summary>
public class PluginMessagePacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_CustomPayload;

    /// <summary>
    /// The name of the channel the data was sent
    /// </summary>
    public string ChannelName { get; set; }
    
    /// <summary>
    /// The message data
    /// </summary>
    public PacketBuffer Data { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="channelName"></param>
    /// <param name="data"></param>
    public PluginMessagePacket(string channelName, PacketBuffer data)
    {
        this.ChannelName = channelName;
        this.Data = data;
    }
    
    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.ChannelName);
        buffer.WriteBytes(this.Data.GetBuffer());
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var channelName = buffer.ReadString();
        var clone = new PacketBuffer(buffer.ReadBytes((int)buffer.ReadableBytes));
        return new PluginMessagePacket(channelName, clone);
    }
}
#pragma warning restore CS1591