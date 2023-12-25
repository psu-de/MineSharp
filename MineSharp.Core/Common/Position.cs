namespace MineSharp.Core.Common;

public class Position
{
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public int Z { get; protected set; }
    
    public Position(ulong value)
    {
        this.X = (int)(value >> 38);
        this.Y = (int)(value & 0xFFF);
        this.Z = (int)(value >> 12 & 0x3FFFFFF);

        if (this.X >= Math.Pow(2, 25)) { this.X -= (int)Math.Pow(2, 26); }
        if (this.Y >= Math.Pow(2, 11)) { this.Y -= (int)Math.Pow(2, 12); }
        if (this.Z >= Math.Pow(2, 25)) { this.Z -= (int)Math.Pow(2, 26); }
    }

    public Position(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Position(double x, double y, double z)
    {
        this.X = (int)Math.Floor(x);
        this.Y = (int)Math.Floor(y);
        this.Z = (int)Math.Floor(z);
    }

    public static bool operator ==(Position a, Position b)
    {
        return a.ToULong() == b.ToULong();
    }
    
    public static bool operator !=(Position a, Position b)
    {
        return a.ToULong() != b.ToULong();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Position pos)
        {
            return pos.ToULong() == this.ToULong();
        }

        return false;
    }

    public ulong ToULong() => ((ulong)this.X & 0x3FFFFFF) << 38 | ((ulong)this.Z & 0x3FFFFFF) << 12 | (ulong)this.Y & 0xFFF;

    public override string ToString() => $"({this.X} / {this.Y} / {this.Z})";

    public override int GetHashCode() => this.X << 22 | this.Z << 12 & 0x3FF | this.Y & 0xFFF;
    
    public static explicit operator Vector3(Position x) => new Vector3(x.X, x.Y, x.Z);
}

public class MutablePosition : Position
{

    public MutablePosition(ulong value) : base(value)
    { }
    
    public MutablePosition(int x, int y, int z) : base(x, y, z)
    { }

    public void Set(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}