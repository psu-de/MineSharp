using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Configuration.UpdateTagsPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Update Tags (configuration) packet
/// </summary>
/// <param name="Registries">Array of registries with their tags</param>
public sealed record UpdateTagsPacket(Registry[] Registries) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_Tags;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        WriteRegistries(buffer, Registries);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var registries = ReadRegistries(buffer);
        return new UpdateTagsPacket(registries);
    }

    private static void WriteRegistries(PacketBuffer buffer, Registry[] registries)
    {
        buffer.WriteVarInt(registries.Length);
        foreach (var registry in registries)
        {
            buffer.WriteIdentifier(registry.Identifier);
            WriteTags(buffer, registry.Tags);
        }
    }

    private static Registry[] ReadRegistries(PacketBuffer buffer)
    {
        var registryCount = buffer.ReadVarInt();
        var registries = new Registry[registryCount];
        for (int i = 0; i < registryCount; i++)
        {
            var registry = buffer.ReadIdentifier();
            var tags = ReadTags(buffer);
            registries[i] = new Registry(registry, tags);
        }
        return registries;
    }

    private static void WriteTags(PacketBuffer buffer, Tag[] tags)
    {
        buffer.WriteVarInt(tags.Length);
        foreach (var tag in tags)
        {
            buffer.WriteIdentifier(tag.Name);
            buffer.WriteVarInt(tag.Entries.Length);
            foreach (var entry in tag.Entries)
            {
                buffer.WriteVarInt(entry);
            }
        }
    }

    private static Tag[] ReadTags(PacketBuffer buffer)
    {
        var tagCount = buffer.ReadVarInt();
        var tags = new Tag[tagCount];
        for (int j = 0; j < tagCount; j++)
        {
            var tagName = buffer.ReadIdentifier();
            var entries = ReadEntries(buffer);
            tags[j] = new Tag(tagName, entries);
        }
        return tags;
    }

    private static int[] ReadEntries(PacketBuffer buffer)
    {
        var entryCount = buffer.ReadVarInt();
        var entries = new int[entryCount];
        for (int k = 0; k < entryCount; k++)
        {
            entries[k] = buffer.ReadVarInt();
        }
        return entries;
    }

    /// <summary>
    ///     Represents a registry with its tags
    /// </summary>
    /// <param name="Identifier">The registry identifier</param>
    /// <param name="Tags">Array of tags</param>
    public sealed record Registry(Identifier Identifier, Tag[] Tags);

    /// <summary>
    ///     Represents a tag with its entries
    /// </summary>
    /// <param name="Name">The tag name</param>
    /// <param name="Entries">Array of entries</param>
    public sealed record Tag(Identifier Name, int[] Entries);
}
