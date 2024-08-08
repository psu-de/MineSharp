using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
/// Represents a packet to open a window in the client.
/// </summary>
/// <param name="WindowId">The ID of the window.</param>
/// <param name="InventoryType">The type of the inventory.</param>
/// <param name="WindowTitle">The title of the window.</param>
/// <param name="WindowTitleChat">The chat component of the window title.</param>
public sealed record OpenWindowPacket(int WindowId, int InventoryType, string WindowTitle, Chat? WindowTitleChat = null) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_OpenWindow;

    public OpenWindowPacket(int windowId, int inventoryType, Chat windowTitle)
        : this(windowId, inventoryType, windowTitle.GetMessage(null), windowTitle)
    {
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(WindowId);
        buffer.WriteVarInt(InventoryType);
        buffer.WriteString(WindowTitle);
    }

    /// <inheritdoc />
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
