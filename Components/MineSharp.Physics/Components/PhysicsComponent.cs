using MineSharp.Core.Common.Entities;
using MineSharp.Physics.Input;
using MineSharp.World;

namespace MineSharp.Physics.Components;

internal abstract class PhysicsComponent(MinecraftPlayer player, IWorld world, MovementInput input, PlayerState state)
{
    protected readonly MovementInput Input = input;
    protected readonly MinecraftPlayer Player = player;
    protected readonly PlayerState State = state;
    protected readonly IWorld World = world;

    public abstract void Tick();
}
