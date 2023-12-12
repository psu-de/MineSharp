using MineSharp.Core.Common.Effects;
using System.Collections.Concurrent;
using Attribute = MineSharp.Core.Common.Entities.Attributes.Attribute;

namespace MineSharp.Core.Common.Entities;

public class Entity
{
    public readonly EntityInfo Info;
    
    public int ServerId { get; set;}
    public Vector3 Position { get; set; }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public Vector3 Velocity { get; set; }
    public bool IsOnGround { get; set; }
    public IDictionary<EffectType, Effect?> Effects { get; }
    public IDictionary<string, Attribute> Attributes { get; }
    public Entity? Vehicle { get; set; }


    public Entity(EntityInfo info, int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<EffectType, Effect?> effects)
    {
        this.Info = info;
        this.ServerId = serverId;
        this.Position = position;
        this.Pitch = pitch;
        this.Yaw = yaw;
        this.Velocity = velocity;
        this.IsOnGround = isOnGround;
        this.Effects = effects;
        this.Attributes = new ConcurrentDictionary<string, Attribute>();
        this.Vehicle = null;
    }
    
    public int? GetEffectLevel(EffectType effectType)
    {

        if (!this.Effects.TryGetValue(effectType, out var effect))
        {
            return null;
        }

        return effect?.Amplifier + 1;
    }

    public override string ToString()
        => $"Entity(Info={this.Info}, Position={Position})";

    public AABB GetBoundingBox()
    {
        var half = this.Info.Width / 2.0f;
        var height = this.Info.Height;
        
        return new AABB(
            this.Position.X - half,
            this.Position.Y,
            this.Position.Z - half,
            this.Position.X + half,
            this.Position.Y + height,
            this.Position.Z + half);
    }
}
