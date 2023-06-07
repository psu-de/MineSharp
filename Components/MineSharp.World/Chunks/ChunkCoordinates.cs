namespace MineSharp.World.Chunks;

public readonly struct ChunkCoordinates
{
    public int X { get; }
    public int Z { get; }

    public ChunkCoordinates(int x, int z)
    {
        this.X = x;
        this.Z = z;
    }

    public override string ToString() => $"({this.X} / {this.Z})";

    public override int GetHashCode() => this.X << 16 | this.Z & 0xFFFF;
}
