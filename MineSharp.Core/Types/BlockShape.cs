namespace MineSharp.Core.Types {
    public class BlockShape {

        public float[] Shape;

        public BlockShape(float[] shape) {
            Shape = shape;
        }

        public AABB ToBoundingBox() {
            return new AABB(Shape[0], Shape[1], Shape[2], Shape[3], Shape[4], Shape[5]);
        }
    }
}
