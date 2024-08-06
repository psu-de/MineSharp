using MineSharp.Core.Geometry;

namespace MineSharp.Core.Common.Entities;

/// <summary>
///     Represents a minecraft player entity.
/// </summary>
/// <param name="username"></param>
/// <param name="uuid"></param>
/// <param name="ping"></param>
/// <param name="gameMode"></param>
/// <param name="entity"></param>
/// <param name="dimension"></param>
/// <param name="permissionLevel"></param>
public class MinecraftPlayer(
    string username,
    Uuid uuid,
    int ping,
    GameMode gameMode,
    Entity? entity,
    Dimension dimension,
    PermissionLevel? permissionLevel = null)
{
    /// <summary>
    ///     The username of this player
    /// </summary>
    public string Username { get; set; } = username;

    /// <summary>
    ///     The UUID of this player
    /// </summary>
    public Uuid Uuid { get; set; } = uuid;

    /// <summary>
    ///     Ping of this player in ms
    /// </summary>
    public int Ping { get; set; } = ping;

    /// <summary>
    ///     The <see cref="MineSharp.Core.Common.GameMode" /> of the player
    /// </summary>
    public GameMode GameMode { get; set; } = gameMode;

    /// <summary>
    ///     The underlying <see cref="MineSharp.Core.Common.Entities.Entity" /> of the player
    /// </summary>
    public Entity? Entity { get; set; } = entity;

    /// <summary>
    ///     The permission level of this player
    /// </summary>
    public PermissionLevel? PermissionLevel { get; set; } = permissionLevel;

    /// <summary>
    ///     The dimension the player is currently in
    /// </summary>
    public Dimension Dimension { get; set; } = dimension;

    /// <summary>
    ///     The pose of this player (TODO: this is never updated...)
    /// </summary>
    public EntityPose Pose { get; set; } = EntityPose.Standing;

    /// <summary>
    ///     The offset for the player's eye height.
    /// </summary>
    public static readonly Vector3 PlayerEyeHeightOffset = new(0, 1.62, 0);

    /// <summary>
    ///     The position of this player's head.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetHeadPosition()
    {
        return Entity!.Position.Plus(PlayerEyeHeightOffset);
    }
}
