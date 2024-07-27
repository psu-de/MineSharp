namespace MineSharp.Core.Common.Effects;

/// <summary>
///     An effect
/// </summary>
/// <param name="info"></param>
/// <param name="amplifier"></param>
/// <param name="startTime"></param>
/// <param name="duration"></param>
public class Effect(EffectInfo info, int amplifier, DateTime startTime, int duration)
{
    /// <summary>
    ///     Descriptor of this Effect
    /// </summary>
    public readonly EffectInfo Info = info;

    /// <summary>
    ///     When this effect has started
    /// </summary>
    public DateTime StartTime { get; set; } = startTime;

    /// <summary>
    ///     Amplifier of this effect
    /// </summary>
    public int Amplifier { get; set; } = amplifier;

    /// <summary>
    ///     Duration of this effect in Ticks
    /// </summary>
    public int Duration { get; set; } = duration;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Effect (Name={Info.Name} Id={Info.Id} Amplifier={Amplifier} Duration={Duration})";
    }
}
