using System.Collections.Frozen;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.NetworkTypes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public interface IParticleData
{
    public void Write(PacketBuffer buffer, MinecraftData data);
}

public interface IParticleDataStatic
{
    public static abstract IParticleData Read(PacketBuffer buffer, MinecraftData data);
}

public static class ParticleDataRegistry
{
    public static IParticleData? Read(PacketBuffer buffer, MinecraftData data, int particleId)
    {
        var particleTypeIdentifier = data.Particles.GetName(particleId);
        return Read(buffer, data, particleTypeIdentifier);
    }

    public static IParticleData? Read(PacketBuffer buffer, MinecraftData data, Identifier typeIdentifier)
    {
        if (!ParticleDataFactories.TryGetValue(typeIdentifier, out var reader))
        {
            return null;
        }
        return reader(buffer, data);
    }

    public static readonly FrozenDictionary<Identifier, Func<PacketBuffer, MinecraftData, IParticleData>> ParticleDataFactories;

    static ParticleDataRegistry()
    {
        ParticleDataFactories = InitializeParticleData();
    }

    private static FrozenDictionary<Identifier, Func<PacketBuffer, MinecraftData, IParticleData>> InitializeParticleData()
    {
        var dict = new Dictionary<Identifier, Func<PacketBuffer, MinecraftData, IParticleData>>();

        void Register<T>(params Identifier[] identifiers)
            where T : IParticleData, IParticleDataStatic
        {
            var factory = T.Read;
            foreach (var identifier in identifiers)
            {
                dict.Add(identifier, factory);
            }
        }

        Register<BlockStateParticleData>(
            Identifier.Parse("minecraft:block"),
            Identifier.Parse("minecraft:block_marker"),
            Identifier.Parse("minecraft:falling_dust"),
            Identifier.Parse("minecraft:dust_pillar")
        );
        Register<DustParticleData>(
            Identifier.Parse("minecraft:dust")
        );
        Register<DustColorTransitionParticleData>(
            Identifier.Parse("minecraft:dust_color_transition")
        );
        Register<EntityEffectParticleData>(
            Identifier.Parse("minecraft:entity_effect")
        );
        Register<SculkChargeParticleData>(
            Identifier.Parse("minecraft:sculk_charge")
        );
        Register<ItemParticleData>(
            Identifier.Parse("minecraft:item")
        );
        Register<VibrationParticleData>(
            Identifier.Parse("minecraft:vibration")
        );
        Register<ShriekParticleData>(
            Identifier.Parse("minecraft:shriek")
        );

        return dict.ToFrozenDictionary();
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// Represents particle data that includes a block state.
/// </summary>
/// <param name="BlockState">The ID of the block state.</param>
public sealed record BlockStateParticleData(int BlockState) : IParticleData, IParticleDataStatic, ISerializableWithMinecraftData<BlockStateParticleData>
{
    /// <inheritdoc/>
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(BlockState);
    }

    /// <inheritdoc/>
    public static BlockStateParticleData Read(PacketBuffer buffer, MinecraftData data)
    {
        var blockState = buffer.ReadVarInt();

        return new(blockState);
    }

    static IParticleData IParticleDataStatic.Read(PacketBuffer buffer, MinecraftData data) => Read(buffer, data);
}

/// <summary>
/// Represents the data for the "dust" particle.
/// </summary>
/// <param name="Red">The red RGB value, between 0 and 1. Divide actual RGB value by 255.</param>
/// <param name="Green">The green RGB value, between 0 and 1. Divide actual RGB value by 255.</param>
/// <param name="Blue">The blue RGB value, between 0 and 1. Divide actual RGB value by 255.</param>
/// <param name="Scale">The scale, will be clamped between 0.01 and 4.</param>
public sealed record DustParticleData(
    float Red,
    float Green,
    float Blue,
    float Scale
) : IParticleData, IParticleDataStatic, ISerializableWithMinecraftData<DustParticleData>
{
    /// <inheritdoc/>
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(Red);
        buffer.WriteFloat(Green);
        buffer.WriteFloat(Blue);
        buffer.WriteFloat(Scale);
    }

    /// <inheritdoc/>
    public static DustParticleData Read(PacketBuffer buffer, MinecraftData data)
    {
        var red = buffer.ReadFloat();
        var green = buffer.ReadFloat();
        var blue = buffer.ReadFloat();
        var scale = buffer.ReadFloat();

        return new DustParticleData(red, green, blue, scale);
    }

    static IParticleData IParticleDataStatic.Read(PacketBuffer buffer, MinecraftData data) => Read(buffer, data);
}

/// <summary>
/// Represents the data for the "dust_color_transition" particle.
/// </summary>
/// <param name="FromRed">The start red RGB value, between 0 and 1. Divide actual RGB value by 255.</param>
/// <param name="FromGreen">The start green RGB value, between 0 and 1. Divide actual RGB value by 255.</param>
/// <param name="FromBlue">The start blue RGB value, between 0 and 1. Divide actual RGB value by 255.</param>
/// <param name="ToRed">The end red RGB value, between 0 and 1. Divide actual RGB value by 255.</param>
/// <param name="ToGreen">The end green RGB value, between 0 and 1. Divide actual RGB value by 255.</param>
/// <param name="ToBlue">The end blue RGB value, between 0 and 1. Divide actual RGB value by 255.</param>
/// <param name="Scale">The scale, will be clamped between 0.01 and 4.</param>
public sealed record DustColorTransitionParticleData(
    float FromRed,
    float FromGreen,
    float FromBlue,
    float ToRed,
    float ToGreen,
    float ToBlue,
    float Scale
) : IParticleData, IParticleDataStatic, ISerializableWithMinecraftData<DustColorTransitionParticleData>
{
    /// <inheritdoc/>
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(FromRed);
        buffer.WriteFloat(FromGreen);
        buffer.WriteFloat(FromBlue);
        buffer.WriteFloat(ToRed);
        buffer.WriteFloat(ToGreen);
        buffer.WriteFloat(ToBlue);
        buffer.WriteFloat(Scale);
    }

    /// <inheritdoc/>
    public static DustColorTransitionParticleData Read(PacketBuffer buffer, MinecraftData data)
    {
        var fromRed = buffer.ReadFloat();
        var fromGreen = buffer.ReadFloat();
        var fromBlue = buffer.ReadFloat();
        var toRed = buffer.ReadFloat();
        var toGreen = buffer.ReadFloat();
        var toBlue = buffer.ReadFloat();
        var scale = buffer.ReadFloat();

        return new(fromRed, fromGreen, fromBlue, toRed, toGreen, toBlue, scale);
    }

    static IParticleData IParticleDataStatic.Read(PacketBuffer buffer, MinecraftData data) => Read(buffer, data);
}

/// <summary>
/// Represents the data for the "entity_effect" particle.
/// </summary>
/// <param name="Color">The ARGB components of the color encoded as an Int.</param>
public sealed record EntityEffectParticleData(int Color) : IParticleData, IParticleDataStatic, ISerializableWithMinecraftData<EntityEffectParticleData>
{
    /// <inheritdoc/>
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteInt(Color);
    }

    /// <inheritdoc/>
    public static EntityEffectParticleData Read(PacketBuffer buffer, MinecraftData data)
    {
        var color = buffer.ReadInt();

        return new(color);
    }

    static IParticleData IParticleDataStatic.Read(PacketBuffer buffer, MinecraftData data) => Read(buffer, data);
}

/// <summary>
/// Represents the data for the "sculk_charge" particle.
/// </summary>
/// <param name="Roll">How much the particle will be rotated when displayed.</param>
public sealed record SculkChargeParticleData(float Roll) : IParticleData, IParticleDataStatic, ISerializableWithMinecraftData<SculkChargeParticleData>
{
    /// <inheritdoc/>
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(Roll);
    }

    /// <inheritdoc/>
    public static SculkChargeParticleData Read(PacketBuffer buffer, MinecraftData data)
    {
        var roll = buffer.ReadFloat();

        return new(roll);
    }

    static IParticleData IParticleDataStatic.Read(PacketBuffer buffer, MinecraftData data) => Read(buffer, data);
}

/// <summary>
/// Represents the data for the "item" particle.
/// </summary>
/// <param name="Item">The item that will be used.</param>
public sealed record ItemParticleData(Item Item) : IParticleData, IParticleDataStatic, ISerializableWithMinecraftData<ItemParticleData>
{
    /// <inheritdoc/>
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteOptionalItem(Item);
    }

    /// <inheritdoc/>
    public static ItemParticleData Read(PacketBuffer buffer, MinecraftData data)
    {
        var item = buffer.ReadOptionalItem(data)!;

        return new(item);
    }

    static IParticleData IParticleDataStatic.Read(PacketBuffer buffer, MinecraftData data) => Read(buffer, data);
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public enum PositionSourceType
{
    Block = 0,
    Entity = 1
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// Represents the data for the "vibration" particle.
/// </summary>
/// <param name="PositionSourceType">The source type of the position (Block or Entity).</param>
/// <param name="BlockPosition">TThe position of the block the vibration originated from, if the source type is Block.</param>
/// <param name="EntityId">The ID of the entity the vibration originated from, if the source type is Entity.</param>
/// <param name="EntityEyeHeight">The height of the entity's eye relative to the entity, if the source type is Entity.</param>
/// <param name="Ticks">The amount of ticks it takes for the vibration to travel from its source to its destination.</param>
public sealed record VibrationParticleData(
    PositionSourceType PositionSourceType,
    Position? BlockPosition,
    int? EntityId,
    float? EntityEyeHeight,
    int Ticks
) : IParticleData, IParticleDataStatic, ISerializableWithMinecraftData<VibrationParticleData>
{
    public static readonly Identifier BlockPositionSourceType = Identifier.Parse("minecraft:block");
    public static readonly Identifier EntityPositionSourceType = Identifier.Parse("minecraft:entity");

    public static readonly FrozenDictionary<PositionSourceType, Identifier> PositionSourceTypes = new Dictionary<PositionSourceType, Identifier>
    {
        { PositionSourceType.Block, BlockPositionSourceType },
        { PositionSourceType.Entity, EntityPositionSourceType },
    }.ToFrozenDictionary();

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt((int)PositionSourceType);
        var positionSourceTypeIdentifier = PositionSourceTypes[PositionSourceType];

        if (positionSourceTypeIdentifier == BlockPositionSourceType)
        {
            buffer.WritePosition(BlockPosition ?? throw new InvalidOperationException($"{nameof(BlockPosition)} must be set when {nameof(PositionSourceType)} is {PositionSourceType}"));
        }
        else if (positionSourceTypeIdentifier == EntityPositionSourceType)
        {
            buffer.WriteVarInt(EntityId ?? throw new InvalidOperationException($"{nameof(EntityId)} must be set when {nameof(PositionSourceType)} is {PositionSourceType}"));
            buffer.WriteFloat(EntityEyeHeight ?? throw new InvalidOperationException($"{nameof(EntityEyeHeight)} must be set when {nameof(PositionSourceType)} is {PositionSourceType}"));
        }

        buffer.WriteVarInt(Ticks);
    }

    /// <inheritdoc />
    public static VibrationParticleData Read(PacketBuffer buffer, MinecraftData data)
    {
        var positionSourceType = (PositionSourceType)buffer.ReadVarInt();
        var positionSourceTypeIdentifier = PositionSourceTypes[positionSourceType];

        Position? blockPosition = null;
        int? entityId = null;
        float? entityEyeHeight = null;
        if (positionSourceTypeIdentifier == BlockPositionSourceType)
        {
            blockPosition = buffer.ReadPosition();
        }
        else if (positionSourceTypeIdentifier == EntityPositionSourceType)
        {
            entityId = buffer.ReadVarInt();
            entityEyeHeight = buffer.ReadFloat();
        }
        var ticks = buffer.ReadVarInt();

        return new(positionSourceType, blockPosition, entityId, entityEyeHeight, ticks);
    }

    static IParticleData IParticleDataStatic.Read(PacketBuffer buffer, MinecraftData data) => Read(buffer, data);
}

/// <summary>
/// Represents the data for the "shriek" particle.
/// </summary>
/// <param name="Delay">The time in ticks before the particle is displayed.</param>
public sealed record ShriekParticleData(int Delay) : IParticleData, IParticleDataStatic, ISerializableWithMinecraftData<ShriekParticleData>
{
    /// <inheritdoc/>
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(Delay);
    }

    /// <inheritdoc/>
    public static ShriekParticleData Read(PacketBuffer buffer, MinecraftData data)
    {
        var delay = buffer.ReadVarInt();

        return new(delay);
    }

    static IParticleData IParticleDataStatic.Read(PacketBuffer buffer, MinecraftData data) => Read(buffer, data);
}
