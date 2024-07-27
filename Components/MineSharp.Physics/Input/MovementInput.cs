namespace MineSharp.Physics.Input;

internal class MovementInput(InputControls controls)
{
    public readonly InputControls Controls = controls;

    public float ForwardImpulse { get; set; }
    public float StrafeImpulse { get; set; }
    public bool JumpedThisTick { get; set; }

    public void Tick(bool slow, float sneakFactor)
    {
        if (!Controls.JumpingKeyDown)
        {
            JumpedThisTick = false;
        }
        else if (!JumpedThisTick)
        {
            JumpedThisTick = true;
        }

        ForwardImpulse = CalculateImpulse(Controls.ForwardKeyDown, Controls.BackwardKeyDown);
        StrafeImpulse = CalculateImpulse(Controls.LeftKeyDown, Controls.RightKeyDown);

        if (!slow)
        {
            return;
        }

        ForwardImpulse *= sneakFactor;
        StrafeImpulse *= sneakFactor;
    }

    public bool HasForwardImpulse()
    {
        return ForwardImpulse > 1.0E-5F;
    }

    public bool HasSprintingImpulse(bool isUnderwater)
    {
        return isUnderwater
            ? HasForwardImpulse()
            : ForwardImpulse > 0.8F;
    }

    private float CalculateImpulse(bool a, bool b)
    {
        if (a == b)
        {
            return 0.0f;
        }

        return a
            ? 1.0f
            : -1.0f;
    }
}
