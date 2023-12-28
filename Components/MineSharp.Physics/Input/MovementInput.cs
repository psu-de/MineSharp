namespace MineSharp.Physics.Input;

internal class MovementInput(InputControls controls)
{
    public readonly InputControls Controls = controls;

    public float ForwardImpulse { get; set; }
    public float StrafeImpulse { get; set; }
    public bool JumpedThisTick { get; set; }

    public void Tick(bool slow, float sneakFactor)
    {
        if (!this.Controls.JumpingKeyDown)
            this.JumpedThisTick = false;
        else if (!this.JumpedThisTick)
            this.JumpedThisTick = true;
        
        this.ForwardImpulse = CalculateImpulse(this.Controls.ForwardKeyDown, this.Controls.BackwardKeyDown);
        this.StrafeImpulse = CalculateImpulse(this.Controls.LeftKeyDown, this.Controls.RightKeyDown);
        
        if (!slow)
            return;

        this.ForwardImpulse *= sneakFactor;
        this.StrafeImpulse *= sneakFactor;
    }

    public bool HasForwardImpulse()
        => this.ForwardImpulse > 1.0E-5F;
    
    public bool HasSprintingImpulse(bool isUnderwater)
    {
        return isUnderwater
            ? this.HasForwardImpulse()
            : this.ForwardImpulse > 0.8F;
    }

    private float CalculateImpulse(bool a, bool b)
    {
        if (a == b) 
            return 0.0f;

        return a
            ? 1.0f
            : -1.0f;
    }
}
