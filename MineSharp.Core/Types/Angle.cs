namespace MineSharp.Core.Types {
    public class Angle {

        private const float CONV = 256 / 360;

        public static Angle FromByte(byte b) {
            return new Angle(b / CONV);
        }

        public float Value { get; private set; }

        public Angle(float value) { Value = value; }

        public static implicit operator float(Angle a) { return a.Value; }
        public static explicit operator Angle(float a) { return new Angle(a); }

        public byte ToByte() {
            return (byte)Math.Floor(this.Value * CONV);
        }
    }
}
