namespace MineSharp.World.Chunks
{
    public struct ChunkCoordinates
    {

        public int X;
        public int Z;

        public ChunkCoordinates(int x, int z)
        {
            this.X = x;
            this.Z = z;
        }

        public override string ToString() => $"({this.X} / {this.Z})";

    }
}
