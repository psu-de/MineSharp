using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class OpenWindowPacket : IPacket
{
    public OpenWindowPacket(int windowId, int inventoryType, string windowTitle)
    {
        WindowId = windowId;
        InventoryType = inventoryType;
        WindowTitle = windowTitle;
    }

    public OpenWindowPacket(int windowId, int inventoryType, Chat windowTitle)
    {
        WindowId = windowId;
        InventoryType = inventoryType;
        WindowTitleChat = windowTitle;
        WindowTitle = windowTitle.GetMessage(null);
    }

    public int WindowId { get; set; }
    public int InventoryType { get; set; }
    public string WindowTitle { get; set; }

    public Chat? WindowTitleChat { get; set; }
    public PacketType Type => PacketType.CB_Play_OpenWindow;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(WindowId);
        buffer.WriteVarInt(InventoryType);
        buffer.WriteString(WindowTitle);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol == ProtocolVersion.V_1_20_3)
        {
            return new OpenWindowPacket(
                buffer.ReadVarInt(),
                buffer.ReadVarInt(),
                buffer.ReadChatComponent()
            );
        }

        return new OpenWindowPacket(
            buffer.ReadVarInt(),
            buffer.ReadVarInt(),
            buffer.ReadString());
    }
}
#pragma warning restore CS1591
