using MineSharp.Core.Common;

namespace MineSharp.Physics.Utils;

internal abstract class Axis
{
    public static readonly Axis X = XAxis.Instance;
    public static readonly Axis Y = YAxis.Instance;
    public static readonly Axis Z = ZAxis.Instance;

    public abstract Axis Next { get; }
    public abstract Axis Previous { get; }

    public abstract double Choose(double x, double y, double z);
    public abstract double GetBBMin(AABB aabb);
    public abstract double GetBBMax(AABB aabb);
    
    private class XAxis : Axis
    {
        public static XAxis Instance = new ();

        public override Axis Next => YAxis.Instance;
        public override Axis Previous => ZAxis.Instance;

        public override double Choose(double x, double y, double z) => x;
        public override double GetBBMin(AABB aabb) => aabb.MinX;
        public override double GetBBMax(AABB aabb) => aabb.MaxX;
    }

    private class YAxis : Axis
    {
        public static YAxis Instance = new ();
    
    
        public override Axis Next => ZAxis.Instance;
        public override Axis Previous => XAxis.Instance;

        public override double Choose(double x, double y, double z) => y;
        public override double GetBBMin(AABB aabb) => aabb.MinY;
        public override double GetBBMax(AABB aabb) => aabb.MaxY;
    }

    private class ZAxis : Axis
    {
        public static ZAxis Instance = new ();
    
    
        public override Axis Next => XAxis.Instance;
        public override Axis Previous => YAxis.Instance;

        public override double Choose(double x, double y, double z) => z;
        public override double GetBBMin(AABB aabb) => aabb.MinZ;
        public override double GetBBMax(AABB aabb) => aabb.MaxZ;
    }
}