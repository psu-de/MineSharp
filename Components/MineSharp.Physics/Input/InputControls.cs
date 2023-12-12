namespace MineSharp.Physics.Input;

public class InputControls
{
    /// <summary>
    /// Whether the 'up' key is down (default W key)
    /// </summary>
    public bool UpKeyDown { get; set; }
    
    /// <summary>
    /// Whether the 'down' key is down (default S key)
    /// </summary>
    public bool DownKeyDown { get; set; }
    
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
}
