namespace MineSharp.Core.Types {
    public class Entity {

		public int Id { get; }
		public string Name { get; }
		public string DisplayName { get; }
		public float Width { get; }
		public float Height { get; }
		public int Category { get; }


        public int ServerId { get; set; }
        public Vector3 Position { get; set; }
        public float Pitch { get; set; }
        public float PitchRadians => (MathF.PI / 180) * Pitch;
        public float Yaw { get; set; }
        public float YawRadians => (MathF.PI / 180) * Yaw;
        public Vector3 Velocity { get; set; }
        public bool IsOnGround { get; set; }
        public Dictionary<int, Effect?> Effects { get; set; }
        public Dictionary<string, Attribute> Attributes { get; set; }

        public Entity(int serverId, Vector3 position, float pitch, float yaw, Vector3 velocity, bool isOnGround, Dictionary<int, Effect?> effects, 
            int id, string name, string displayName, float width, float height, int category) {
            Id = id;
            Name = name;
            DisplayName = displayName;
            Width = width;
            Height = height;
            Category = category;
            ServerId = serverId;
            Position = position;
            Pitch = pitch;
            Yaw = yaw;
            Velocity = velocity;
            IsOnGround = isOnGround;
            Effects = effects;
            Attributes = new Dictionary<string, Attribute>();
        }

        public int? GetEffectLevel(int effectId) {

            if (!Effects.TryGetValue(effectId, out var effect)) {
                return null;
            }

            return effect?.Amplifier + 1;
        }

        public Vector3 GetDirectionVector() {
            var len = Math.Cos(this.PitchRadians);
            return new Vector3(
                len * Math.Sin(-YawRadians),
                -Math.Sin(PitchRadians),
                len * Math.Cos(YawRadians));
        }

        public override string ToString()
        {
            return $"Entity (Name={Name} Id={Id} Position={Position})";
        }
    }
}
