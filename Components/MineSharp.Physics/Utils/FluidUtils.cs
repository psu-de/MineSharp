using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.World;

namespace MineSharp.Physics.Utils;

public static class FluidUtils
{
    private const float MAX_FLUID_HEIGHT = 9.0f;

    private static readonly Vector3[] XZPlane = new[] {
        Vector3.North, Vector3.East, Vector3.South, Vector3.West
    };
    
    public static float GetFluidHeight(IWorld world, Block block)
    {
        var blockAbove = world.GetBlockAt(
            (Position)Vector3.Up.Plus(block.Position));

        if (blockAbove.Info.Type == block.Info.Type)
            return 1.0f;

        return (8 - block.GetProperty<int>("level")) / MAX_FLUID_HEIGHT;
    }

    public static Vector3 GetFlow(IWorld world, Block fluid)
    {
        var dX = 0.0d;
        var dZ = 0.0d;
        var pos = new MutablePosition(0, 0, 0);
        var height = GetFluidHeight(world, fluid);

        foreach (var direction in XZPlane)
        {
            pos.Set(
                fluid.Position.X + (int)direction.X,
                fluid.Position.Y + (int)direction.Y,
                fluid.Position.Z + (int)direction.Z);
            
            var fluid2 = world.GetBlockAt(pos);
            if (!AffectsFlow(fluid, fluid2))
                continue;

            var f = GetFluidHeight(world, fluid2);
            var heightDiff = 0.0f;
            
            if (0.0f == f)
            {
                if (!BlocksMotion(world.GetBlockAt(pos)))
                {
                    var below = (Position)Vector3.Down.Plus(pos);
                    var fluid3 = world.GetBlockAt(below);
                    if (AffectsFlow(fluid, fluid3))
                    {
                        f = GetFluidHeight(world, fluid3);
                        if (f > 0.0f)
                            heightDiff = GetFluidHeight(world, fluid) - (f - 0.8888889f);
                    }
                }
            }
            else if (f > 0.0f)
                heightDiff = height - f;

            dX += direction.X * heightDiff;
            dZ += direction.Z * heightDiff;
        }
        
        var vel = new Vector3(dX, 0, dZ);
        
        // TODO: FluidUtils.GetFlow() difference with java:
        // Block property falling
        // FlowingFluid.java:85

        return vel.Normalized();
    }

    private static bool AffectsFlow(Block first, Block second)
    {
        return first.Info.Type == second.Info.Type || second.IsFluid() || !second.IsSolid();
    }

    private static bool BlocksMotion(Block block)
    {
        return block.Info.Type != BlockType.Cobweb
               && block.Info.Type != BlockType.BambooSapling
               && block.IsSolid() 
               && !block.IsFluid();
    }
}
