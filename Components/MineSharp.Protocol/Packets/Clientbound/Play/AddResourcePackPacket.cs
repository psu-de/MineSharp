using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to add a resource pack.
/// </summary>
/// <param name="Uuid">The unique identifier of the resource pack.</param>
/// <param name="Url">The URL to the resource pack.</param>
/// <param name="Hash">A 40 character hexadecimal, case-insensitive SHA-1 hash of the resource pack file.</param>
/// <param name="Forced">Whether the client is forced to use the resource pack.</param>
/// <param name="PromptMessage">The custom message shown in the prompt, if present.</param>
public sealed record AddResourcePackPacket(Uuid Uuid, string Url, string Hash, bool Forced, Chat? PromptMessage) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_AddResourcePack;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteUuid(Uuid);
        buffer.WriteString(Url);
        buffer.WriteString(Hash);
        buffer.WriteBool(Forced);
        var hasPromptMessage = PromptMessage is not null;
        buffer.WriteBool(hasPromptMessage);
        if (hasPromptMessage && PromptMessage is not null)
        {
            buffer.WriteChatComponent(PromptMessage);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var uuid = buffer.ReadUuid();
        var url = buffer.ReadString();
        var hash = buffer.ReadString();
        var forced = buffer.ReadBool();
        var hasPromptMessage = buffer.ReadBool();
        Chat? promptMessage = hasPromptMessage ? buffer.ReadChatComponent() : null;

        return new AddResourcePackPacket(uuid, url, hash, forced, promptMessage);
    }
}
