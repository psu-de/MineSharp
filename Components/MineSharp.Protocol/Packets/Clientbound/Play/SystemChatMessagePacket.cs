using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SystemChatMessagePacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_SystemChat;

    public string Content   { get; set; }
    public int?   ChatType  { get; set; }
    public bool?  IsOverlay { get; set; }

    /// <summary>
    /// Constructor for 1.19.1
    /// </summary>
    /// <param name="content"></param>
    /// <param name="chatType"></param>
    public SystemChatMessagePacket(string content, int chatType)
    {
        this.Content  = content;
        this.ChatType = chatType;
    }

    /// <summary>
    /// Constructor for >= 1.19.2
    /// </summary>
    /// <param name="content"></param>
    /// <param name="isOverlay"></param>
    public SystemChatMessagePacket(string content, bool isOverlay)
    {
        this.Content   = content;
        this.IsOverlay = isOverlay;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Content);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_2)
            buffer.WriteBool(this.IsOverlay!.Value);
        else buffer.WriteVarInt(this.ChatType!.Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var content = buffer.ReadString();
        if (version.Version.Protocol >= ProtocolVersion.V_1_19_2)
            return new SystemChatMessagePacket(content, buffer.ReadBool());

        return new SystemChatMessagePacket(content, buffer.ReadVarInt());
    }
}
#pragma warning restore CS1591
