namespace MineSharp.Physics.Input;

/// <summary>
/// Represents the Input keys for the physics simulation
/// </summary>
public class InputControls
{
    /// <summary>
    /// Whether the 'up' key is down (default W key)
    /// </summary>
    public bool ForwardKeyDown { get; set; }
    
    /// <summary>
    /// Whether the 'down' key is down (default S key)
    /// </summary>
    public bool BackwardKeyDown { get; set; }
    
    /// <summary>
    /// Whether the 'left' key is down (default A key)
    /// </summary>
    public bool LeftKeyDown { get; set; }
    
    /// <summary>
    /// Whether the 'right' key is down (default D key)
    /// </summary>
    public bool RightKeyDown { get; set; }
    
    /// <summary>
    /// Whether the 'jump' key is down (default Space key)
    /// </summary>
    public bool JumpingKeyDown { get; set; }
    
    /// <summary>
    /// Whether the 'shift' key is down (default Shift key)
    /// </summary>
    public bool SneakingKeyDown { get; set; }
    
    /// <summary>
    /// Whether the sprinting key is down
    /// </summary>
    public bool SprintingKeyDown { get; set; }

    /// <summary>
    /// Set all input controls to false.
    /// </summary>
    public void Reset()
    {
        this.ForwardKeyDown = false;
        this.BackwardKeyDown = false;
        this.LeftKeyDown = false;
        this.RightKeyDown = false;
        this.JumpingKeyDown = false;
        this.SneakingKeyDown = false;
        this.SprintingKeyDown = false;
    }
}
