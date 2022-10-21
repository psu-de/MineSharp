namespace MineSharp.Core.Types
{
    public class AABB
    {


        public AABB(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            this.MinX = x0;
            this.MinY = y0;
            this.MinZ = z0;
            this.MaxX = x1;
            this.MaxY = y1;
            this.MaxZ = z1;
        }

        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MinZ { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double MaxZ { get; set; }

        public override string ToString() => $"AABB (MinX={this.MinX} MaxX={this.MaxX} MinY={this.MinY} MaxY={this.MaxY} MinZ={this.MinZ} MaxZ={this.MaxZ})";

        public AABB Clone() => new AABB(this.MinX, this.MinY, this.MinZ, this.MaxX, this.MaxY, this.MaxZ);

        public void Floor()
        {
            this.MinX = Math.Floor(this.MinX);
            this.MinY = Math.Floor(this.MinY);
            this.MinZ = Math.Floor(this.MinZ);
            this.MaxX = Math.Floor(this.MaxX);
            this.MaxY = Math.Floor(this.MaxY);
            this.MaxZ = Math.Floor(this.MaxZ);
        }

        public AABB Extend(double dx, double dy, double dz)
        {
            if (dx < 0) this.MinX += dx;
            else this.MaxX += dx;

            if (dy < 0) this.MinY += dy;
            else this.MaxY += dy;

            if (dz < 0) this.MinZ += dz;
            else this.MaxZ += dz;


            return this;
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

        public AABB Expand(double x, double y, double z)
        {
            this.MinX -= x;
            this.MinY -= y;
            this.MinZ -= z;
            this.MaxX += x;
            this.MaxY += y;
            this.MaxZ += z;
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

        public double ComputeOffsetX(AABB other, double offsetX)
        {
            if (other.MaxY > this.MinY && other.MinY < this.MaxY && other.MaxZ > this.MinZ && other.MinZ < this.MaxZ)
            {
                if (offsetX > 0.0 && other.MaxX <= this.MinX)
                {
                    offsetX = Math.Min(this.MinX - other.MaxX, offsetX);
                } else if (offsetX < 0.0 && other.MinX >= this.MaxX)
                {
                    offsetX = Math.Max(this.MaxX - other.MinX, offsetX);
                }
            }
            return offsetX;
        }

        public double ComputeOffsetY(AABB other, double offsetY)
        {
            if (other.MaxX > this.MinX && other.MinX < this.MaxX && other.MaxZ > this.MinZ && other.MinZ < this.MaxZ)
            {
                if (offsetY > 0.0 && other.MaxY <= this.MinY)
                {
                    offsetY = Math.Min(this.MinY - other.MaxY, offsetY);
                } else if (offsetY < 0.0 && other.MinY >= this.MaxY)
                {
                    offsetY = Math.Max(this.MaxY - other.MinY, offsetY);
                }
            }
            return offsetY;
        }

        public double ComputeOffsetZ(AABB other, double offsetZ)
        {
            if (other.MaxX > this.MinX && other.MinX < this.MaxX && other.MaxY > this.MinY && other.MinY < this.MaxY)
            {
                if (offsetZ > 0.0 && other.MaxZ <= this.MinZ)
                {
                    offsetZ = Math.Min(this.MinZ - other.MaxZ, offsetZ);
                } else if (offsetZ < 0.0 && other.MinZ >= this.MaxZ)
                {
                    offsetZ = Math.Max(this.MaxZ - other.MinZ, offsetZ);
                }
            }
            return offsetZ;
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
    }
}
