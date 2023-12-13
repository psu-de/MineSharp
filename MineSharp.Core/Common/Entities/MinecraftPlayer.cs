using MineSharp.Core.Common.Effects;

namespace MineSharp.Core.Common.Entities;

public class MinecraftPlayer
{
    public string Username { get; set; }
    public UUID Uuid { get; set; }
    public int Ping { get; set; }
    public GameMode GameMode { get; set; }
    public Entity? Entity { get; set; }
    public PermissionLevel? PermissionLevel { get; set; }
    public Dimension Dimension { get; set; }
    public EntityPose Pose { get; set; }

    public MinecraftPlayer(string username, UUID uuid, int ping, GameMode gameMode, Entity? entity, Dimension dimension, PermissionLevel? permissionLevel = null)
    {
        this.Username = username;
        this.Uuid = uuid;
        this.Ping = ping;
        this.GameMode = gameMode;
        this.Entity = entity;
        this.Dimension = dimension;
        this.PermissionLevel = permissionLevel;
        this.Pose = EntityPose.Standing;
    }
    
    /// <summary>
    /// The position of this player's head.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetHeadPosition() => this.Entity!.Position.Plus(Vector3.Up);

    public double GetEyeHeight()
    {
        return this.Pose switch {
            EntityPose.Swimming 
                or EntityPose.FallFlying 
                or EntityPose.SpinAttack => 0.4d,
            EntityPose.Crouching => 1.27d,
            _ => 1.62d
        };
    }
    
    /// <summary>
    /// The Y Coordinate of the eyes of this player
    /// </summary>
    /// <returns></returns>
    public double GetEyeY()
    {
        return this.Entity!.Position.Y + this.GetEyeHeight();
    }
}
