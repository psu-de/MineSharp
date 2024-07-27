namespace MineSharp.Core.Common.Effects;

/// <summary>
///     Effect descriptor class
/// </summary>
/// <param name="Id">The numerical id of this effect (depends on Minecraft version)</param>
/// <param name="Type">The <see cref="EffectType" /> of this effect (independent of Minecraft version)</param>
/// <param name="Name">The text id of this effect</param>
/// <param name="DisplayName">Minecraft's display name of this effect</param>
/// <param name="IsGood">Whether this effect is considered good</param>
public record EffectInfo(int Id, EffectType Type, string Name, string DisplayName, bool IsGood);
