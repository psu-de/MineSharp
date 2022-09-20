using System.Reflection.Metadata.Ecma335;

namespace MineSharp.Core.Types {
    public class Vector3 {

        public static Vector3 Zero => new Vector3(0, 0, 0);
        public static Vector3 Up => new Vector3(0, 1, 0);
        public static Vector3 Down => new Vector3(0, -1, 0);
        public static Vector3 North => new Vector3(0, 0, -1);
        public static Vector3 South => new Vector3(0, 0, 1);
        public static Vector3 West => new Vector3(-1, 0, 0);
        public static Vector3 East => new Vector3(1, 0, 0);

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static implicit operator Position(Vector3 x) => new Position((int)Math.Floor(x.X), (int)Math.Ceiling(x.Y), (int)Math.Floor(x.Z));
        public static explicit operator Vector3(Position x) => new Vector3(x.X, x.Y, x.Z);

        public Vector3(double x, double y, double z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void Add(Vector3 v) {
            this.X += v.X;
            this.Y += v.Y;
            this.Z += v.Z;
        }

        public void Subtract(Vector3 v) {
            this.X -= v.X;
            this.Y -= v.Y;
            this.Z -= v.Z;
        }

        public Vector3 Normalized () {
            return this * Length();
        }

        public Vector3 Clone() {
            return new Vector3(this.X, this.Y, this.Z);
        }

        public double Length() {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }

        public Vector3 Floored() {
            return new Vector3(Math.Floor(X), Math.Floor(Y), Math.Floor(Z));    
        }

        public Vector3 Ceiled() {
            return new Vector3(Math.Ceiling(X), Math.Ceiling(Y), Math.Ceiling(Z));
        }

        public Vector3 Plus(Vector3 v) {
            return new Vector3(this.X + v.X, this.Y + v.Y, this.Z + v.Z);
        }

        public Vector3 Minus(Vector3 v) {
            return new Vector3(this.X - v.X, this.Y - v.Y, this.Z - v.Z);
        }

        public void Mul(Vector3 v)
        {
            this.X *= v.X;
            this.Y *= v.Y;
            this.Z *= v.Z;
        }

        public Vector3 Multiply(Vector3 v)
        {
            return new Vector3(this.X * v.X, this.Y * v.Y, this.Z * v.Z);
        }

        public double DistanceSquared(Vector3 v) {
            var diff = this.Minus(v);
            return diff.X * diff.X + diff.Y * diff.Y + diff.Z * diff.Z;
        }

        public double DotProduct(Vector3 v) {
            return this.X * v.X + this.Y * v.Y + this.Z * v.Z;
        }

        public double Distance(Vector3 v) {
            return Math.Sqrt(this.DistanceSquared(v));
        }

        public double Angle(Vector3 v) {
            var dot = this.DotProduct(v);
            return Math.Acos(dot / this.Length() * v.Length());
        }


        public override string ToString() {
            return $"({X.ToString("0.##")} / {Y.ToString("0.##")} / {Z.ToString("0.##")})";
        }



        public static Vector3 operator *(Vector3 v, int val) {
            return new Vector3(v.X * val, v.Y * val, v.Z * val);
        }

        public static Vector3 operator *(Vector3 v, double val) {
            return new Vector3(v.X * val, v.Y * val, v.Z * val);
        }

        public static Vector3 operator /(Vector3 v, int val) {
            return new Vector3(v.X / val, v.Y / val, v.Z / val);
        }
    }
}
