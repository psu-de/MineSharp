using System.Collections.Concurrent;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Geometry;
using Attribute = MineSharp.Core.Common.Entities.Attributes.Attribute;

namespace MineSharp.Core.Common.Entities;

/// <summary>
///     Represents an Entity
/// </summary>
/// <param name="info"></param>
/// <param name="serverId"></param>
/// <param name="position"></param>
/// <param name="pitch"></param>
/// <param name="yaw"></param>
/// <param name="velocity"></param>
/// <param name="isOnGround"></param>
/// <param name="effects"></param>
public class Entity(
    EntityInfo info,
    int serverId,
    Vector3 position,
    float pitch,
    float yaw,
    Vector3 velocity,
    bool isOnGround,
    Dictionary<EffectType, Effect?> effects)
{
    /// <summary>
    ///     The entity descriptor
    /// </summary>
    public readonly EntityInfo Info = info;

    /// <summary>
    ///     The Passengers of an entity (for example a boat can takes 2 passengers)
    /// </summary>
    public List<Entity> Passengers = [];

    /// <summary>
    ///     The id the entity has on the minecraft server
    /// </summary>
    public int ServerId { get; set; } = serverId;

    /// <summary>
    ///     The position of this entity
    /// </summary>
    public Vector3 Position { get; } = position;

    /// <summary>
    ///     The pitch of this entity (in degrees)
    /// </summary>
    public float Pitch { get; set; } = pitch;

    /// <summary>
    ///     The yaw of this entity (in degrees)
    /// </summary>
    public float Yaw { get; set; } = yaw;

    /// <summary>
    ///     The Velocity of this Entity
    /// </summary>
    public Vector3 Velocity { get; } = velocity;

    /// <summary>
    ///     Whether this entity is on the ground
    /// </summary>
    public bool IsOnGround { get; set; } = isOnGround;

    /// <summary>
    ///     Currently active effects of this entity
    /// </summary>
    public IDictionary<EffectType, Effect?> Effects { get; } = effects;

    /// <summary>
    ///     A list of attributes active on this entity
    /// </summary>
    public ConcurrentDictionary<Identifier, Attribute> Attributes { get; } = new();

    /// <summary>
    ///     The entity this entity is riding (f.e. an boat or a minecart)
    /// </summary>
    public Entity? Vehicle { get; set; } = null;

    /// <summary>
    ///     Returns the attribute with the given name.
    ///     If this entity doesn't have this attribute, null is returned
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Attribute? GetAttribute(Identifier name)
    {
        Attributes.TryGetValue(name, out var attr);
        return attr;
    }

    /// <summary>
    ///     Add an attribute to this entity
    /// </summary>
    /// <param name="attribute"></param>
    public void AddAttribute(Attribute attribute)
    {
        Attributes.TryAdd(attribute.Key, attribute);
    }

    /// <summary>
    ///     Returns the level of the given effect type or null if the entity does not have this effect
    /// </summary>
    /// <param name="effectType"></param>
    /// <returns></returns>
    public int? GetEffectLevel(EffectType effectType)
    {
        if (!Effects.TryGetValue(effectType, out var effect))
        {
            return null;
        }

        return effect?.Amplifier + 1;
    }

    /// <summary>
    ///     Return the bounding box for this entity.
    /// </summary>
    /// <returns></returns>
    public MutableAabb GetBoundingBox()
    {
        var half = Info.Width / 2.0f;
        var height = Info.Height;

        return new(
            Position.X - half,
            Position.Y,
            Position.Z - half,
            Position.X + half,
            Position.Y + height,
            Position.Z + half);
    }

    /// <summary>
    ///     The direction in which this entity is looking
    /// </summary>
    /// <returns></returns>
    public Vector3 GetLookVector()
    {
        var pitchRadians = Pitch * (MathF.PI / 180.0f);
        var yawRadians = Yaw * (MathF.PI / 180.0);

        var len = Math.Cos(pitchRadians);
        var x = len * Math.Sin(-yawRadians);
        var y = -Math.Sin(pitchRadians);
        var z = len * Math.Cos(yawRadians);

        return new MutableVector3(x, y, z).Normalize();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Entity(Info={Info}, Position={Position})";
    }
}
