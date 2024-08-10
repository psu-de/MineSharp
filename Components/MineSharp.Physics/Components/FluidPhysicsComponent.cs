using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Physics.Input;
using MineSharp.World;

namespace MineSharp.Physics.Components;

internal class FluidPhysicsComponent(MinecraftPlayer player, IWorld world, MovementInput input, PlayerState state)
    : PhysicsComponent(player, world, input, state)
{
    private const float MaxFluidHeight = 9.0f;

    private static readonly Vector3[] XzPlane = { Vector3.North, Vector3.East, Vector3.South, Vector3.West };

    private readonly HashSet<BlockType> fluidOnEyes = new();

    public override void Tick()
    {
        State.WasUnderwater = fluidOnEyes.Contains(BlockType.Water);

        // Entity.java:421 vehicle movement

        State.WaterHeight = 0.0;
        State.LavaHeight = 0.0;

        if (DoFluidPushing(BlockType.Water, 0.014d, out var waterHeight))
        {
            State.WasTouchingWater = true;
        }
        else
        {
            State.WasTouchingWater = false;
        }

        var lavaFactor = Player.Dimension == Dimension.Nether
            ? 0.007D
            : 0.0023333333333333335D;

        DoFluidPushing(BlockType.Lava, lavaFactor, out var lavaHeight);

        State.WaterHeight = waterHeight;
        State.LavaHeight = lavaHeight;
    }


    private bool DoFluidPushing(BlockType type, double factor, out double height)
    {
        var aabb = Player.Entity!
                         .GetBoundingBox()
                         .Deflate(0.001d, 0.001d, 0.001d);

        var fromX = (int)Math.Floor(aabb.Min.X);
        var toX = (int)Math.Ceiling(aabb.Max.X);
        var fromY = (int)Math.Floor(aabb.Min.Y);
        var toY = (int)Math.Ceiling(aabb.Max.Y);
        var fromZ = (int)Math.Floor(aabb.Min.Z);
        var toZ = (int)Math.Ceiling(aabb.Max.Z);
        var d0 = 0.0d;
        var result = false;
        var vel = Vector3.Zero.Clone();
        var k1 = 0;

        var pos = Position.Zero;
        for (var x = fromX; x < toX; ++x) // TODO: Implement world iterators, that would be nicer
        {
            for (var y = fromY; y < toY; ++y)
            {
                for (var z = fromZ; z < toZ; ++z)
                {
                    pos = new(x, y, z);
                    var block = World.GetBlockAt(pos);

                    if (block.Info.Type != type)
                    {
                        continue;
                    }

                    var fHeight = y + GetFluidHeight(World, block);
                    if (fHeight < aabb.Min.Y)
                    {
                        continue;
                    }

                    result = true;
                    d0 = Math.Max(fHeight - aabb.Min.Y, d0);
                    var flow = GetFlow(World, block);

                    if (d0 < 0.4d)
                    {
                        flow.Scale(d0);
                    }

                    vel.Add(flow);
                    k1++;
                }
            }
        }

        height = d0;

        if (vel.LengthSquared() == 0)
        {
            return result;
        }

        if (k1 > 0)
        {
            vel.Scale(1.0d / k1);
        }

        vel.Scale(factor);

        var delta = Player.Entity!.Velocity;
        if (Math.Abs(delta.X) < PhysicsConst.VelocityThreshold
            && Math.Abs(delta.Z) < PhysicsConst.VelocityThreshold
            && vel.Length() < PhysicsConst.VelocityScale)
        {
            vel.Normalize();
            vel.Scale(PhysicsConst.VelocityScale);
        }

        (Player.Entity!.Velocity as MutableVector3)!.Add(vel);

        return result;
    }

    private float GetFluidHeight(IWorld world, Block block)
    {
        var blockAbove = world.GetBlockAt(
            (Position)Vector3.Up.Plus(block.Position));

        if (blockAbove.Info.Type == block.Info.Type)
        {
            return 1.0f;
        }

        return (8 - block.GetProperty<int>("level")) / MaxFluidHeight;
    }

    private MutableVector3 GetFlow(IWorld world, Block fluid)
    {
        var dX = 0.0d;
        var dZ = 0.0d;
        var pos = Position.Zero;
        var height = GetFluidHeight(world, fluid);

        foreach (var direction in XzPlane)
        {
            pos = new(
                fluid.Position.X + (int)direction.X,
                fluid.Position.Y + (int)direction.Y,
                fluid.Position.Z + (int)direction.Z);

            var fluid2 = world.GetBlockAt(pos);
            if (!AffectsFlow(fluid, fluid2))
            {
                continue;
            }

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
                        {
                            heightDiff = GetFluidHeight(world, fluid) - (f - 0.8888889f);
                        }
                    }
                }
            }
            else if (f > 0.0f)
            {
                heightDiff = height - f;
            }

            dX += direction.X * heightDiff;
            dZ += direction.Z * heightDiff;
        }

        var vel = new MutableVector3(dX, 0, dZ);

        // TODO: FluidUtils.GetFlow() difference with java:
        // Block property falling
        // FlowingFluid.java:85

        return vel.Normalize();
    }

    private bool AffectsFlow(Block first, Block second)
    {
        return first.Info.Type == second.Info.Type || second.IsFluid() || !second.IsSolid();
    }

    private bool BlocksMotion(Block block)
    {
        return block.Info.Type != BlockType.Cobweb
            && block.Info.Type != BlockType.BambooSapling
            && block.IsSolid()
            && !block.IsFluid();
    }
}
