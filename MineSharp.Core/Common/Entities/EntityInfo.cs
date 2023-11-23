namespace MineSharp.Core.Common.Entities;

public record EntityInfo(int Id, EntityType Type, string Name, string DisplayName, float Width, float Height, MobType MobType, EntityCategory Category);
