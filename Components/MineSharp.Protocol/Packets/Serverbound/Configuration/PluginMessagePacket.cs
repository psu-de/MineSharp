using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
#pragma warning disable CS1591
public class PluginMessagePacket : IPacket
{
    public PluginMessagePacket(string channelName, PacketBuffer data)
    {
        ChannelName = channelName;
        Data = data;
    }

    public string ChannelName { get; set; }
    public PacketBuffer Data { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Configuration_CustomPayload;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(ChannelName);
        buffer.WriteBytes(Data.GetBuffer());
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var channelName = buffer.ReadString();
        var clone = new PacketBuffer(buffer.ReadBytes((int)buffer.ReadableBytes), version.Version.Protocol);
        return new PluginMessagePacket(channelName, clone);
    }
}
#pragma warning restore CS1591
