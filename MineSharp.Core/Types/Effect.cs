namespace MineSharp.Core.Types
{
    public class EffectInfo
    {
        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public bool IsGood { get; }

        public EffectInfo(int id, string name, string displayName, bool isGood)
        {
            this.Id = id;
            this.Name = name;
            this.DisplayName = displayName;
            this.IsGood = isGood;
        }
    }

    public class Effect
    {
        public Effect(EffectInfo info, int amplifier, DateTime startTime, int duration)
        {
            this.Info = info;
            this.Amplifier = amplifier;
            this.StartTime = startTime;
            this.Duration = duration;
        }

        public EffectInfo Info { get; }
        public DateTime StartTime { get; set; }
        public int Amplifier { get; set; }
        public int Duration { get; set; }

        public override string ToString() => $"Effect (Name={this.Info.Name} Id={this.Info.Id} Amplifier={this.Amplifier} Duration={this.Duration})";
    }
}
