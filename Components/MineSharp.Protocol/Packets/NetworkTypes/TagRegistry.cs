using MineSharp.Core.Common;
using MineSharp.Core.Serialization;

namespace MineSharp.Protocol.Packets.NetworkTypes;

/// <summary>
///     Registry record
/// </summary>
/// <param name="Identifier">Registry identifier</param>
/// <param name="Tags">Array of tags</param>
public sealed record Registry(Identifier Identifier, Tag[] Tags) : ISerializable<Registry>
{
    /// <inheritdoc />
    public void Write(PacketBuffer buffer)
    {
        buffer.WriteIdentifier(Identifier);
        buffer.WriteVarInt(Tags.Length);
        foreach (var tag in Tags)
        {
            tag.Write(buffer);
        }
    }

    /// <inheritdoc />
    public static Registry Read(PacketBuffer buffer)
    {
        var identifier = buffer.ReadIdentifier();
        var length = buffer.ReadVarInt();
        var tags = new Tag[length];
        for (var i = 0; i < length; i++)
        {
            tags[i] = Tag.Read(buffer);
        }

        return new Registry(identifier, tags);
    }
}

/// <summary>
///     Tag record
/// </summary>
/// <param name="TagName">Tag name</param>
/// <param name="Entries">Array of entries</param>
public sealed record Tag(Identifier TagName, int[] Entries) : ISerializable<Tag>
{
    /// <inheritdoc />
    public void Write(PacketBuffer buffer)
    {
        buffer.WriteIdentifier(TagName);
        buffer.WriteVarInt(Entries.Length);
        foreach (var entry in Entries)
        {
            buffer.WriteVarInt(entry);
        }
    }

    /// <inheritdoc />
    public static Tag Read(PacketBuffer buffer)
    {
        var tagName = buffer.ReadIdentifier();
        var length = buffer.ReadVarInt();
        var entries = new int[length];
        for (var i = 0; i < length; i++)
        {
            entries[i] = buffer.ReadVarInt();
        }

        return new Tag(tagName, entries);
    }
}
