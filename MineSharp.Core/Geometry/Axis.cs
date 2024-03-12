namespace MineSharp.Core.Geometry;

/// <summary>
/// Represents a basis axis
/// </summary>
public abstract class Axis
{
    /// <summary>
    /// The default X Axis
    /// </summary>
    public static readonly Axis X = XAxis.Instance;
    
    /// <summary>
    /// The default Y Axis
    /// </summary>
    public static readonly Axis Y = YAxis.Instance;
    
    /// <summary>
    /// The default Z Axis
    /// </summary>
    public static readonly Axis Z = ZAxis.Instance;

    /// <summary>
    /// The next axis
    /// </summary>
    public abstract Axis Next     { get; }
    
    /// <summary>
    /// The previous axis
    /// </summary>
    public abstract Axis Previous { get; }

    /// <summary>
    /// Returns the coordinate of this axis
    /// </summary>
    public abstract double Choose(double x, double y, double z);

    /// <summary>
    /// Returns the coordinate of this axis
    /// </summary>
    public double Choose(Vector3 vec)
        => this.Choose(vec.X, vec.Y, vec.Z);

    private class XAxis : Axis
    {
        public static readonly XAxis Instance = new();

        public override Axis Next     => YAxis.Instance;
        public override Axis Previous => ZAxis.Instance;

        public override double Choose(double x, double y, double z) => x;
        
        private XAxis() { }
    }

    private class YAxis : Axis
    {
        public static readonly YAxis Instance = new();

        public override Axis Next     => ZAxis.Instance;
        public override Axis Previous => XAxis.Instance;

        public override double Choose(double x, double y, double z) => y;

        private YAxis() { }
    }

    private class ZAxis : Axis
    {
        public static readonly ZAxis Instance = new();
        
        public override Axis Next     => XAxis.Instance;
        public override Axis Previous => YAxis.Instance;

        public override double Choose(double x, double y, double z) => z;

        private ZAxis() { }
    }
}
