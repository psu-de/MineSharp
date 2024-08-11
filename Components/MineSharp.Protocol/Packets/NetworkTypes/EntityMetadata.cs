using System.Collections.Frozen;
using System.Diagnostics.Contracts;
using fNbt;
using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using static MineSharp.Protocol.Packets.NetworkTypes.SnifferStateMetadata;

namespace MineSharp.Protocol.Packets.NetworkTypes;

/// <summary>
/// Represents the metadata of an entity.
/// </summary>
/// <param name="Entries">The metadata entries.</param>
public sealed record EntityMetadata(EntityMetadataEntry[] Entries) : ISerializableWithMinecraftData<EntityMetadata>
{
    /// <summary>
    /// Represents the end of metadata index.
    /// </summary>
    public const byte EndOfMetadataIndex = 0xff;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        foreach (var entry in Entries)
        {
            entry.Write(buffer, data);
        }
        // Write the terminating entry
        buffer.WriteByte(EndOfMetadataIndex);
    }

    /// <inheritdoc />
    public static EntityMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var entries = new List<EntityMetadataEntry>();
        while (true)
        {
            // TODO: Some metadata is broken and causes exceptions
            var index = buffer.Peek();
            if (index == EndOfMetadataIndex)
            {
                buffer.ReadByte(); // Consume the index
                break;
            }

            var entry = EntityMetadataEntry.Read(buffer, data);
            entries.Add(entry);
        }
        return new EntityMetadata(entries.ToArray());
    }
}

/// <summary>
/// Represents a entity metadata entry.
/// </summary>
public sealed record EntityMetadataEntry(byte Index, int Type, IMetadataValue Value) : ISerializableWithMinecraftData<EntityMetadataEntry>
{
    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteByte(Index);
        buffer.WriteVarInt(Type);
        Value!.Write(buffer, data);
    }

    /// <inheritdoc />
    public static EntityMetadataEntry Read(PacketBuffer buffer, MinecraftData data)
    {
        var index = buffer.ReadByte();
        var type = buffer.ReadVarInt();
        var value = MetadataValueFactory.Create(type, buffer, data);

        return new(index, type, value);
    }
}

/// <summary>
/// Factory for creating metadata values based on type.
/// </summary>
public static class MetadataValueFactory
{
    private static readonly FrozenDictionary<int, Func<PacketBuffer, MinecraftData, IMetadataValue>> TypeToReadDelegate = new Dictionary<int, Func<PacketBuffer, MinecraftData, IMetadataValue>>()
    {
        { 0, ByteMetadata.Read },
        { 1, VarIntMetadata.Read },
        { 2, VarLongMetadata.Read },
        { 3, FloatMetadata.Read },
        { 4, StringMetadata.Read },
        { 5, TextComponentMetadata.Read },
        { 6, OptionalTextComponentMetadata.Read },
        { 7, SlotMetadata.Read },
        { 8, BooleanMetadata.Read },
        { 9, RotationsMetadata.Read },
        { 10, PositionMetadata.Read },
        { 11, OptionalPositionMetadata.Read },
        { 12, DirectionMetadata.Read },
        { 13, OptionalUuidMetadata.Read },
        { 14, BlockStateMetadata.Read },
        { 15, OptionalBlockStateMetadata.Read },
        { 16, NbtMetadata.Read },
        { 17, ParticleMetadata.Read },
        { 18, VillagerDataMetadata.Read },
        { 19, OptionalVarIntMetadata.Read },
        { 20, PoseMetadata.Read },
        { 21, CatVariantMetadata.Read },
        { 22, FrogVariantMetadata.Read },
        { 23, OptionalGlobalPositionMetadata.Read },
        { 24, PaintingVariantMetadata.Read },
        { 25, SnifferStateMetadata.Read },
        { 26, Vector3Metadata.Read },
        { 27, QuaternionMetadata.Read },
    }.ToFrozenDictionary();

    /// <summary>
    /// Creates an instance of <see cref="IMetadataValue"/> based on the provided type.
    /// </summary>
    /// <param name="type">The type of the metadata value.</param>
    /// <param name="buffer">The packet buffer to read from.</param>
    /// <param name="data">The Minecraft data context.</param>
    /// <returns>An instance of <see cref="IMetadataValue"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the type is unknown.</exception>
    public static IMetadataValue Create(int type, PacketBuffer buffer, MinecraftData data)
    {
        if (TypeToReadDelegate.TryGetValue(type, out var readDelegate))
        {
            return readDelegate(buffer, data);
        }

        throw new ArgumentOutOfRangeException(nameof(type), $"Unknown metadata type: {type}");
    }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
/// <summary>
/// Interface for metadata values.
/// </summary>
public interface IMetadataValue
{
    public abstract void Write(PacketBuffer buffer, MinecraftData data);
}

public sealed record ByteMetadata(byte Value) : IMetadataValue, ISerializableWithMinecraftData<ByteMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteByte(Value);
    public static ByteMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadByte());
}

public sealed record VarIntMetadata(int Value) : IMetadataValue, ISerializableWithMinecraftData<VarIntMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarInt(Value);
    public static VarIntMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadVarInt());
}

public sealed record VarLongMetadata(long Value) : IMetadataValue, ISerializableWithMinecraftData<VarLongMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarLong(Value);
    public static VarLongMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadVarLong());
}

public sealed record FloatMetadata(float Value) : IMetadataValue, ISerializableWithMinecraftData<FloatMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteFloat(Value);
    public static FloatMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadFloat());
}

public sealed record StringMetadata(string Value) : IMetadataValue, ISerializableWithMinecraftData<StringMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteString(Value);
    public static StringMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadString());
}

public sealed record TextComponentMetadata(Chat Value) : IMetadataValue, ISerializableWithMinecraftData<TextComponentMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteChatComponent(Value);
    public static TextComponentMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadChatComponent());
}

public sealed record OptionalTextComponentMetadata(Chat? Value) : IMetadataValue, ISerializableWithMinecraftData<OptionalTextComponentMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        var hasValue = Value != null;
        buffer.WriteBool(hasValue);
        if (hasValue) buffer.WriteChatComponent(Value!);
    }

    public static OptionalTextComponentMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var hasValue = buffer.ReadBool();
        var value = hasValue ? buffer.ReadChatComponent() : null;
        return new(value);
    }
}

public sealed record SlotMetadata(Item? Value) : IMetadataValue, ISerializableWithMinecraftData<SlotMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteOptionalItem(Value);
    public static SlotMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadOptionalItem(data));
}

public sealed record BooleanMetadata(bool Value) : IMetadataValue, ISerializableWithMinecraftData<BooleanMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteBool(Value);
    public static BooleanMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadBool());
}

public sealed record RotationsMetadata(float X, float Y, float Z) : IMetadataValue, ISerializableWithMinecraftData<RotationsMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(X);
        buffer.WriteFloat(Y);
        buffer.WriteFloat(Z);
    }

    public static RotationsMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var x = buffer.ReadFloat();
        var y = buffer.ReadFloat();
        var z = buffer.ReadFloat();
        return new(x, y, z);
    }
}

public sealed record PositionMetadata(Position Value) : IMetadataValue, ISerializableWithMinecraftData<PositionMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WritePosition(Value);
    public static PositionMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadPosition());
}

/// <param name="Value">Position is present if the Boolean is set to true.</param>
public sealed record OptionalPositionMetadata(Position? Value) : IMetadataValue, ISerializableWithMinecraftData<OptionalPositionMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        var hasValue = Value.HasValue;
        buffer.WriteBool(hasValue);
        if (hasValue) buffer.WritePosition(Value!.Value);
    }

    public static OptionalPositionMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var hasValue = buffer.ReadBool();
        Position? value = hasValue ? buffer.ReadPosition() : null;
        return new(value);
    }
}

public sealed record DirectionMetadata(Direction Value) : IMetadataValue, ISerializableWithMinecraftData<DirectionMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarInt((int)Value);
    public static DirectionMetadata Read(PacketBuffer buffer, MinecraftData data) => new((Direction)buffer.ReadVarInt());
}

public sealed record OptionalUuidMetadata(Uuid? Value) : IMetadataValue, ISerializableWithMinecraftData<OptionalUuidMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        var hasValue = Value.HasValue;
        buffer.WriteBool(hasValue);
        if (hasValue) buffer.WriteUuid(Value!.Value);
    }

    public static OptionalUuidMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var hasValue = buffer.ReadBool();
        Uuid? value = hasValue ? buffer.ReadUuid() : null;
        return new(value);
    }
}

/// <param name="Value">An ID in the block state registry.</param>
public sealed record BlockStateMetadata(int Value) : IMetadataValue, ISerializableWithMinecraftData<BlockStateMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarInt(Value);
    public static BlockStateMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadVarInt());
}

/// <param name="Value">0 for absent (air is unrepresentable); otherwise, an ID in the block state registry.</param>
public sealed record OptionalBlockStateMetadata(int Value) : IMetadataValue, ISerializableWithMinecraftData<OptionalBlockStateMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarInt(Value);
    public static OptionalBlockStateMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadVarInt());
}

public sealed record NbtMetadata(NbtTag Value) : IMetadataValue, ISerializableWithMinecraftData<NbtMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteNbt(Value);
    public static NbtMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadNbt());
}

public sealed record ParticleMetadata(int ParticleType, byte[]? ParticleData) : IMetadataValue, ISerializableWithMinecraftData<ParticleMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(ParticleType);
        // TODO: Write particle data
        //buffer.WriteObject(ParticleData);
    }

    public static ParticleMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var particleType = buffer.ReadVarInt();
        //var particleData = buffer.ReadObject();
        return new(particleType, null);
    }
}

public sealed record VillagerDataMetadata(int VillagerType, int VillagerProfession, int Level) : IMetadataValue, ISerializableWithMinecraftData<VillagerDataMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(VillagerType);
        buffer.WriteVarInt(VillagerProfession);
        buffer.WriteVarInt(Level);
    }

    public static VillagerDataMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var villagerType = buffer.ReadVarInt();
        var villagerProfession = buffer.ReadVarInt();
        var level = buffer.ReadVarInt();
        return new(villagerType, villagerProfession, level);
    }
}

public sealed record OptionalVarIntMetadata(int? Value) : IMetadataValue, ISerializableWithMinecraftData<OptionalVarIntMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(Value.HasValue ? Value.Value + 1 : 0);
    }

    public static OptionalVarIntMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var value = buffer.ReadVarInt();
        return new(value == 0 ? (int?)null : value - 1);
    }
}

public sealed record PoseMetadata(EntityPose Value) : IMetadataValue, ISerializableWithMinecraftData<PoseMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarInt((int)Value);
    public static PoseMetadata Read(PacketBuffer buffer, MinecraftData data) => new((EntityPose)buffer.ReadVarInt());
}

public sealed record CatVariantMetadata(int Value) : IMetadataValue, ISerializableWithMinecraftData<CatVariantMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarInt(Value);
    public static CatVariantMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadVarInt());
}

public sealed record FrogVariantMetadata(int Value) : IMetadataValue, ISerializableWithMinecraftData<FrogVariantMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarInt(Value);
    public static FrogVariantMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadVarInt());
}

public sealed record OptionalGlobalPositionMetadata(bool HasValue, Identifier? DimensionIdentifier, Position? Position) : IMetadataValue, ISerializableWithMinecraftData<OptionalGlobalPositionMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteBool(HasValue);
        if (HasValue)
        {
            buffer.WriteIdentifier(DimensionIdentifier ?? throw new InvalidOperationException($"{nameof(DimensionIdentifier)} must not be null if {nameof(HasValue)} is true."));
            buffer.WritePosition(Position ?? throw new InvalidOperationException($"{nameof(Position)} must not be null if {nameof(HasValue)} is true."));
        }
    }

    public static OptionalGlobalPositionMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var hasValue = buffer.ReadBool();
        var dimensionIdentifier = hasValue ? buffer.ReadIdentifier() : null;
        Position? position = hasValue ? buffer.ReadPosition() : null;
        return new(hasValue, dimensionIdentifier, position);
    }
}

public sealed record PaintingVariantMetadata(int Value) : IMetadataValue, ISerializableWithMinecraftData<PaintingVariantMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarInt(Value);
    public static PaintingVariantMetadata Read(PacketBuffer buffer, MinecraftData data) => new(buffer.ReadVarInt());
}

public sealed record SnifferStateMetadata(SnifferState Value) : IMetadataValue, ISerializableWithMinecraftData<SnifferStateMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data) => buffer.WriteVarInt((int)Value);
    public static SnifferStateMetadata Read(PacketBuffer buffer, MinecraftData data) => new((SnifferState)buffer.ReadVarInt());

    public enum SnifferState
    {
        Idling = 0,
        FeelingHappy = 1,
        Scenting = 2,
        Sniffing = 3,
        Searching = 4,
        Digging = 5,
        Rising = 6
    }
}

public sealed record Vector3Metadata(float X, float Y, float Z) : IMetadataValue, ISerializableWithMinecraftData<Vector3Metadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(X);
        buffer.WriteFloat(Y);
        buffer.WriteFloat(Z);
    }

    public static Vector3Metadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var x = buffer.ReadFloat();
        var y = buffer.ReadFloat();
        var z = buffer.ReadFloat();
        return new(x, y, z);
    }

    [Pure]
    public Vector3 ToVector3() => new(X, Y, Z);
}

public sealed record QuaternionMetadata(float X, float Y, float Z, float W) : IMetadataValue, ISerializableWithMinecraftData<QuaternionMetadata>
{
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(X);
        buffer.WriteFloat(Y);
        buffer.WriteFloat(Z);
        buffer.WriteFloat(W);
    }

    public static QuaternionMetadata Read(PacketBuffer buffer, MinecraftData data)
    {
        var x = buffer.ReadFloat();
        var y = buffer.ReadFloat();
        var z = buffer.ReadFloat();
        var w = buffer.ReadFloat();
        return new(x, y, z, w);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
