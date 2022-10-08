namespace MineSharp.Core.Types
{
    public abstract class Effect
    {

        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public bool IsGood { get; }

        public DateTime StartTime { get; set; }
        public int Amplifier { get; set; }
        public int Duration { get; set; }

        public Effect(int id, string name, string displayName, bool isGood)
        {
            this.Id = id;
            this.Name = name;
            this.DisplayName = displayName;
            this.IsGood = isGood;
        }

        public Effect(int amplifier, DateTime startTime, int duration, int id, string name, string displayName, bool isGood)
            : this(id, name, displayName, isGood)
        {
            this.Amplifier = amplifier;
            this.StartTime = startTime;
            this.Duration = duration;
        }

        public override string ToString() => $"Effect (Name={this.Name} Id={this.Id} Amplifier={this.Amplifier} Duration={this.Duration})";

    }
}
