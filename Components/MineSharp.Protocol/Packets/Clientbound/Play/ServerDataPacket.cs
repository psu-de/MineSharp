using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Sent by the server to provide server data such as MOTD, icon, and secure chat enforcement.
/// </summary>
/// <param name="Motd">The message of the day (MOTD) as a chat component.</param>
/// <param name="HasIcon">Indicates if the server has an icon.</param>
/// <param name="Icon">Optional icon bytes in the PNG format.</param>
/// <param name="EnforcesSecureChat">Indicates if the server enforces secure chat.</param>
public sealed record ServerDataPacket(Chat Motd, bool HasIcon, byte[]? Icon, bool EnforcesSecureChat) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ServerData;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteChatComponent(Motd);
        buffer.WriteBool(HasIcon);
        if (HasIcon && Icon is not null)
        {
            buffer.WriteVarInt(Icon.Length);
            buffer.WriteBytes(Icon);
        }
        buffer.WriteBool(EnforcesSecureChat);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var motd = buffer.ReadChatComponent();
        var hasIcon = buffer.ReadBool();
        byte[]? icon = null;
        if (hasIcon)
        {
            // TODO: Is the Size of the icon byte array always present?
            var size = buffer.ReadVarInt();
            icon = buffer.ReadBytes(size);
        }
        var enforcesSecureChat = buffer.ReadBool();

        return new ServerDataPacket(motd, hasIcon, icon, enforcesSecureChat);
    }
}
