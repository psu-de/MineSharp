using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class OpenWindowPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_OpenWindow;

    public int    WindowId      { get; set; }
    public int    InventoryType { get; set; }
    public string WindowTitle   { get; set; }
    
    public Chat? WindowTitleChat { get; set; }
    public OpenWindowPacket(int windowId, int inventoryType, string windowTitle)
    {
        this.WindowId      = windowId;
        this.InventoryType = inventoryType;
        this.WindowTitle   = windowTitle;
    }
    
    public OpenWindowPacket(int windowId, int inventoryType, Chat windowTitle)
    {
        this.WindowId        = windowId;
        this.InventoryType   = inventoryType;
        this.WindowTitleChat = windowTitle;
        this.WindowTitle     = windowTitle.GetMessage(null);
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.WindowId);
        buffer.WriteVarInt(this.InventoryType);
        buffer.WriteString(this.WindowTitle);
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
