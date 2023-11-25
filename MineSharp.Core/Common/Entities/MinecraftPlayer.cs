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

    public MinecraftPlayer(string username, UUID uuid, int ping, GameMode gameMode, Entity? entity, PermissionLevel? permissionLevel = null)
    {
        this.Username = username;
        this.Uuid = uuid;
        this.Ping = ping;
        this.GameMode = gameMode;
        this.Entity = entity;
        this.PermissionLevel = permissionLevel;
    }
}
