namespace MineSharp.Core.Common;

public class Vector3
{
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
    
    public Vector3(double x, double y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    /// <summary>
    /// Component-wise vector addition.
    /// </summary>
    /// <param name="other"></param>
    public void Add(Vector3 other)
    {
        this.X += other.X;
        this.Y += other.Y;
        this.Z += other.Z;
    }

    /// <summary>
    /// Component-wise vector subtraction.
    /// </summary>
    /// <param name="other"></param>
    public void Subtract(Vector3 other)
    {
        this.X -= other.X;
        this.Y -= other.Y;
        this.Z -= other.Z;
    }

    /// <summary>
    /// Component-wise vector multiplication.
    /// </summary>
    /// <param name="other"></param>
    public void Multiply(Vector3 other)
    {
        this.X *= other.X;
        this.Y *= other.Y;
        this.Z *= other.Z;
    }

    /// <summary>
    /// Component-wise vector division.
    /// </summary>
    /// <param name="other"></param>
    public void Divide(Vector3 other)
    {
        this.X /= other.X;
        this.Y /= other.Y;
        this.Z /= other.Z;
    }

    /// <summary>
    /// Scale this vector by a scalar
    /// </summary>
    /// <param name="scalar"></param>
    public void Scale(double scalar)
    {
        this.X *= scalar;
        this.Y *= scalar;
        this.Z *= scalar;
    }

    /// <summary>
    /// Returns the length of this vector.
    /// </summary>
    /// <returns></returns>
    public double Length()
    {
        return Math.Sqrt(this.LengthSquared());
    }


    /// <summary>
    /// Returns the squared length of this vector instance
    /// </summary>
    /// <returns></returns>
    public double LengthSquared()
    {
        return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
    }

    /// <summary>
    /// Returns the distance to the <see cref="other"/> vector.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double DistanceTo(Vector3 other)
    {
        return this.Minus(other).Length();
    }

    /// <summary>
    /// Returns the squared distance to the <see cref="other"/> vector
    /// </summary>
    /// <param name="other"></param>
    /// <returns>The distance squared.</returns>
    public double DistanceToSquared(Vector3 other)
    {
        var diff = this.Minus(other);
        return diff.X * diff.X + 
               diff.Y * diff.Y +
               diff.Z * diff.Z;
    }

    public void Normalize()
    {
        var length = this.Length();
        
        var scale = length == 0 
            ? 0 
            : 1 / length;
        this.X *= scale;
        this.Y *= scale;
        this.Z *= scale;
    }

    public Vector3 Normalized()
    {
        var clone = this.Clone();
        clone.Normalize();
        return clone;
    }

    public Vector3 Plus(Vector3 other)
    {
        return new Vector3(
            this.X + other.X,
            this.Y + other.Y,
            this.Z + other.Z);
    }

    public Vector3 Minus(Vector3 other)
    {
        return new Vector3(
            this.X - other.X,
            this.Y - other.Y,
            this.Z - other.Z);
    }

    public Vector3 Clone()
        => new Vector3(this.X, this.Y, this.Z);

    public override string ToString() 
        => $"({this.X:0.####} / {this.Y:0.####} / {this.Z:0.####})";

    public static explicit operator Position(Vector3 x) => new Position((int)Math.Floor(x.X), (int)Math.Ceiling(x.Y), (int)Math.Floor(x.Z));
    public static implicit operator Vector3(Position x) => new Vector3(x.X, x.Y, x.Z);
}
