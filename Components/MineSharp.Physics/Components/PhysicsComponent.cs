using MineSharp.Core.Common.Entities;
using MineSharp.Physics.Input;
using MineSharp.World;

namespace MineSharp.Physics.Components;

public abstract class PhysicsComponent(MinecraftPlayer player, IWorld world, MovementInput input, PlayerState state)
{
    protected readonly MinecraftPlayer Player = player;
    protected readonly IWorld World = world;
    protected readonly MovementInput Input = input;
    protected readonly PlayerState State = state;

    public abstract void Tick();
}
