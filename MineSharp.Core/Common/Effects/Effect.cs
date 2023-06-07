namespace MineSharp.Core.Common.Effects;

public class Effect
{
    public readonly EffectInfo Info;
    public DateTime StartTime { get; set; }
    public int Amplifier { get; set; }
    public int Duration { get; set; }

    public Effect(EffectInfo info, int amplifier, DateTime startTime, int duration)
    {
        this.Info = info;
        this.StartTime = startTime;
        this.Amplifier = amplifier;
        this.Duration = duration;
    }
    
    public override string ToString() => $"Effect (Name={this.Info.Name} Id={this.Info.Id} Amplifier={this.Amplifier} Duration={this.Duration})";
}
