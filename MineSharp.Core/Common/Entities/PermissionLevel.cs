namespace MineSharp.Core.Common.Entities;

/// <summary>
///     Specifies the Permission level a player has on a minecraft server
/// </summary>
public enum PermissionLevel
{
    /// <summary>
    ///     No special permissions
    /// </summary>
    Normal = 0,

    /// <summary>
    ///     Player can bypass spawn protection.
    /// </summary>
    Moderator = 1,

    /// <summary>
    ///     Player or executor can use more commands and player can use command blocks.
    /// </summary>
    GameMaster = 2,

    /// <summary>
    ///     Player or executor can use commands related to multiplayer management.
    /// </summary>
    Admin = 3,

    /// <summary>
    ///     Player or executor can use all of the commands, including commands related to server management.
    /// </summary>
    Owner = 4
}
