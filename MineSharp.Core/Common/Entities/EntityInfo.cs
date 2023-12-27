namespace MineSharp.Core.Common.Entities;

/// <summary>
/// Entity descriptor class
/// </summary>
/// <param name="Id">The numerical id of the entity (depends on minecraft version)</param>
/// <param name="Type">The Type of this entity (independent of minecraft version)</param>
/// <param name="Name">The text id of this entity</param>
/// <param name="DisplayName">Minecraft's display name for this entity</param>
/// <param name="Width">The width of this entity</param>
/// <param name="Height">The height of this entity</param>
/// <param name="MobType"></param>
/// <param name="Category"></param>
public record EntityInfo(int Id, EntityType Type, string Name, string DisplayName, float Width, float Height, MobType MobType, EntityCategory Category);
