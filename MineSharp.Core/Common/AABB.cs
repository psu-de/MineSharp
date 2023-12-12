namespace MineSharp.Core.Common;

public class AABB
{
    public double MinX { get; set; }
    public double MinY { get; set; }
    public double MinZ { get; set; }
    public double MaxX { get; set; }
    public double MaxY { get; set; }
    public double MaxZ { get; set; }
    public double Width => this.MaxX - this.MinX;
    public double Height => this.MaxY - this.MinY;
    public double Depth => this.MaxZ - this.MinZ;

    public AABB(float[] data)
    {
        if (data.Length != 6)
        {
            throw new ArgumentException($"Expected array with length of 6.", nameof(data));
        }

        this.MinX = data[0];
        this.MinY = data[1];
        this.MinZ = data[2];
        this.MaxX = data[3];
        this.MaxY = data[4];
        this.MaxZ = data[5];
    }
    
    public AABB(double x0, double y0, double z0, double x1, double y1, double z1)
    {
        this.MinX = x0;
        this.MinY = y0;
        this.MinZ = z0;
        this.MaxX = x1;
        this.MaxY = y1;
        this.MaxZ = z1;
    }
    

    public AABB Contract(double x, double y, double z)
    {
        this.MinX += x;
        this.MinY += y;
        this.MinZ += z;
        this.MaxX -= x;
        this.MaxY -= y;
        this.MaxZ -= z;
        return this;
    }

    public AABB Offset(double x, double y, double z)
    {
        this.MinX += x;
        this.MinY += y;
        this.MinZ += z;
        this.MaxX += x;
        this.MaxY += y;
        this.MaxZ += z;
        return this;
    }

    public bool Intersects(AABB other)
    {
        if (this.MaxX >= other.MinX && this.MinX <= other.MaxX)
        {
            if (this.MaxY < other.MinY || this.MinY > other.MaxY)
            {
                return false;
            }
            return this.MaxZ >= other.MinZ && this.MinZ <= other.MaxZ;
        }
        return false;
    }

    public bool Contains(double x, double y, double z) => this.MinX <= x && this.MaxX >= x &&
                                                          this.MinY <= y && this.MaxY >= y &&
                                                          this.MinZ <= z && this.MaxZ >= z;
    
    public override string ToString() => $"AABB (MinX={this.MinX} MaxX={this.MaxX} MinY={this.MinY} MaxY={this.MaxY} MinZ={this.MinZ} MaxZ={this.MaxZ})";
}