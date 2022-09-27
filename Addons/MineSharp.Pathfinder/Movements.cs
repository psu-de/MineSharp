namespace MineSharp.Pathfinding
{
    public class Movements
    {
        public static readonly Movements DefaultMovements = new Movements() {
            AllowSprinting = true,
        };
        


        public bool AllowSprinting { get; set; }

        public Movements(bool allowSprinting)
        {
            AllowSprinting = allowSprinting;
        }

        public Movements(){}
    }
}
