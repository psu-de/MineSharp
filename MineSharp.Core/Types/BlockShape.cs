namespace MineSharp.Core.Types
{
    public class BlockShape
    {

        public float[] Shape;

        public BlockShape(float[] shape)
        {
            this.Shape = shape;
        }

        public AABB ToBoundingBox() => new AABB(this.Shape[0], this.Shape[1], this.Shape[2], this.Shape[3], this.Shape[4], this.Shape[5]);
    }
}
