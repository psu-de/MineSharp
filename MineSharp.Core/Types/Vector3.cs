namespace MineSharp.Core.Types
{
    public class Vector3
    {

        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static Vector3 One => new Vector3(1, 1, 1);
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

        public void Add(Vector3 v)
        {
            this.X += v.X;
            this.Y += v.Y;
            this.Z += v.Z;
        }

        public void Subtract(Vector3 v)
        {
            this.X -= v.X;
            this.Y -= v.Y;
            this.Z -= v.Z;
        }

        public Vector3 Normalized() => this * this.Length();

        public Vector3 Clone() => new Vector3(this.X, this.Y, this.Z);

        public double Length() => Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);

        public Vector3 Floored() => new Vector3(Math.Floor(this.X), Math.Floor(this.Y + 0.001), Math.Floor(this.Z));

        public Vector3 Ceiled() => new Vector3(Math.Ceiling(this.X), Math.Ceiling(this.Y), Math.Ceiling(this.Z));

        public Vector3 Plus(Vector3 v) => new Vector3(this.X + v.X, this.Y + v.Y, this.Z + v.Z);

        public Vector3 Minus(Vector3 v) => new Vector3(this.X - v.X, this.Y - v.Y, this.Z - v.Z);

        public void Mul(Vector3 v)
        {
            this.X *= v.X;
            this.Y *= v.Y;
            this.Z *= v.Z;
        }

        public Vector3 Multiply(Vector3 v) => new Vector3(this.X * v.X, this.Y * v.Y, this.Z * v.Z);

        public double DistanceSquared(Vector3 v)
        {
            var diff = this.Minus(v);
            return diff.X * diff.X + diff.Y * diff.Y + diff.Z * diff.Z;
        }

        public double DotProduct(Vector3 v) => this.X * v.X + this.Y * v.Y + this.Z * v.Z;

        public double Distance(Vector3 v) => Math.Sqrt(this.DistanceSquared(v));

        public double Angle(Vector3 v)
        {
            var dot = this.DotProduct(v);
            return Math.Acos(dot / this.Length() * v.Length());
        }


        public override string ToString() => $"({this.X.ToString("0.##")} / {this.Y.ToString("0.##")} / {this.Z.ToString("0.##")})";



        public static Vector3 operator *(Vector3 v, int val) => new Vector3(v.X * val, v.Y * val, v.Z * val);

        public static Vector3 operator *(Vector3 v, double val) => new Vector3(v.X * val, v.Y * val, v.Z * val);

        public static Vector3 operator /(Vector3 v, int val) => new Vector3(v.X / val, v.Y / val, v.Z / val);

        public static bool operator ==(Vector3 v1, Vector3 v2) => v1.X == v2.X &&
                                                                  v1.Y == v2.Y &&
                                                                  v1.Z == v2.Z;

        public static bool operator !=(Vector3 v1, Vector3 v2) => v1.X != v2.X ||
                                                                  v1.Y != v2.Y ||
                                                                  v1.Z != v2.Z;
    }
}
