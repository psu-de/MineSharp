using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
/// Plugin message during play phase
/// See https://wiki.vg/Protocol#Serverbound_Plugin_Message_.28play.29
/// </summary>
public class PluginMessagePacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.SB_Play_CustomPayload;
    
    /// <summary>
    /// The channel of this message
    /// </summary>
    public required string ChannelName { get; init; }
    
    /// <summary>
    /// The data of this message
    /// </summary>
    public required byte[] Data { get; init; }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(ChannelName);
        buffer.WriteBytes(Data);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var channelName = buffer.ReadString();
        var data        = buffer.RestBuffer();
        
        return new PluginMessagePacket
        {
            ChannelName = channelName,
            Data        = data
        };
    }
}
